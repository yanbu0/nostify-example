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

public class Create_Account_Command_Should
{
    private Mock<INostify> _nostifyMock;
    private CreateAccount _func;
    private Mock<HttpClient> _httpClientMock;
    private Mock<ILogger<CreateAccount>> _loggerMock;
    private Mock<HttpRequestData> _httpReqMock;
    private Mock<FunctionContext> _functionContextMock;

    public Create_Account_Command_Should()
    {
        _nostifyMock = new Mock<INostify>();
        _httpClientMock = new Mock<HttpClient>();
        _loggerMock = new Mock<ILogger<CreateAccount>>();
        _func = new CreateAccount(_httpClientMock.Object, _nostifyMock.Object, _loggerMock.Object);
        _httpReqMock = new Mock<HttpRequestData>();
        _functionContextMock = new Mock<FunctionContext>();
    }

    [Fact]
    public async Task Insert_Create_Event()
    {
        //Arrange
        Account test = new Account();
        HttpRequestData testReq = MockHttpRequestData.Create(test);
        
        // Act
        var resp = await _func.Run(testReq, _functionContextMock.Object);

        // Assert
        Assert.True(resp != Guid.Empty);
    }


}
