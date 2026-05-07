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

namespace Employee_Service.Tests;

public class Update_Employee_Command_Should
{
    private Mock<INostify> _nostifyMock;
    private UpdateEmployee _func;
    private Mock<HttpClient> _httpClientMock;
    private Mock<ILogger<UpdateEmployee>> _loggerMock;
    private Mock<FunctionContext> _functionContextMock;

    public Update_Employee_Command_Should()
    {
        _nostifyMock = new Mock<INostify>();
        _httpClientMock = new Mock<HttpClient>();
        _loggerMock = new Mock<ILogger<UpdateEmployee>>();
        _func = new UpdateEmployee(_httpClientMock.Object, _nostifyMock.Object, _loggerMock.Object);
        _functionContextMock = new Mock<FunctionContext>();
    }

    [Fact]
    public async Task Insert_Update_Event()
    {
        //Arrange
        Guid newId = Guid.NewGuid();
        object updateEmployee = new {
            id = newId
        };
        Employee test = new Employee();
        HttpRequestData testReq = MockHttpRequestData.Create(updateEmployee);

        // Act
        var resp = await _func.Run(testReq, _functionContextMock.Object, newId);

        // Assert
        Assert.True(newId == resp);
    }


}
