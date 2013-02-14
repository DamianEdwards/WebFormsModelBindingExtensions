
namespace AspNet.WebForms.ModelBinding.Extensions
{
    public interface IDataMethodFilter
    {
        void OnMethodExecuting(MethodExecutingContext methodExecutingContext);
        void OnMethodExecuted(MethodExecutedContext methodExecutedContext);
    }
}
