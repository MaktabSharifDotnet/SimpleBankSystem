using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBankSystem.Exceptions
{
    public class PasswordWrongException : VallidationException
    {
        public PasswordWrongException(string message) : base(message)
        {
        }
    }
}
