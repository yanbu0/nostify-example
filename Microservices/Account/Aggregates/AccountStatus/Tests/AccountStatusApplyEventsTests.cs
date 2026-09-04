using nostify;
using Xunit;

namespace Account_Service.Tests;

public class AccountStatusApplyEventsTests
{
    [Fact]
    public void Apply_Update_Event_Updates_Properties()
    {
        var accountStatus = new AccountStatus();
        var updateEvent = new TestEvent(
            AccountStatusCommand.Update,
            new
            {
                name = "Active"
            });

        accountStatus.Apply(updateEvent);

        Assert.Equal("Active", accountStatus.name);
        Assert.False(accountStatus.isDeleted);
    }

    [Fact]
    public void Apply_Delete_Event_Sets_IsDeleted()
    {
        var accountStatus = new AccountStatus();
        var deleteEvent = new TestEvent(AccountStatusCommand.Delete, new { });

        accountStatus.Apply(deleteEvent);

        Assert.True(accountStatus.isDeleted);
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
