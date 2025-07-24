using nostify;


namespace Account_Service;

public abstract class AccountBaseClass : NostifyObject
{
    public string name { get; set; }
    public Guid statusId { get; set; }
    public Guid accountManagerId { get; set; }
}