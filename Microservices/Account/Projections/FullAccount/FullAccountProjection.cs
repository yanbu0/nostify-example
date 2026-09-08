

using System.Net.Http.Json;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using nostify;

namespace Account_Service;

public class FullAccount : AccountBaseClass, IProjection, IHasExternalData<FullAccount>
{
    public FullAccount()
    {
        
    }   

    public static string containerName => "FullAccount";

    public bool initialized { get; set; } = false;

    public bool isDeleted { get; set; }

    // Properties from requirements
    public string statusName { get; set; }
    public string accountManagerName { get; set; }

    [ApplyEvents("Create_Account", "BulkCreate_Account", "Update_Account")]
    protected void ApplyAccountCreateOrUpdate(IEvent eventToApply)
    {
        this.UpdateProperties<FullAccount>(eventToApply.payload);
    }

    [ApplyEvents("Delete_Account")]
    protected void ApplyAccountDelete(IEvent eventToApply)
    {
        this.isDeleted = true;
        this.ttl = 1;
    }

    [ApplyEvents("Create_AccountStatus", "Update_AccountStatus")]
    protected void ApplyAccountStatusCreateOrUpdate(IEvent eventToApply)
    {
        var statusMapping = new Dictionary<string, string>
        {
            { "name", "statusName" }
        };
        this.UpdateProperties<FullAccount>(eventToApply.payload, statusMapping);
    }

    [ApplyEvents("Create_Employee", "Update_Employee")]
    protected void ApplyEmployeeCreateOrUpdate(IEvent eventToApply)
    {
        var employeeMapping = new Dictionary<string, string>
        {
            { "name", "accountManagerName" }
        };
        this.UpdateProperties<FullAccount>(eventToApply.payload, employeeMapping);
    }

    public async static Task<List<ExternalDataEvent>> GetExternalDataEventsAsync(List<FullAccount> projectionsToInit, INostify nostify, HttpClient? httpClient = null, DateTime? pointInTime = null)
    {
        var grpcAddress = Environment.GetEnvironmentVariable("GrpcEventRequestAddress");
        var authToken = Environment.GetEnvironmentVariable("GrpcEventRequestAuthToken");
        var employeeServiceName = Environment.GetEnvironmentVariable("GrpcEmployeeServiceName") ?? "Employee";

        var factory = new ExternalDataEventFactory<FullAccount>(
                nostify,
                projectionsToInit,
                httpClient,
                pointInTime)
            // Get events from same service for statusId (nullable selector example)
            .WithSameServiceIdSelectors(p => p.statusId);

        // Use gRPC gateway when configured; otherwise fall back to HTTP EventRequest endpoint.
        if (!string.IsNullOrWhiteSpace(grpcAddress))
        {
            factory = factory.WithGrpcEventRequestor(grpcAddress, serviceName: employeeServiceName, authToken: authToken, p => p.accountManagerId);
        }
        else if (httpClient != null)
        {
            factory = factory.WithEventRequestor("http://localhost:7072/api/EventRequest", p => p.accountManagerId);
        }

        return await factory.GetEventsAsync();
    }
}