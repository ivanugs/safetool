using System;
using System.Collections.Generic;

namespace safetool.Models
{
    public class Risk
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public required string Image { get; set; }
        public bool Active { get; set; }

        // Relación muchos a muchos con Device
        public List<Device> Devices { get; set; } = new List<Device>();
    }
}
