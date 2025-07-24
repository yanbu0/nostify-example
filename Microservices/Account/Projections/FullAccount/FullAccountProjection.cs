

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

    public override void Apply(Event eventToApply)
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
        // Get events from the same service for statusId property
        Container sameServiceEventStore = await nostify.GetEventStoreContainerAsync();
        
        //Use GetEventsAsync to get events from the same service for statusId
        List<ExternalDataEvent> externalDataEvents = await ExternalDataEvent.GetEventsAsync(sameServiceEventStore, 
            projectionsToInit, 
            p => p.statusId);

        // Get external data for Employee service for accountManagerId
        if (httpClient != null)
        {
            //Query the Employee service for accountManagerId
            var externalEventsFromEmployeeService = await ExternalDataEvent.GetEventsAsync(httpClient,
                "http://localhost:7072/api/EventRequest",
                projectionsToInit,
                p => p.accountManagerId
            );
            externalDataEvents.AddRange(externalEventsFromEmployeeService);
        }

        return externalDataEvents;
    }
}