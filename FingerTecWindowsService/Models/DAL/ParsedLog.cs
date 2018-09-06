using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerTecWindowsService.Models.DAL
{
    public partial class ParsedLog
    {
        public long Id { get; set; }

        public DateTime? CheckDate { get; set; }

        public TimeSpan? CheckIn { get; set; }

        public TimeSpan? CheckOut { get; set; }

        public int? TypeId { get; set; }

        public int? EmployeeId { get; set; }
        
    }
}
