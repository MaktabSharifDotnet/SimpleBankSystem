using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBankSystem.Exceptions
{
    public class NotCardLoginException : VallidationException
    {
        public NotCardLoginException(string message) : base(message)
        {
        }
    }
}
