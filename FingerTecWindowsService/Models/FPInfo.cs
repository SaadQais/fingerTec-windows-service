using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerTecWindowsService.Models
{
    public class FPInfo
    {
        public int? EmployeeId { get; set; }
        public DateTime CheckDate { get; set; }
        public int? DeviceId { get; set; }
        public TimeSpan CheckTime { get; set; }
    }
}
