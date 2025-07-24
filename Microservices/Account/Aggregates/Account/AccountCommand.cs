

using nostify;

namespace Account_Service;

public class AccountCommand : NostifyCommand
{
    ///<summary>
    ///Base Create Command
    ///</summary>
    public static readonly AccountCommand Create = new AccountCommand("Create_Account", true);
    ///<summary>
    ///Base Update Command
    ///</summary>
    public static readonly AccountCommand Update = new AccountCommand("Update_Account");
    ///<summary>
    ///Base Delete Command
    ///</summary>
    public static readonly AccountCommand Delete = new AccountCommand("Delete_Account");
    ///<summary>
    ///Bulk Create Command
    ///</summary>
    public static readonly AccountCommand BulkCreate = new AccountCommand("BulkCreate_Account", true);


    public AccountCommand(string name, bool isNew = false)
    : base(name, isNew)
    {

    }
}