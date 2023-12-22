using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterApp
{
    public class RequestException : Exception
    {
        public RequestException()
        {
        }

        public RequestException(string message)
            : base(message)
        {
        }

        public RequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
