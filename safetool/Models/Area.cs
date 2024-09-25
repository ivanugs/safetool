using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace safetool.Models
{
    //Dependent
    public class Area
    {
        public int ID { get; set; }

        [Required(AllowEmptyStrings = true)]
        public int LocationID { get; set; } //Foreign key property
        public required string Name { get; set; }
        public bool Active { get; set; }

        // Relacion de uno a varios obligatoria con Location
        public Location? Location { get; set; }

        // Relacion de uno a varios obligatoria con Device
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
