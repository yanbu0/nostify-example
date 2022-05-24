using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using nostify;
using Newtonsoft.Json.Linq;


namespace AccountManager_Service
{

    public class AccountManagerCommand : NostifyCommand
    {

        public static readonly AccountManagerCommand UpdateName = new AccountManagerCommand("Update Name");


        public AccountManagerCommand(string name)
        : base(name)
        {

        }
    }

    public class AccountManager : Aggregate
    {
        new public string aggregateType = "AccountManager";

        public AccountManager()
        {
        }

        public string name { get; set; }


        public override void Apply(PersistedEvent pe)
        {
            if (pe.command == NostifyCommand.Create || pe.command == NostifyCommand.Update || pe.command == AccountManagerCommand.UpdateName)
            {
                this.UpdateProperties<AccountManager>(pe.payload);
            }
            else if (pe.command == NostifyCommand.Delete)
            {
                this.isDeleted = true;
            }
        }
    }


}
