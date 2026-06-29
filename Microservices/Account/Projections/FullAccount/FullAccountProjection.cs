

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

    public override void Apply(IEvent eventToApply)
    {
        // Handle Account events
        if (eventToApply.command.name.Equals("Create_Account") 
                || eventToApply.command.name.Equals("Update_Account"))
        {
            this.UpdateProperties<FullAccount>(eventToApply.payload);
        }
        else if (eventToApply.command.name.Equals("Delete_Account"))
        {
            this.isDeleted = true;
            this.ttl = 1;
        }
        // Handle AccountStatus events - map name to statusName
        else if (eventToApply.command.name.Equals("Create_AccountStatus") || 
                 eventToApply.command.name.Equals("Update_AccountStatus"))
        {
            var statusMapping = new Dictionary<string, string>
            {
                { "name", "statusName" }
            };
            this.UpdateProperties<FullAccount>(eventToApply.payload, statusMapping);
        }
        // Handle Employee events - map name to accountManagerName
        else if (eventToApply.command.name.Equals("Create_Employee") || 
                 eventToApply.command.name.Equals("Update_Employee"))
        {
            var employeeMapping = new Dictionary<string, string>
            {
                { "name", "accountManagerName" }
            };
            this.UpdateProperties<FullAccount>(eventToApply.payload, employeeMapping);
        }
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
            factory = string.IsNullOrWhiteSpace(authToken)
                ? factory.WithGrpcEventRequestor(grpcAddress, employeeServiceName, p => p.accountManagerId)
                : factory.WithGrpcEventRequestor(grpcAddress, serviceName: employeeServiceName, authToken: authToken, p => p.accountManagerId);
        }
        else if (httpClient != null)
        {
            factory = factory.WithEventRequestor("http://localhost:7072/api/EventRequest", p => p.accountManagerId);
        }

        return await factory.GetEventsAsync();
    }
}