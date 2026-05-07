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

public class Update_Account_Command_Should
{
    private Mock<INostify> _nostifyMock;
    private UpdateAccount _func;
    private Mock<HttpClient> _httpClientMock;
    private Mock<ILogger<UpdateAccount>> _loggerMock;
    private Mock<FunctionContext> _functionContextMock;

    public Update_Account_Command_Should()
    {
        _nostifyMock = new Mock<INostify>();
        _httpClientMock = new Mock<HttpClient>();
        _loggerMock = new Mock<ILogger<UpdateAccount>>();
        _func = new UpdateAccount(_httpClientMock.Object, _nostifyMock.Object, _loggerMock.Object);
        _functionContextMock = new Mock<FunctionContext>();
    }

    [Fact]
    public async Task Insert_Update_Event()
    {
        //Arrange
        Guid newId = Guid.NewGuid();
        object updateAccount = new {
            id = newId
        };
        Account test = new Account();
        HttpRequestData testReq = MockHttpRequestData.Create(updateAccount);

        // Act
        var resp = await _func.Run(testReq, _functionContextMock.Object, newId);

        // Assert
        Assert.True(newId == resp);
    }


}
