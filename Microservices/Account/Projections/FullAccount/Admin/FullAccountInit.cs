
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
    private readonly ILogger<FullAccountInit> _logger;
    public FullAccountInit(HttpClient httpClient, INostify nostify, ILogger<FullAccountInit> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(FullAccountInit))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "FullAccountInit")] HttpRequestData req)
    {
        await _nostify.InitContainerAsync<FullAccount,Account>();
        return new OkResult();
    }
}