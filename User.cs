using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using nostify;


namespace nostify_example
{

    // public class UserCommand : AggregateCommand
    // {

    //     public static readonly UserCommand CreateUser = new UserCommand(1, $"Create User");
    //     public static readonly UserCommand UpdateUser = new UserCommand(2, $"Update User");
    //     public static readonly UserCommand DeleteUser = new UserCommand(3, $"Delete User");

    //     public UserCommand(int id, string name)
    //     : base(id, name)
    //     {

    //     }
    // }

    public class User : Aggregate
    {
        public User() { 
        }

        public int tenantId { get; set; }
        public string userName { get; set; }
        new public static string aggregateType => "User";

        public override void Apply(PersistedEvent pe)
        {
            if (pe.command == AggregateCommand.Create || pe.command == AggregateCommand.Update){
                this.UpdateProperties<User>(pe.payload);
            }
            else if (pe.command == AggregateCommand.Delete) {
                
            }
        }
    }

}
