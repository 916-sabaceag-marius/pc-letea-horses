namespace Honse.Global.Extensions
{
    public static class ActionExtensions
    {
        public static async Task<TryCatchResult<TResult>> WithTryCatch<TResult>(this Task<TResult> task)
        {
            try
            {
                TResult result = await task;

                return new TryCatchResult<TResult> { Result = result };
            }
            catch (Exception ex)
            {
                return new TryCatchResult<TResult> { Exception = ex };
            }
        }

        public static async Task<TryCatchResult> WithTryCatch(this Task task)
        {
            try
            {
                await task;

                return new TryCatchResult();
            }
            catch (Exception ex)
            {
                return new TryCatchResult { Exception = ex };
            }
        }
    }

    public class TryCatchResult
    {
        public Exception? Exception { get; set; }

        public bool IsSuccessfull => Exception == null;
    }

    public class TryCatchResult<TResult> : TryCatchResult
    {
        public TResult? Result { get; set; }
    }
}
