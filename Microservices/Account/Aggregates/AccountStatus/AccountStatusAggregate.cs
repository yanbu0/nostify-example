using nostify;

namespace Account_Service;

public class AccountStatus : AccountStatusBaseClass, IAggregate
{
    public AccountStatus()
    {
    }

    public bool isDeleted { get; set; } = false;

    public static string aggregateType => "AccountStatus";
    public static string currentStateContainerName => $"{aggregateType}CurrentState";

    [ApplyEvents("Create_AccountStatus", "BulkCreate_AccountStatus", "Update_AccountStatus")]
    protected void ApplyCreateOrUpdate(IEvent eventToApply)
    {
        this.UpdateProperties<AccountStatus>(eventToApply.payload);
    }

    [ApplyEvents("Delete_AccountStatus")]
    protected void ApplyDelete(IEvent eventToApply)
    {
        this.isDeleted = true;
    }
}


