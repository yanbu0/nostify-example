using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using nostify;

[assembly: FunctionsStartup(typeof(AccountManager_Service.Startup))]

namespace AccountManager_Service
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<Nostify>((s) => {
                string apiKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                string dbName = "AccountManager_DB";
                string endPoint = "https://localhost:8081";
                var nostify = new Nostify(apiKey,dbName,endPoint);
                return nostify;
            });
        }
    }
}