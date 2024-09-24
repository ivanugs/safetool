using System;
using System.Collections.Generic;

namespace safetool.Models
{
    //Dependent
    public class Area
    {
        public int ID { get; set; }
        public int LocationID { get; set; } //Foreign key property
        public required string Name { get; set; }
        public bool Active { get; set; }

        // Relacion de uno a varios obligatoria con Location
        public Location Location { get; set; } = null!;

        // Relacion de uno a varios obligatoria con Device
        public ICollection<Device> Devices { get; } = new List<Device>();
    }
}
