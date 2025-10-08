using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBankSystem.Exceptions
{
    public class DailyTransferLimitExceededException : VallidationException
    {
        public DailyTransferLimitExceededException(string message) : base(message)
        {
        }
    }
}
