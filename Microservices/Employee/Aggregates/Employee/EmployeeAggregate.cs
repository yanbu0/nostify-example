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

    [ApplyEvents("Create_Employee", "BulkCreate_Employee", "Update_Employee")]
    protected void ApplyCreateOrUpdate(IEvent eventToApply)
    {
        //Note: this uses reflection, may be desirable to optimize
        this.UpdateProperties<Employee>(eventToApply.payload);
    }

    [ApplyEvents("Delete_Employee")]
    protected void ApplyDelete(IEvent eventToApply)
    {
        this.isDeleted = true;
    }
}


