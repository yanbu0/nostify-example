using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;
using nostify;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service.Tests;

public class Create_Account_Command_Should
{
    private Mock<INostify> _nostifyMock;
    private CreateAccount _func;
    private Mock<HttpClient> _httpClientMock;
    private Mock<ILogger> _loggerMock;
    private Mock<HttpRequestData> _httpReqMock;

    public Create_Account_Command_Should()
    {
        _nostifyMock = new Mock<INostify>();
        _httpClientMock = new Mock<HttpClient>();
        _func = new CreateAccount(_httpClientMock.Object, _nostifyMock.Object);
        _loggerMock = new Mock<ILogger>();
        _httpReqMock = new Mock<HttpRequestData>();
    }

    [Fact]
    public async Task Insert_Create_Event()
    {
        //Arrange
        Account test = new Account();
        HttpRequestData testReq = MockHttpRequestData.Create(test);
        
        // Act
        var resp = await _func.Run(testReq, _loggerMock.Object);

        // Assert
        Assert.True(resp != Guid.Empty);
    }


}
