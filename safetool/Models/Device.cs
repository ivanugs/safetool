using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace safetool.Models
{
    //Dependent
    public class Device
    {
        public int ID { get; set; }
        public int LocationID { get; set; } //Foreign key property
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
        public string? Name { get; set; }
        public string? Model { get; set; }
        public string? Function { get; set; }
        public string? SpecificFunction { get; set; }
        public int Operators { get; set; }
        public DateOnly LastMaintenance { get; set; }
        public string? EmergencyStopImage { get; set; }
        public string? TypeSafetyDevice { get; set; }
        public string? FunctionSafetyDevice { get; set; }
        public bool Active { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        [NotMapped]
        public IFormFile? ImageFileES {  get; set; }
        [NotMapped]
        public List<int> SelectedPPEs { get; set; } = new List<int>();
        [NotMapped]
        public List<int> SelectedRisks { get; set; } = new List<int>();
    }
}
