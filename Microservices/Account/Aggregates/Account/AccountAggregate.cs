using nostify;


namespace Account_Service;

public class Account : AccountBaseClass, IAggregate
{
    public Account()
    {
    }

    public bool isDeleted { get; set; } = false;

    public static string aggregateType => "Account";
    public static string currentStateContainerName => $"{aggregateType}CurrentState";

    [ApplyEvents("Create_Account", "BulkCreate_Account", "Update_Account")]
    protected void ApplyCreateOrUpdate(IEvent eventToApply)
    {
        //Note: this uses reflection, may be desirable to optimize
        this.UpdateProperties<Account>(eventToApply.payload);
    }

    [ApplyEvents("Delete_Account")]
    protected void ApplyDelete(IEvent eventToApply)
    {
        this.isDeleted = true;
    }
}


