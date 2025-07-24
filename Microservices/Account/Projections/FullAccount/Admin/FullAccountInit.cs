
using Account_Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using nostify;

namespace AccountService;

public class FullAccountInit
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public FullAccountInit(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(FullAccountInit))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "FullAccountInit")] HttpRequestData req,
        ILogger log)
    {
        await _nostify.InitContainerAsync<FullAccount,Account>();
        return new OkResult();
    }
}