using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using nostify;

[assembly: FunctionsStartup(typeof(BankAccount_Service.Startup))]

namespace BankAccount_Service
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            string apiKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            string dbName = "BankAccount_DB";
            string endPoint = "https://localhost:8081";
            var nostify = new Nostify(apiKey,dbName,endPoint);
            
            _ = nostify.GetPersistedEventsContainerAsync().GetAwaiter().GetResult();
            _ = nostify.GetProjectionContainerAsync(BankAccountDetails.containerName).GetAwaiter().GetResult();

            builder.Services.AddSingleton<Nostify>((s) => {
                return nostify;
            });
        }
    }
}