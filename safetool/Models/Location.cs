﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace safetool.Models
{
    //Principal 
    public class Location
    {
        [Key]
        public int ID { get; set; }
        public required string Name { get; set; }
        public required string Acronym { get; set; }
        public bool Active {  get; set; }

        // Relacion uno a varios obligatoria con Area
        public ICollection<Area> Areas { get; set; } = new List<Area>();
    }
}
