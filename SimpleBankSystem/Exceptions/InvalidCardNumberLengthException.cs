using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBankSystem.Exceptions
{
    public class InvalidCardNumberLengthException : VallidationException
    {
        public InvalidCardNumberLengthException(string message) : base(message)
        {

        }
    }
}
