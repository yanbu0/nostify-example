using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using nostify;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service_Service.Tests;

public class Delete_AccountStatus_Command_Should
{
    private Mock<INostify> _nostifyMock;
    private DeleteAccountStatus _func;
    private Mock<HttpClient> _httpClientMock;
    private Mock<ILogger> _loggerMock;

    public Delete_AccountStatus_Command_Should()
    {
        _nostifyMock = new Mock<INostify>();
        _httpClientMock = new Mock<HttpClient>();
        _func = new DeleteAccountStatus(_httpClientMock.Object, _nostifyMock.Object);
        _loggerMock = new Mock<ILogger>();
    }

    [Fact]
    public async Task Insert_Delete_Event()
    {
        //Arrange
        AccountStatus test = new AccountStatus();
        HttpRequestData testReq = MockHttpRequestData.Create();
        Guid newId = Guid.NewGuid();

        // Act
        var resp = await _func.Run(testReq, newId, _loggerMock.Object);

        // Assert
        Assert.True(newId == resp);
    }


}
