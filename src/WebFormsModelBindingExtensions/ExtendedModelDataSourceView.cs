using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebFormsModelBindingExtensions
{
    public class ExtendedModelDataSourceView : ModelDataSourceView
    {
        private readonly ExtendedModelDataSource _owner;

        public ExtendedModelDataSourceView(ExtendedModelDataSource owner)
            : base(owner)
        {
            _owner = owner;
        }

        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            // TODO: Make this work
            return Execute<IEnumerable>(DataSourceOperation.Select, null);
        }

        protected override int ExecuteInsert(IDictionary values)
        {
            IDictionary paramaters = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            MergeDictionaries(values, paramaters);
            return Execute<int>(DataSourceOperation.Insert, paramaters);
        }

        protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            IDictionary paramaters = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            MergeDictionaries(oldValues, paramaters);
            MergeDictionaries(keys, paramaters);
            MergeDictionaries(values, paramaters);
            return Execute<int>(DataSourceOperation.Update, paramaters);
        }

        protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
        {
            IDictionary paramaters = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            MergeDictionaries(keys, paramaters);
            MergeDictionaries(oldValues, paramaters);
            return Execute<int>(DataSourceOperation.Delete, paramaters);
        }

        public override void Select(DataSourceSelectArguments arguments, DataSourceViewSelectCallback callback)
        {
            var method = FindMethod(SelectMethod);

            // Check if the method returns a Task<>
            Type taskReturnType;
            if (_owner.IsAsync && InheritsFromGenericTask(method.MethodInfo.ReturnType, out taskReturnType))
            {
                // Method returns a Task<>, we can run it async!
                SelectAsync(arguments, callback);
            }
            else
            {
                base.Select(arguments, callback);
            }
            // TODO: Detect if method returns a Task but page is not async and throw?
        }

        //public override void Insert(IDictionary values, DataSourceViewOperationCallback callback)
        //{
        //    var method = FindMethod(InsertMethod);
        //    var returnType = method.MethodInfo.ReturnType;
        //    // Method must return a Task<>
        //    if (typeof(Task).IsAssignableFrom(returnType) && _owner.IsAsync)
        //    {
        //        // Method returns a Task<>, we can run it!
        //        var page = _owner.DataControl.Page;
        //        var task = new PageAsyncTask(async () =>
        //        {
        //            var methodTask = GetInsertMethodResult(values) as Task;
        //            await methodTask;
        //            // Validate the returned result
        //            var result = ((dynamic)methodTask).Result;

        //        });
        //        page.RegisterAsyncTask(task);
        //    }
        //    else
        //    {
        //        base.Insert(values, callback);
        //    }
        //}

        //public override void Delete(IDictionary keys, IDictionary oldValues, DataSourceViewOperationCallback callback)
        //{
        //    var method = FindMethod(DeleteMethod);
        //    var returnType = method.MethodInfo.ReturnType;
        //    // Method must return a Task<>
        //    if (typeof(Task).IsAssignableFrom(returnType) && _owner.IsAsync)
        //    {
        //        // Method returns a Task<>, we can run it!
        //        var page = _owner.DataControl.Page;
        //        var task = new PageAsyncTask(async () =>
        //        {
        //            var methodTask = GetDeleteMethodResult(keys, oldValues) as Task;
        //            await methodTask;
        //            // Validate the returned result
        //            var result = ((dynamic)methodTask).Result;

        //        });
        //        page.RegisterAsyncTask(task);
        //    }
        //    else
        //    {
        //        base.Delete(keys, oldValues, callback);
        //    }
        //}

        //public override void Update(IDictionary keys, IDictionary values, IDictionary oldValues, DataSourceViewOperationCallback callback)
        //{
        //    var method = FindMethod(UpdateMethod);
        //    var returnType = method.MethodInfo.ReturnType;
        //    // Method must return a Task<>
        //    Type taskReturnType;
        //    if (_owner.IsAsync)
        //    {
        //        // Method returns a Task<>, we can run it!
        //        var page = _owner.DataControl.Page;
        //        var task = new PageAsyncTask(async () =>
        //        {
        //            var methodTask = GetUpdateMethodResult(keys, values, oldValues) as Task;
        //            await methodTask;
        //            // Validate the returned result
        //            var result = ((dynamic)methodTask).Result;

        //        });
        //        page.RegisterAsyncTask(task);
        //    }
        //    else
        //    {
        //        base.Update(keys, values, oldValues, callback);
        //    }
        //}

        private void SelectAsync(DataSourceSelectArguments arguments, DataSourceViewSelectCallback callback)
        {
            // Execute pre-method filters
            var method = FindMethod(SelectMethod);
            //var filters = GetDataMethodFilters(method);
            //MethodExecutingContext methodExecutingContext = ProcessDataMethodExecutingFilters(method, filters, DataSourceOperation.Select);

            var pageAsyncTask = new PageAsyncTask(async () =>
            {
                Exception selectException = null;
                IEnumerable selectResult = null;
                try
                {
                    var selectTask = GetSelectMethodResult(arguments) as Task;
                    await selectTask;

                    object result = ((dynamic)selectTask).Result;
                    selectResult = CreateSelectResult(result);
                }
                catch (Exception ex)
                {
                    selectException = ex;
                }

                // Excute post-method filters
                //MethodExecutedContext methodExecutedContext = ProcessDataMethodExecutedFilters(filters, DataSourceOperation.Select, selectException, selectResult);
                if (selectException != null //&& !methodExecutedContext.ExceptionHandled
                    )
                {
                    throw selectException;
                }

                //if (methodExecutedContext.Result != null && !(methodExecutedContext.Result is IEnumerable))
                //{
                //    throw new InvalidOperationException("Select method result must be an IEnumerable");
                //}

                //callback((IEnumerable)methodExecutedContext.Result);
                callback(selectResult);
            });

            _owner.DataControl.Page.RegisterAsyncTask(pageAsyncTask);
        }

        private TResult Execute<TResult>(DataSourceOperation operation, IDictionary parameters)
        {
            var method = FindMethod(operation);
            EvaluateMethodParameters(operation, method, parameters);
            var filters = GetDataMethodFilters(method);

            MethodExecutingContext methodExecutingContext = ProcessDataMethodExecutingFilters(method, filters, operation);
            if (methodExecutingContext.Result != null)
            {
                if (methodExecutingContext.Result is TResult)
                {
                    return (TResult)methodExecutingContext.Result;
                }
                throw new InvalidOperationException(String.Format("{0} method result must be of type {1}", operation, typeof(TResult).Name));
            }

            method.Parameters.Clear();
            foreach (var parameter in methodExecutingContext.Parameters)
            {
                method.Parameters.Add(parameter.Key, parameter.Value);
            }

            TResult result = default(TResult);
            Exception methodException = null;
            try
            {
                var returnValue = InvokeMethod(method).ReturnValue;
                result = returnValue != null ? (TResult)returnValue : default(TResult);
            }
            catch (Exception ex)
            {
                methodException = ex;
            }

            MethodExecutedContext methodExecutedContext = ProcessDataMethodExecutedFilters(filters, operation, methodException, result);

            if (methodException != null && !methodExecutedContext.ExceptionHandled)
            {
                throw methodException;
            }

            if (methodExecutedContext.Result != null && !(methodExecutedContext.Result is TResult))
            {
                throw new InvalidOperationException(String.Format("{0} method result must be of type {1}", operation, typeof(TResult).Name));
            }

            return (TResult)methodExecutedContext.Result;
        }

        private ModelDataSourceMethod FindMethod(DataSourceOperation operation)
        {
            switch (operation)
            {
                case DataSourceOperation.Insert:
                    return FindMethod(InsertMethod);
                case DataSourceOperation.Update:
                    return FindMethod(UpdateMethod);
                case DataSourceOperation.Delete:
                    return FindMethod(DeleteMethod);
                case DataSourceOperation.Select:
                default:
                    return FindMethod(SelectMethod);
            }
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

        private IEnumerable<IDataMethodFilter> GetDataMethodFilters(ModelDataSourceMethod method)
        {
            var filters = method.MethodInfo.GetCustomAttributes(typeof(IDataMethodFilter), false);
            return filters.Cast<IDataMethodFilter>() ?? Enumerable.Empty<IDataMethodFilter>();
        }

        private MethodExecutingContext ProcessDataMethodExecutingFilters(ModelDataSourceMethod method, IEnumerable<IDataMethodFilter> filters, DataSourceOperation operation)
        {
            var methodExecutingContext = new MethodExecutingContext { DataControl = _owner.DataControl, Operation = operation };
            foreach (DictionaryEntry entry in method.Parameters)
            {
                methodExecutingContext.Parameters.Add((string)entry.Key, entry.Value);
            }
            foreach (var filter in filters)
            {
                filter.OnMethodExecuting(methodExecutingContext);
            }
            return methodExecutingContext;
        }

        private MethodExecutedContext ProcessDataMethodExecutedFilters(IEnumerable<IDataMethodFilter> filters, DataSourceOperation operation, Exception exception, object result)
        {
            var methodExecutedContext = new MethodExecutedContext
            {
                DataControl = _owner.DataControl,
                Operation = operation,
                Exception = exception,
                ExceptionHandled = exception == null,
                Result = result
            };

            foreach (var filter in filters)
            {
                filter.OnMethodExecuted(methodExecutedContext);
            }
            return methodExecutedContext;
        }
    }
}
