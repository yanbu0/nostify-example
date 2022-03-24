using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using nostify;
using Newtonsoft.Json.Linq;


namespace nostify_example
{

    public class BankAccountCommand : NostifyCommand
    {


        public static readonly BankAccountCommand ProcessTransaction = new BankAccountCommand("Process Transaction");

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
            var applyFunctions = new Dictionary<NostifyCommand, Action>();
            applyFunctions[NostifyCommand.Create] = () => { this.UpdateProperties<BankAccount>(pe.payload); };
            applyFunctions[NostifyCommand.Update] = applyFunctions[NostifyCommand.Create];
            applyFunctions[BankAccountCommand.ProcessTransaction] = () => { 
                Transaction transaction = ((JObject)pe.payload).ToObject<Transaction>();
                 this.transactions.Add(transaction);
            };
            applyFunctions[NostifyCommand.Delete] = () => { this.isDeleted = true; };

            applyFunctions[pe.command].Invoke();
        }
    }

    public class Transaction
    {
        public decimal amount { get; set; }
    }

}
