namespace BlazorServerFirstProject.Services
{
    public class RateLimiter
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static DateTime _lastRequest = DateTime.MinValue;

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, int delayMs = 500)
        {
            await _semaphore.WaitAsync();
            try
            {
                var now = DateTime.UtcNow;
                var elapsed = (int)(now - _lastRequest).TotalMilliseconds;
                if (elapsed < delayMs)
                    await Task.Delay(Math.Max(0, delayMs - elapsed));

                var result = await action();
                _lastRequest = DateTime.UtcNow;
                return result;
            }
            finally
            {
                _semaphore.Release();
            }
        }

    }
}
