using nostify;


namespace Employee_Service;

public class Employee : EmployeeBaseClass, IAggregate
{
    public Employee()
    {
    }

    public bool isDeleted { get; set; } = false;

    public static string aggregateType => "Employee";
    public static string currentStateContainerName => $"{aggregateType}CurrentState";

    public override void Apply(Event eventToApply)
    {
        if (eventToApply.command == EmployeeCommand.Create || eventToApply.command == EmployeeCommand.BulkCreate || eventToApply.command == EmployeeCommand.Update)
        {
            //Note: this uses reflection, may be desirable to optimize
            this.UpdateProperties<Employee>(eventToApply.payload);
        }
        else if (eventToApply.command == EmployeeCommand.Delete)
        {
            this.isDeleted = true;
        }
    }
}



