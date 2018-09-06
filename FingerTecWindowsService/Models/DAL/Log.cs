using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerTecWindowsService.Models.DAL
{
    public class Log
    {
        public long LogId { get; set; }

        public DateTime? CheckDate { get; set; }

        public TimeSpan? CheckTime { get; set; }

        public int? DeviceId { get; set; }

        public int? CheckType { get; set; }

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
