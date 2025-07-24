

using nostify;

namespace Account_Service;

public class AccountStatusCommand : NostifyCommand
{
    ///<summary>
    ///Base Create Command
    ///</summary>
    public static readonly AccountStatusCommand Create = new AccountStatusCommand("Create_AccountStatus", true);
    ///<summary>
    ///Base Update Command
    ///</summary>
    public static readonly AccountStatusCommand Update = new AccountStatusCommand("Update_AccountStatus");
    ///<summary>
    ///Base Delete Command
    ///</summary>
    public static readonly AccountStatusCommand Delete = new AccountStatusCommand("Delete_AccountStatus");
    ///<summary>
    ///Bulk Create Command
    ///</summary>
    public static readonly AccountStatusCommand BulkCreate = new AccountStatusCommand("BulkCreate_AccountStatus", true);


    public AccountStatusCommand(string name, bool isNew = false)
    : base(name, isNew)
    {

    }
}