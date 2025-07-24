using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using nostify;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service.Tests;

public class Update_AccountStatus_Command_Should
{
    private Mock<INostify> _nostifyMock;
    private UpdateAccountStatus _func;
    private Mock<HttpClient> _httpClientMock;
    private Mock<ILogger> _loggerMock;

    public Update_AccountStatus_Command_Should()
    {
        _nostifyMock = new Mock<INostify>();
        _httpClientMock = new Mock<HttpClient>();
        _func = new UpdateAccountStatus(_httpClientMock.Object, _nostifyMock.Object);
        _loggerMock = new Mock<ILogger>();
    }

    [Fact]
    public async Task Insert_Update_Event()
    {
        //Arrange
        Guid newId = Guid.NewGuid();
        object updateAccountStatus = new {
            id = newId
        };
        AccountStatus test = new AccountStatus();
        HttpRequestData testReq = MockHttpRequestData.Create(updateAccountStatus);

        // Act
        var resp = await _func.Run(testReq, _loggerMock.Object);

        // Assert
        Assert.True(newId == resp);
    }


}
