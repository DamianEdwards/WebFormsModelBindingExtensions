using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.WebControls;
using AspNet.WebForms.ModelBinding.Extensions.Properties;

namespace AspNet.WebForms.ModelBinding.Extensions
{
    public class ExtendedModelDataSourceView : ModelDataSourceView
    {
        private readonly ExtendedModelDataSource _owner;
        private bool _isAsyncSelect = false;
        private bool _viewOperationInProgress = false;

        public ExtendedModelDataSourceView(ExtendedModelDataSource owner)
            : base(owner)
        {
            _owner = owner;
        }

        // Select call tree:
        // DataBoundControl.PerformSelect
        //   DataSourceView.Select
        //     ExecuteSelect
        //       GetSelectMethodResult
        //         EvaluateSelectMethodParamaters
        //         InvokeMethod
        //         ProcessSelectMethodResult
        //       CreateSelectResult

        public override void Select(DataSourceSelectArguments arguments, DataSourceViewSelectCallback callback)
        {
            if (_viewOperationInProgress)
            {
                // There appears to be a race somewhere in the model binding or data control base
                // so we have to ensure we don't honor any call to select if a view operation
                // (insert, update, delete) is still in progress as it results odd exceptions.
                return;
            }

            var method = FindMethod(SelectMethod);

            if (InheritsFromTask<SelectResult>(method.MethodInfo.ReturnType))
            {
                SelectAsync(arguments, callback);
            }
            else
            {
                base.Select(arguments, callback);
            }
        }

        public override void Insert(IDictionary values, DataSourceViewOperationCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (typeof(Task).IsAssignableFrom(FindMethod(InsertMethod).MethodInfo.ReturnType))
            {
                ViewOperationAsync(() => (Task)GetInsertMethodResult(values), callback);
            }
            else
            {
                base.Insert(values, callback);
            }
        }

        public override void Update(IDictionary keys, IDictionary values, IDictionary oldValues, DataSourceViewOperationCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (typeof(Task).IsAssignableFrom(FindMethod(UpdateMethod).MethodInfo.ReturnType))
            {
                ViewOperationAsync(() => (Task)GetUpdateMethodResult(keys, values, oldValues), callback);
            }
            else
            {
                base.Update(keys, values, oldValues, callback);
            }
        }

        public override void Delete(IDictionary keys, IDictionary oldValues, DataSourceViewOperationCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            _viewOperationInProgress = true;

            if (typeof(Task).IsAssignableFrom(FindMethod(DeleteMethod).MethodInfo.ReturnType))
            {
                ViewOperationAsync(() => (Task)GetDeleteMethodResult(keys, oldValues), callback);
            }
            else
            {
                base.Delete(keys, oldValues, callback);
            }
        }

