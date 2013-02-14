
namespace WebFormsModelBindingExtensions
{
    public interface IDataMethodFilter
    {
        void OnMethodExecuting(MethodExecutingContext methodExecutingContext);
        void OnMethodExecuted(MethodExecutedContext methodExecutedContext);
    }
}
