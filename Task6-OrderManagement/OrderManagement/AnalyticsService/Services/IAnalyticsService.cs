using AnalyticsService.DTOs;

namespace AnalyticsService.Services
{
    public interface IAnalyticsService
    {
        Task<AnalyticsSummaryDto> GetSummaryAsync();
    }
}
