using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterApp
{
    public class FileWriteException : Exception
    {
        public FileWriteException()
        {
        }

        public FileWriteException(string message)
            : base(message)
        {
        }

        public FileWriteException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
