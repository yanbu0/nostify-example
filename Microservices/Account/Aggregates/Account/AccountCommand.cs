

using nostify;

namespace Account_Service;

public abstract class AccountCommand : EventType
{
    ///<summary>
    ///Base Create Command
    ///</summary>
    public static Create_Account Create => Create_Account.Instance;
    ///<summary>
    ///Base Update Command
    ///</summary>
    public static Update_Account Update => Update_Account.Instance;
    ///<summary>
    ///Base Delete Command
    ///</summary>
    public static Delete_Account Delete => Delete_Account.Instance;
    ///<summary>
    ///Bulk Create Command
    ///</summary>
    public static BulkCreate_Account BulkCreate => BulkCreate_Account.Instance;

    protected AccountCommand(string name, bool isNew = false)
    : base(name, isNew)
    {

    }
}

public sealed class Create_Account : AccountCommand
{
    public static readonly Create_Account Instance = new Create_Account();

    private Create_Account() : base("Create_Account", true)
    {
    }
}

public sealed class Update_Account : AccountCommand
{
    public static readonly Update_Account Instance = new Update_Account();

    private Update_Account() : base("Update_Account")
    {
    }
}

public sealed class Delete_Account : AccountCommand
{
    public static readonly Delete_Account Instance = new Delete_Account();

    private Delete_Account() : base("Delete_Account")
    {
    }
}

public sealed class BulkCreate_Account : AccountCommand
{
    public static readonly BulkCreate_Account Instance = new BulkCreate_Account();

    private BulkCreate_Account() : base("BulkCreate_Account", true)
    {
    }
}