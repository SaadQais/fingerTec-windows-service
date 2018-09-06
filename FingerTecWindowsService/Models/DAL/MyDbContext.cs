using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerTecWindowsService.Models.DAL
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base("name=MyModels")
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<ParsedLog> ParsedLogs { get; set; }
    }
}
