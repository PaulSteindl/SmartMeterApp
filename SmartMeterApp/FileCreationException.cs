using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterApp
{
    public class FileCreationException : Exception
    {
        public FileCreationException()
        {
        }

        public FileCreationException(string message)
            : base(message)
        {
        }

        public FileCreationException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
