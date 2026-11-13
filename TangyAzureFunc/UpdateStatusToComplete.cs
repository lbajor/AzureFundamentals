using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TangyAzureFunc.Data;
using TangyAzureFunc.Models;

namespace TangyAzureFunc;

public class UpdateStatusToComplete
{
    private readonly ILogger _logger;
    private ApplicationDbContext _dbContext;

    public UpdateStatusToComplete(ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
    {
        _logger = loggerFactory.CreateLogger<UpdateStatusToComplete>();
        _dbContext = dbContext;
    }

    [Function("UpdateStatusToComplete")]
    public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

        IEnumerable<SalesRequest> salesRequests = _dbContext.SalesRequests.Where(sr => sr.Status == "Image Resized").ToList();

        foreach (var salesRequest in salesRequests)
        {
            salesRequest.Status = "Completed";
        }

        _dbContext.SaveChanges();

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}