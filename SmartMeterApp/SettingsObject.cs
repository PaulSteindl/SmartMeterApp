using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterApp
{
    [Serializable]
    public class SettingsObject
    {
        public string Top6Url { get; set; }

        public string Top7Url { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int IntervallRealtimeData { get; set; }

        public int IntervallLogging { get; set; }

        public bool FlagTop6Phase1 { get; set; }

        public bool FlagTop6Phase2 { get; set; }

        public bool FlagTop6Phase3 { get; set; }

        public bool FlagTop7Phase1 { get; set; }

        public bool FlagTop7Phase2 { get; set; }

        public bool FlagTop7Phase3 { get; set; }
    }
}
