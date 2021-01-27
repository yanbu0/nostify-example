using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using nostify;


namespace nostify_example
{

    public class UserCommand : AggregateCommand
    {

        public static readonly UserCommand CreateUser = new UserCommand(1, $"Create User","User");
        public static readonly UserCommand UpdateUser = new UserCommand(2, $"Update User","User");

        public UserCommand(int id, string name, string aggregateType)
        : base(id, name, aggregateType)
        {

        }
    }

    public class User : Aggregate
    {
        public User() { }

        public int tenantId { get; set; }
        public string userName { get; set; }


        public override void Apply(PersistedEvent pe)
        {
            if (pe.command == UserCommand.CreateUser || pe.command == UserCommand.UpdateUser){
                this.UpdateProperties<User>(pe.payload);
            }
        }
    }

}
