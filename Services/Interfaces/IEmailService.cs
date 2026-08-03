namespace WebApplication2.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOfflinePeopleReportAsync(
            string reportFilePath,
            int offlinePeopleCount,
            CancellationToken cancellationToken =
                default);
    }
}