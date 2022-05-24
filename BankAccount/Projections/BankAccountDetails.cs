using System;
using nostify;
using Newtonsoft.Json.Linq;

namespace nostify_example
{
    public class BankAccountDetails : Projection
    {
        new public static string containerName => "BankAccountDetails";
        
        public BankAccountDetails()
        {
        }


        public Guid id { get; set;}
        public Guid? accountManagerId { get; set; }
        public string accountManagerName { get; set; }
        public decimal currentBalance { get; set; } = 0;
        public int tenantId { get; set; }

        public override void Apply(PersistedEvent pe)
        {
            if (pe.command == NostifyCommand.Create || pe.command == NostifyCommand.Update || pe.command == BankAccountCommand.UpdateManagerName)
            {
                this.UpdateProperties<BankAccountDetails>(pe.payload);
            }
            else if (pe.command == BankAccountCommand.ProcessTransaction)
            {
                Transaction transaction = ((JObject)pe.payload).ToObject<Transaction>();
                this.currentBalance += transaction.amount;
            }
            else if (pe.command == NostifyCommand.Delete)
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
