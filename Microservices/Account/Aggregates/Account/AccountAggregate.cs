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

    public override void Apply(IEvent eventToApply)
    {
        if (eventToApply.command == AccountCommand.Create || eventToApply.command == AccountCommand.BulkCreate || eventToApply.command == AccountCommand.Update)
        {
            //Note: this uses reflection, may be desirable to optimize
            this.UpdateProperties<Account>(eventToApply.payload);
        }
        else if (eventToApply.command == AccountCommand.Delete)
        {
            this.isDeleted = true;
        }
    }
}



