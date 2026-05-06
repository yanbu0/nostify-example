using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;
using nostify;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace Account_Service.Tests;

public class Delete_Account_Command_Should
{
    private Mock<INostify> _nostifyMock;
    private DeleteAccount _func;
    private Mock<HttpClient> _httpClientMock;
    private Mock<ILogger<DeleteAccount>> _loggerMock;
    private Mock<FunctionContext> _functionContextMock;

    public Delete_Account_Command_Should()
    {
        _nostifyMock = new Mock<INostify>();
        _httpClientMock = new Mock<HttpClient>();
        _loggerMock = new Mock<ILogger<DeleteAccount>>();
        _func = new DeleteAccount(_httpClientMock.Object, _nostifyMock.Object, _loggerMock.Object);
        _functionContextMock = new Mock<FunctionContext>();
    }

    [Fact]
    public async Task Insert_Delete_Event()
    {
        //Arrange
        Account test = new Account();
        HttpRequestData testReq = MockHttpRequestData.Create();
        Guid newId = Guid.NewGuid();

        // Act
        var resp = await _func.Run(testReq, _functionContextMock.Object, newId);

        // Assert
        Assert.True(newId == resp);
    }


}
