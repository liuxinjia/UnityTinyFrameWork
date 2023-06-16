namespace Cr7Sund.Async
{
    public abstract class AsyncAwaitable<TAwaiter> : Async, IAsyncAwaitable<TAwaiter> where TAwaiter : IAwaiter, new()
    {
        protected TAwaiter awaiter;
        public TAwaiter GetAwaiter()
        {
            if (awaiter == null)
            {
                awaiter = new TAwaiter();
                if (IsCompleted)
                {
                    awaiter.Completed(this);
                }
            }
            return awaiter;
        }

        public override void Completed()
        {
            //先自己的回调处理完，再处理awaiter
            base.Completed();
            awaiter?.Completed(this);
        }
    }

    public class AsyncAwaitable : AsyncAwaitable<Awaiter> { }
}