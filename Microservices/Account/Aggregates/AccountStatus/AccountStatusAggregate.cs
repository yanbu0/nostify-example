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

    public override void Apply(Event eventToApply)
    {
        if (eventToApply.command == AccountStatusCommand.BulkCreate || eventToApply.command == AccountStatusCommand.Create || eventToApply.command == AccountStatusCommand.Update)
        {
            this.UpdateProperties<AccountStatus>(eventToApply.payload);
        }
        else if (eventToApply.command == AccountStatusCommand.Delete)
        {
            this.isDeleted = true;
        }
    }
}



