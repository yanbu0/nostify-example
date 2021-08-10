using System;
using nostify;
using Newtonsoft.Json.Linq;

namespace nostify_example
{
    public class BankAccountWithManager : Projection
    {
        public BankAccountWithManager(string containerName) : base(containerName)
        {
            
        }

        Guid id { get; set;}
        Guid accountManagerId { get; set; }
        string accountManagerName { get; set; }

        public override void Apply(PersistedEvent pe)
        {
            if (pe.command == AggregateCommand.Create || pe.command == AggregateCommand.Update)
            {
                this.UpdateProperties<BankAccountWithManager>(pe.payload);
            }
            else if (pe.command == AggregateCommand.Delete)
            {
                DoDelete();
            }
        }

        private void DoDelete()
        {
            throw new NotImplementedException();
        }
    }
}
