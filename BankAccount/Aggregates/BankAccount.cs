using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using nostify;
using Newtonsoft.Json.Linq;


namespace nostify_example
{

    public class BankAccountCommand : AggregateCommand
    {


        public static readonly BankAccountCommand AddTransaction = new BankAccountCommand("Add Transaction");

        public static readonly BankAccountCommand UpdateManager = new BankAccountCommand("Update Manager");


        public BankAccountCommand(string name)
        : base(name)
        {

        }
    }

    public class BankAccount : Aggregate
    {
        public BankAccount()
        {
            this.transactions = new List<Transaction>();
        }

        public int accountId { get; set; }
        public Guid accountManagerId { get; set; }
        public string customerName { get; set; }
        public List<Transaction> transactions { get; set; }
        new public static string aggregateType => "BankAccount";

        public override void Apply(PersistedEvent pe)
        {
            if (pe.command == AggregateCommand.Create || pe.command == AggregateCommand.Update)
            {
                this.UpdateProperties<BankAccount>(pe.payload);
            }
            else if (pe.command == BankAccountCommand.AddTransaction)
            {
                Transaction transaction = ((JObject)pe.payload).ToObject<Transaction>();
                this.transactions.Add(transaction);
            }
            else if (pe.command == AggregateCommand.Delete)
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
