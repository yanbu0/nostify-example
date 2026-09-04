

using nostify;

namespace Account_Service;

public abstract class AccountStatusCommand : EventType
{
    ///<summary>
    ///Base Create Command
    ///</summary>
    public static Create_AccountStatus Create => Create_AccountStatus.Instance;
    ///<summary>
    ///Base Update Command
    ///</summary>
    public static Update_AccountStatus Update => Update_AccountStatus.Instance;
    ///<summary>
    ///Base Delete Command
    ///</summary>
    public static Delete_AccountStatus Delete => Delete_AccountStatus.Instance;
    ///<summary>
    ///Bulk Create Command
    ///</summary>
    public static BulkCreate_AccountStatus BulkCreate => BulkCreate_AccountStatus.Instance;


    protected AccountStatusCommand(string name, bool isNew = false)
    : base(name, isNew)
    {

    }
}

public sealed class Create_AccountStatus : AccountStatusCommand
{
    public static readonly Create_AccountStatus Instance = new Create_AccountStatus();

    private Create_AccountStatus() : base("Create_AccountStatus", true)
    {
    }
}

public sealed class Update_AccountStatus : AccountStatusCommand
{
    public static readonly Update_AccountStatus Instance = new Update_AccountStatus();

    private Update_AccountStatus() : base("Update_AccountStatus")
    {
    }
}

public sealed class Delete_AccountStatus : AccountStatusCommand
{
    public static readonly Delete_AccountStatus Instance = new Delete_AccountStatus();

    private Delete_AccountStatus() : base("Delete_AccountStatus")
    {
    }
}

public sealed class BulkCreate_AccountStatus : AccountStatusCommand
{
    public static readonly BulkCreate_AccountStatus Instance = new BulkCreate_AccountStatus();

    private BulkCreate_AccountStatus() : base("BulkCreate_AccountStatus", true)
    {
    }
}