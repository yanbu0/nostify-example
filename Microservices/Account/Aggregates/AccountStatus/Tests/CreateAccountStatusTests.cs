using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using nostify;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service_Service.Tests;

public class Create_AccountStatus_Command_Should
{
    private Mock<INostify> _nostifyMock;
    private CreateAccountStatus _func;
    private Mock<HttpClient> _httpClientMock;
    private Mock<ILogger> _loggerMock;
    private Mock<HttpRequestData> _httpReqMock;

    public Create_AccountStatus_Command_Should()
    {
        _nostifyMock = new Mock<INostify>();
        _httpClientMock = new Mock<HttpClient>();
        _func = new CreateAccountStatus(_httpClientMock.Object, _nostifyMock.Object);
        _loggerMock = new Mock<ILogger>();
        _httpReqMock = new Mock<HttpRequestData>();
    }

    [Fact]
    public async Task Insert_Create_Event()
    {
        //Arrange
        AccountStatus test = new AccountStatus();
        HttpRequestData testReq = MockHttpRequestData.Create(test);
        
        // Act
        var resp = await _func.Run(testReq, _loggerMock.Object);

        // Assert
        Assert.True(resp != Guid.Empty);
    }


}
