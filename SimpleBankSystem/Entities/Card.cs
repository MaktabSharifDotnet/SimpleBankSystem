using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBankSystem.Entities
{
    public class Card
    {
        public int Id { get; set; }

        public string CardNumber { get; set; }

        public string HolderName { get; set; }
        public float Balance { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }

        public int FailedAttemptCount { get; set; }

        [InverseProperty("SourceCard")]
        public List<Transaction> SentTransactions { get; set; } = [];

        [InverseProperty("DestinationCard")]
        public List<Transaction> ReceivedTransactions { get; set; } = [];

    }
}
