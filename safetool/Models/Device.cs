﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace safetool.Models
{
    //Dependent
    public class Device
    {
        public int ID { get; set; }

        public int LocationID { get; set; } //Foreign key property
        //public Location Location { get; set; } = null!; // relacion de uno a varios obligatoria con Location

        public int AreaID { get; set; } //Foreign key property
        public Area? Area { get; set; } // relacion de uno a varios obligatoria con Area

        public int DeviceTypeID { get; set; } //Foreign key property
        public DeviceType? DeviceType { get; set; } // relacion de uno a varios obligatoria con DeviceType

        public int RiskLevelID { get; set; } //Foreign key property
        public RiskLevel? RiskLevel { get; set; } // relacion de uno a varios obligatoria con RiskLevel

        // Relación muchos a muchos con PPE
        public ICollection<PPE> PPEs { get; set; } = new List<PPE>();

        // Relación muchos a muchos con Risk
        public ICollection<Risk> Risks { get; set; } = new List<Risk>();

        public string? Image {  get; set; }
        public required string Name { get; set; }
        public required string Function { get; set; }
        public required string SpecificFunction { get; set; }
        public required int Operators { get; set; }
        public required DateOnly LastMaintenance { get; set; }
        public string? EmergencyStopImage { get; set; }
        public string? TypeSafetyDevice { get; set; }
        public string? FunctionSafetyDevice { get; set; }
        public bool Active { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public IFormFile? ImageFileES {  get; set; }
    }
}
