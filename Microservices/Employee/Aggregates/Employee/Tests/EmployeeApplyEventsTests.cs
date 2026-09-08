using nostify;
using Xunit;

namespace Employee_Service.Tests;

public class EmployeeApplyEventsTests
{
    [Fact]
    public void Apply_Create_Event_Updates_Properties()
    {
        var employee = new Employee();
        var createEvent = new TestEvent(
            EmployeeCommand.Create,
            new
            {
                name = "John Smith"
            });

        employee.Apply(createEvent);

        Assert.Equal("John Smith", employee.name);
        Assert.False(employee.isDeleted);
    }

    [Fact]
    public void Apply_Delete_Event_Sets_IsDeleted()
    {
        var employee = new Employee();
        var deleteEvent = new TestEvent(EmployeeCommand.Delete, new { });

        employee.Apply(deleteEvent);

        Assert.True(employee.isDeleted);
    }

    private sealed class TestEvent : Event
    {
        public TestEvent(EventType eventType, object payload)
        {
            id = Guid.NewGuid();
            aggregateRootId = Guid.NewGuid();
            this.eventType = eventType;
            this.payload = payload;
        }
    }
}
