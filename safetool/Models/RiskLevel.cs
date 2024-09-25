using System;
using System.Collections.Generic;

namespace safetool.Models
{
    // Principal
    public class RiskLevel
    {
        public int ID { get; set; }
        public required string Level { get; set; }
        public bool Active { get; set; }

        // Relacion de uno a varios obligatoria con Device
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
