using nostify;
using Xunit;

namespace Account_Service.Tests;

public class AccountApplyEventsTests
{
    [Fact]
    public void Apply_Create_Event_Updates_Properties()
    {
        var account = new Account();
        var statusId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var createEvent = new TestEvent(
            AccountCommand.Create,
            new
            {
                name = "Primary Account",
                statusId,
                accountManagerId = managerId
            });

        account.Apply(createEvent);

        Assert.Equal("Primary Account", account.name);
        Assert.Equal(statusId, account.statusId);
        Assert.Equal(managerId, account.accountManagerId);
        Assert.False(account.isDeleted);
    }

    [Fact]
    public void Apply_Delete_Event_Sets_IsDeleted()
    {
        var account = new Account();
        var deleteEvent = new TestEvent(AccountCommand.Delete, new { });

        account.Apply(deleteEvent);

        Assert.True(account.isDeleted);
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
