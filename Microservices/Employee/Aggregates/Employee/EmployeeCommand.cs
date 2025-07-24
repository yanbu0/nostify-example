

using nostify;

namespace Employee_Service;

public class EmployeeCommand : NostifyCommand
{
    ///<summary>
    ///Base Create Command
    ///</summary>
    public static readonly EmployeeCommand Create = new EmployeeCommand("Create_Employee", true);
    ///<summary>
    ///Base Update Command
    ///</summary>
    public static readonly EmployeeCommand Update = new EmployeeCommand("Update_Employee");
    ///<summary>
    ///Base Delete Command
    ///</summary>
    public static readonly EmployeeCommand Delete = new EmployeeCommand("Delete_Employee");
    ///<summary>
    ///Bulk Create Command
    ///</summary>
    public static readonly EmployeeCommand BulkCreate = new EmployeeCommand("BulkCreate_Employee", true);


    public EmployeeCommand(string name, bool isNew = false)
    : base(name, isNew)
    {

    }
}