namespace Common.Libraries.Services.BackgroundJobService.TestApi
{
    public class ReportService
    {
        public async Task GenerateDailyReport(CancellationToken token)
        {
            Console.WriteLine("Starting report generation...");

            for (int i = 0; i < 5; i++)
            {
                token.ThrowIfCancellationRequested();
                Console.WriteLine($"Processing part {i + 1}...");
                await Task.Delay(1000, token);
            }

            Console.WriteLine("Report generation completed.");
        }
    }
}
