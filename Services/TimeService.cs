using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class TimeService
    {
        public TimeSpan Interval { get; set; }
        public TimeService()
        {
            Interval = new TimeSpan(0, 1, 0);
        }
    }
}
