using nostify;
using Xunit;

namespace Account_Service.Tests;

public class FullAccountApplyEventsTests
{
    [Fact]
    public void Apply_Account_Events_Update_Projection()
    {
        var projection = new FullAccount();
        var statusId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var createEvent = new TestEvent(
            AccountCommand.Create,
            new
            {
                name = "Checking",
                statusId,
                accountManagerId = managerId
            });

        projection.Apply(createEvent);

        Assert.Equal("Checking", projection.name);
        Assert.Equal(statusId, projection.statusId);
        Assert.Equal(managerId, projection.accountManagerId);
        Assert.False(projection.isDeleted);
    }

    [Fact]
    public void Apply_Delete_Account_Event_Sets_Soft_Delete_Flags()
    {
        var projection = new FullAccount();
        var deleteEvent = new TestEvent(AccountCommand.Delete, new { });

        projection.Apply(deleteEvent);

        Assert.True(projection.isDeleted);
        Assert.Equal(1, projection.ttl);
    }

    [Fact]
    public void Apply_Related_Service_Events_Map_Names()
    {
        var projection = new FullAccount();
        var statusEvent = new TestEvent(
            AccountStatusCommand.Update,
            new
            {
                name = "Pending Review"
            });
        var employeeEvent = new TestEvent(
            Update_Employee_TestEventType.Instance,
            new
            {
                name = "Jane Doe"
            });

        projection.Apply(statusEvent);
        projection.Apply(employeeEvent);

        Assert.Equal("Pending Review", projection.statusName);
        Assert.Equal("Jane Doe", projection.accountManagerName);
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

    private sealed class Update_Employee_TestEventType : EventType
    {
        public static readonly Update_Employee_TestEventType Instance = new();

        private Update_Employee_TestEventType() : base("Update_Employee")
        {
        }
    }
}
