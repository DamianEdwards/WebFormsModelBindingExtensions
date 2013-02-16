using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspNet.WebForms.ModelBinding.Extensions
{
    public class ExtendedModelDataSourceView : ModelDataSourceView
    {
        private bool _isAsyncSelect = false;
        private readonly ExtendedModelDataSource _owner;

        public ExtendedModelDataSourceView(ExtendedModelDataSource owner)
            : base(owner)
        {
            _owner = owner;
        }

        // Call tree:
        //   Select
        //   - ExecuteSelect
        //     - GetSelectMethodResult
        //       - EvaluateSelectMethodParamaters
        //       - InvokeMethod
        //       - ProcessSelectMethodResult
        //     - CreateSelectResult

        public override void Select(DataSourceSelectArguments arguments, DataSourceViewSelectCallback callback)
        {
            var method = FindMethod(SelectMethod);

            // Check if the method returns a Task<SelectResult<T>>
            Type taskReturnType;
            if (_owner.IsAsync && InheritsFromGenericTask(method.MethodInfo.ReturnType, out taskReturnType)
                && typeof(SelectResult).IsAssignableFrom(taskReturnType))
            {
                _isAsyncSelect = true;

                DataSourceSelectResultProcessingOptions selectResultProcessingOptions = null;
                ModelDataSourceMethod modelMethod = EvaluateSelectMethodParameters(arguments, out selectResultProcessingOptions);

                ModelDataMethodResult result = InvokeMethod(modelMethod);
                var pageAsyncTask = new PageAsyncTask(async () =>
                {
                    var task = result.ReturnValue as Task<SelectResult>;
                    var selectResult = await task;
                    if (arguments.RetrieveTotalRowCount)
                    {
                        if (!selectResult.TotalRowCount.HasValue)
                        {
                            throw new InvalidOperationException("");
                        }
                        arguments.TotalRowCount = selectResult.TotalRowCount.Value;
                    }
                    var data = CreateSelectResult(selectResult.Results);
                    
                    callback(data);
                });
                _owner.DataControl.Page.RegisterAsyncTask(pageAsyncTask);
            }
            else
            {
                base.Select(arguments, callback);
            }
        }

        protected override ModelDataSourceMethod EvaluateSelectMethodParameters(DataSourceSelectArguments arguments, out DataSourceSelectResultProcessingOptions selectResultProcessingOptions)
        {
            if (!_isAsyncSelect)
            {
                return base.EvaluateSelectMethodParameters(arguments, out selectResultProcessingOptions);
            }

            IOrderedDictionary controlValues = MergeSelectParameters(arguments);
            ModelDataSourceMethod modelDataSourceMethod = this.FindMethod(this.SelectMethod);
            Type returnType = modelDataSourceMethod.MethodInfo.ReturnType;
            Type modelType = this.ModelType;
            if (modelType == null)
            {
                foreach (Type type3 in returnType.GetGenericArguments())
                {
                    if (typeof(IQueryable<>).MakeGenericType(new Type[] { type3 }).IsAssignableFrom(returnType))
                    {
                        modelType = type3;
                    }
                }
            }
            Type type4 = (modelType != null) ? typeof(IQueryable<>).MakeGenericType(new Type[] { modelType }) : null;
            bool isReturningQueryable = (type4 != null) && type4.IsAssignableFrom(returnType);
            bool flag2 = false;
            bool flag3 = false;
            if ((arguments.StartRowIndex >= 0) && (arguments.MaximumRows > 0))
            {
                flag2 = IsAutoPagingRequired(modelDataSourceMethod.MethodInfo, isReturningQueryable, _isAsyncSelect);
            }
            if (!string.IsNullOrEmpty(arguments.SortExpression))
            {
                flag3 = IsAutoSortingRequired(modelDataSourceMethod.MethodInfo, isReturningQueryable);
            }
            selectResultProcessingOptions = new DataSourceSelectResultProcessingOptions { ModelType = modelType, AutoPage = flag2, AutoSort = flag3 };
            this.EvaluateMethodParameters(DataSourceOperation.Select, modelDataSourceMethod, controlValues);
            return modelDataSourceMethod;
        }

        private static IOrderedDictionary MergeSelectParameters(DataSourceSelectArguments arguments)
        {
            bool flag = (arguments.StartRowIndex >= 0) && (arguments.MaximumRows > 0);
            bool flag2 = !string.IsNullOrEmpty(arguments.SortExpression);
            IOrderedDictionary destination = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            if (flag2)
            {
                destination["sortByExpression"] = arguments.SortExpression;
            }
            if (flag)
            {
                IDictionary source = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
                source["maximumRows"] = arguments.MaximumRows;
                source["startRowIndex"] = arguments.StartRowIndex;
                source["totalRowCount"] = 0;
                MergeDictionaries(source, destination);
            }
            return destination;
        }

        private static bool IsAutoPagingRequired(MethodInfo selectMethod, bool isReturningQueryable, bool isAsyncSelect)
        {
            bool hasMaximumRowsParamater = false;
            bool hasTotalRowCountOutParameter = false;
            bool hasStartRowIndexParameter = false;
            foreach (ParameterInfo info in selectMethod.GetParameters())
            {
                string name = info.Name;
                if (string.Equals("startRowIndex", name, StringComparison.OrdinalIgnoreCase))
                {
                    if (info.ParameterType.IsAssignableFrom(typeof(int)))
                    {
                        hasStartRowIndexParameter = true;
                    }
                }
                else if (string.Equals("maximumRows", name, StringComparison.OrdinalIgnoreCase))
                {
                    if (info.ParameterType.IsAssignableFrom(typeof(int)))
                    {
                        hasMaximumRowsParamater = true;
                    }
                }
                else if ((string.Equals("totalRowCount", name, StringComparison.OrdinalIgnoreCase) && info.IsOut) && typeof(int).IsAssignableFrom(info.ParameterType.GetElementType()))
                {
                    hasTotalRowCountOutParameter = true;
                }
            }
            bool hasAllPagingParameters = hasMaximumRowsParamater && hasStartRowIndexParameter && hasTotalRowCountOutParameter;
            if ((!isAsyncSelect && !isReturningQueryable && !hasAllPagingParameters)
                || (isAsyncSelect && !(hasStartRowIndexParameter && hasMaximumRowsParamater)))
            {
                throw new InvalidOperationException("ModelDataSourceView_InvalidPagingParameters");
            }
            return !hasAllPagingParameters;
        }

        private static bool IsAutoSortingRequired(MethodInfo selectMethod, bool isReturningQueryable)
        {
            bool flag = false;
            foreach (ParameterInfo info in selectMethod.GetParameters())
            {
                string name = info.Name;
                if (string.Equals("sortByExpression", name, StringComparison.OrdinalIgnoreCase) && info.ParameterType.IsAssignableFrom(typeof(string)))
                {
                    flag = true;
                }
            }
            if (!isReturningQueryable && !flag)
            {
                throw new InvalidOperationException("ModelDataSourceView_InvalidSortingParameters");
            }
            return !flag;
        }

        private static void MergeDictionaries(IDictionary source, IDictionary destination)
        {
            if (source != null)
            {
                foreach (DictionaryEntry entry in source)
                {
                    object obj2 = entry.Value;
                    string key = (string)entry.Key;
                    destination[key] = obj2;
                }
            }
        }

        private Type ModelType
        {
            get
            {
                string modelTypeName = this.ModelTypeName;
                if (string.IsNullOrEmpty(modelTypeName))
                {
                    return null;
                }
                return BuildManager.GetType(modelTypeName, true, true);
            }
        }

        private static bool InheritsFromGenericTask(Type type, out Type genericTypeArgument)
        {
            var result = false;
            var baseType = type;
            genericTypeArgument = null;
            while (baseType != null && baseType != typeof(System.Object))
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    result = true;
                    genericTypeArgument = baseType.GenericTypeArguments[0];
                    break;
                }
                baseType = baseType.BaseType;
            }
            return result;
        }
    }
}