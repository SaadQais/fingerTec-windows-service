using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerTecWindowsService.Models.DAL
{
    public partial class Device
    {
        public int? Id { get; set; }

        public int DeviceId { get; set; }

        [Required]
        [StringLength(200)]
        public string DeviceName { get; set; }

        [StringLength(1000)]
        public string IP { get; set; }

        [StringLength(1000)]
        public string Port { get; set; }

        [StringLength(1000)]
        public string Key { get; set; }
    }
}
