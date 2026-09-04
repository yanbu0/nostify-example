

using nostify;

namespace Employee_Service;

public abstract class EmployeeCommand : EventType
{
    ///<summary>
    ///Base Create Command
    ///</summary>
    public static Create_Employee Create => Create_Employee.Instance;
    ///<summary>
    ///Base Update Command
    ///</summary>
    public static Update_Employee Update => Update_Employee.Instance;
    ///<summary>
    ///Base Delete Command
    ///</summary>
    public static Delete_Employee Delete => Delete_Employee.Instance;
    ///<summary>
    ///Bulk Create Command
    ///</summary>
    public static BulkCreate_Employee BulkCreate => BulkCreate_Employee.Instance;


    protected EmployeeCommand(string name, bool isNew = false)
    : base(name, isNew)
    {

    }
}

public sealed class Create_Employee : EmployeeCommand
{
    public static readonly Create_Employee Instance = new Create_Employee();

    private Create_Employee() : base("Create_Employee", true)
    {
    }
}

public sealed class Update_Employee : EmployeeCommand
{
    public static readonly Update_Employee Instance = new Update_Employee();

    private Update_Employee() : base("Update_Employee")
    {
    }
}

public sealed class Delete_Employee : EmployeeCommand
{
    public static readonly Delete_Employee Instance = new Delete_Employee();

    private Delete_Employee() : base("Delete_Employee")
    {
    }
}

public sealed class BulkCreate_Employee : EmployeeCommand
{
    public static readonly BulkCreate_Employee Instance = new BulkCreate_Employee();

    private BulkCreate_Employee() : base("BulkCreate_Employee", true)
    {
    }
}