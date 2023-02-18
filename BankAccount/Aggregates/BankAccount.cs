using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using nostify;
using Newtonsoft.Json.Linq;


namespace BankAccount_Service
{

    public class BankAccountCommand : NostifyCommand
    {


        public static readonly BankAccountCommand ProcessTransaction = new BankAccountCommand("Process Transaction");

        public static readonly BankAccountCommand UpdateManagerName = new BankAccountCommand("Update Manager Name");


        public BankAccountCommand(string name)
        : base(name)
        {

        }
    }

    public class BankAccount : Aggregate
    {
        new public string aggregateType = "BankAccount";
        
        public BankAccount()
        {
            this.transactions = new List<Transaction>();
        }

        public int accountId { get; set; }
        public Guid accountManagerId { get; set; }
        public string customerName { get; set; }
        public List<Transaction> transactions { get; set; }

        public override void Apply(PersistedEvent pe)
        {
            if (pe.command == NostifyCommand.Create || pe.command == NostifyCommand.Update)
            {
                this.UpdateProperties<BankAccount>(pe.payload);
            }
            else if (pe.command == BankAccountCommand.ProcessTransaction)
            {
                Transaction transaction = ((JObject)pe.payload).ToObject<Transaction>();
                this.transactions.Add(transaction);
            }
            else if (pe.command == NostifyCommand.Delete)
            {
                this.isDeleted = true;
            }
        }
    }

    public class Transaction
    {
        public decimal amount { get; set; }
    }

}
