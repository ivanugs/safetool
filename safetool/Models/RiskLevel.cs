using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace safetool.Models
{
    // Principal
    public class RiskLevel
    {
        [Key]
        public int ID { get; set; }
        public required string Level { get; set; }
        public bool Active { get; set; }

        // Relacion de uno a varios obligatoria con Device
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
