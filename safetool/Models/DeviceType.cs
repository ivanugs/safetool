using System;
using System.Collections.Generic;

namespace safetool.Models
{
    // Principal
    public class DeviceType
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public bool Active { get; set; }

        //Relacion uno a varios obligatoria con Device
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