        protected void SelectAsync(DataSourceSelectArguments arguments, DataSourceViewSelectCallback callback)
        {
            if (!_owner.DataControl.Page.IsAsync)
            {
                throw new InvalidOperationException(Resources.ExtendedModelDataSourceView_MustBeAsyncPage);
            }

            _isAsyncSelect = true;

            DataSourceSelectResultProcessingOptions selectResultProcessingOptions = null;
            ModelDataSourceMethod modelMethod = EvaluateSelectMethodParameters(arguments, out selectResultProcessingOptions);
            ModelDataMethodResult result = InvokeMethod(modelMethod);

            _owner.DataControl.Page.RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                var selectResult = await (Task<SelectResult>)result.ReturnValue;
                if (arguments.RetrieveTotalRowCount)
                {
                    if (!selectResult.TotalRowCount.HasValue)
                    {
                        throw new InvalidOperationException(Resources.ExtendedModelDataSourceView_TotalRowCountNotSet);
                    }
                    arguments.TotalRowCount = selectResult.TotalRowCount.Value;
                }

                _isAsyncSelect = false;

                callback(CreateSelectResult(selectResult.Results));
            }));
        }

        protected void ViewOperationAsync(Func<Task> asyncViewOperation, DataSourceViewOperationCallback callback)
        {
            if (!_owner.DataControl.Page.IsAsync)
            {
                throw new InvalidOperationException(Resources.ExtendedModelDataSourceView_MustBeAsyncPage);
            }

            _owner.DataControl.Page.RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                var operationTask = asyncViewOperation();
                var operationTaskInt = operationTask as Task<int>;
                var operationThrew = false;
                var affectedRecords = 0;
                try
                {
                    await operationTask;
                    if (operationTaskInt != null)
                    {
                        affectedRecords = operationTaskInt.Result;
                    }
                }
                catch (Exception ex)
                {
                    operationThrew = true;
                    if (!callback(affectedRecords, ex))
                    {
                        // Nobody handled the operation error so re-throw
                        throw;
                    }
                }
                finally
                {
                    _viewOperationInProgress = false;

                    if (!operationThrew)
                    {
                        // The following line is technically meant to be called, but this causes an exception deep in ListView for some reason,
                        // but it appears to automatically rebind anyway even without this call, so leaving commented out.
                        //if (_owner.DataControl.Page.ModelState.IsValid)
                        //{
                        //    OnDataSourceViewChanged(EventArgs.Empty);
                        //}

                        // Success
                        callback(affectedRecords, null);
                    }
                }
            }));
        }

        protected override ModelDataSourceMethod EvaluateSelectMethodParameters(DataSourceSelectArguments arguments, out DataSourceSelectResultProcessingOptions selectResultProcessingOptions)
        {
            if (!_isAsyncSelect)
            {
                // Not doing async so just delegate to the base
                return base.EvaluateSelectMethodParameters(arguments, out selectResultProcessingOptions);
            }

            IOrderedDictionary controlValues = MergeSelectParameters(arguments);
            ModelDataSourceMethod modelDataSourceMethod = FindMethod(this.SelectMethod);
            Type returnType = modelDataSourceMethod.MethodInfo.ReturnType;
            Type modelType = ModelType;
            if (modelType == null)
            {
                foreach (var genericTypeArg in returnType.GetGenericArguments())
                {
                    if (typeof(IQueryable<>).MakeGenericType(genericTypeArg).IsAssignableFrom(returnType))
                    {
                        modelType = genericTypeArg;
                    }
                }
            }
            var queryableModelType = (modelType != null) ? typeof(IQueryable<>).MakeGenericType(modelType) : null;
            var isReturningQueryable = (queryableModelType != null) && queryableModelType.IsAssignableFrom(returnType);
            var autoPage = false;
            var autoSort = false;
            if ((arguments.StartRowIndex >= 0) && (arguments.MaximumRows > 0))
            {
                autoPage = IsAutoPagingRequired(modelDataSourceMethod.MethodInfo, isReturningQueryable, _isAsyncSelect);
            }
            if (!String.IsNullOrEmpty(arguments.SortExpression))
            {
                autoSort = IsAutoSortingRequired(modelDataSourceMethod.MethodInfo, isReturningQueryable);
            }
            selectResultProcessingOptions = new DataSourceSelectResultProcessingOptions { ModelType = modelType, AutoPage = autoPage, AutoSort = autoSort };
            EvaluateMethodParameters(DataSourceOperation.Select, modelDataSourceMethod, controlValues);
            return modelDataSourceMethod;
        }

        private static IOrderedDictionary MergeSelectParameters(DataSourceSelectArguments arguments)
        {
            var hasPaging = (arguments.StartRowIndex >= 0) && (arguments.MaximumRows > 0);
            var hasSortExpression = !String.IsNullOrEmpty(arguments.SortExpression);
            IOrderedDictionary destination = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            if (hasSortExpression)
            {
                destination["sortByExpression"] = arguments.SortExpression;
            }
            if (hasPaging)
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
            var hasMaximumRowsParamater = false;
            var hasTotalRowCountOutParameter = false;
            var hasStartRowIndexParameter = false;

            foreach (ParameterInfo info in selectMethod.GetParameters())
            {
                var name = info.Name;
                if (String.Equals("startRowIndex", name, StringComparison.OrdinalIgnoreCase))
                {
                    if (info.ParameterType.IsAssignableFrom(typeof(Int32)))
                    {
                        hasStartRowIndexParameter = true;
                    }
                }
                else if (String.Equals("maximumRows", name, StringComparison.OrdinalIgnoreCase))
                {
                    if (info.ParameterType.IsAssignableFrom(typeof(Int32)))
                    {
                        hasMaximumRowsParamater = true;
                    }
                }
                else if ((String.Equals("totalRowCount", name, StringComparison.OrdinalIgnoreCase) && info.IsOut) && typeof(Int32).IsAssignableFrom(info.ParameterType.GetElementType()))
                {
                    hasTotalRowCountOutParameter = true;
                }
            }

            var hasAllPagingParameters = hasMaximumRowsParamater && hasStartRowIndexParameter && hasTotalRowCountOutParameter;

            if ((!isAsyncSelect && !isReturningQueryable && !hasAllPagingParameters)
                || (isAsyncSelect && !(hasStartRowIndexParameter && hasMaximumRowsParamater)))
            {
                throw new InvalidOperationException(SR.GetString("ModelDataSourceView_InvalidPagingParameters"));
            }

            return !hasAllPagingParameters;
        }

        private static bool IsAutoSortingRequired(MethodInfo selectMethod, bool isReturningQueryable)
        {
            bool hasSortByExpressionParameter = false;
            foreach (ParameterInfo info in selectMethod.GetParameters())
            {
                string name = info.Name;
                if (string.Equals("sortByExpression", name, StringComparison.OrdinalIgnoreCase) && info.ParameterType.IsAssignableFrom(typeof(string)))
                {
                    hasSortByExpressionParameter = true;
                }
            }
            if (!isReturningQueryable && !hasSortByExpressionParameter)
            {
                throw new InvalidOperationException(SR.GetString("ModelDataSourceView_InvalidSortingParameters"));
            }
            return !hasSortByExpressionParameter;
        }

        private static void MergeDictionaries(IDictionary source, IDictionary destination)
        {
            if (source != null)
            {
                foreach (DictionaryEntry entry in source)
                {
                    var key = (string)entry.Key;
                    var value = entry.Value;
                    destination[key] = value;
                }
            }
        }

        private Type ModelType
        {
            get
            {
                var modelTypeName = ModelTypeName;
                if (String.IsNullOrEmpty(modelTypeName))
                {
                    return null;
                }
                return BuildManager.GetType(modelTypeName, true, true);
            }
        }

        private static bool InheritsFromTask<T>(Type type)
        {
            Type genericTypeArgument;
            return InheritsFromGenericTask(type, out genericTypeArgument) && typeof(T).IsAssignableFrom(genericTypeArgument);
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