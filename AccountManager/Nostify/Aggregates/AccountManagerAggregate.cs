using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using nostify;
using Newtonsoft.Json.Linq;


namespace AccountManager_Service
{

    public class AccountManagerCommand : AggregateCommand
    {


        public AccountManagerCommand(string name)
        : base(name)
        {

        }
    }

    public class AccountManager : Aggregate
    {
        public AccountManager()
        {
        }

        public string name { get; set; }

        new public static string aggregateType => "AccountManager";

        public override void Apply(PersistedEvent pe)
        {
            if (pe.command == AggregateCommand.Create || pe.command == AggregateCommand.Update)
            {
                this.UpdateProperties<AccountManager>(pe.payload);
            }
            else if (pe.command == AggregateCommand.Delete)
            {
                this.isDeleted = true;
            }
        }
    }


}
