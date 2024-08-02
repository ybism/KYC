namespace KYC360.Helpers
{
    public static class RetryHelper
    {
        public static void ExecuteWithRetry(Action action, int maxRetryAttempts, int delayMilliseconds)
        {
            int retryAttempts = 0;
            while (true)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    retryAttempts++;
                    if (retryAttempts >= maxRetryAttempts)
                    {
                        throw;
                    }

                    int delay = (int)Math.Pow(2, retryAttempts) * delayMilliseconds;
                    Console.WriteLine(
                        $"Retry attempt {retryAttempts} failed due to {ex.Message}. Waiting {delay}ms before next retry.");
                    Thread.Sleep(delay);
                }
            }
        }
    }
}