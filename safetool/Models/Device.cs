using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace safetool.Models
{
    //Dependent
    public class Device
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una localidad")]
        public int LocationID { get; set; } //Foreign key property

        [Required(ErrorMessage = "Debe seleccionar un área")]
        public int AreaID { get; set; } //Foreign key property

        public Area? Area { get; set; } // relacion de uno a varios obligatoria con Area

        [Required(ErrorMessage = "Debe seleccionar el tipo de equipo")]
        public int DeviceTypeID { get; set; } //Foreign key property

        public DeviceType? DeviceType { get; set; } // relacion de uno a varios obligatoria con DeviceType

        [Required(ErrorMessage = "Debe seleccionar el nivel de riesgo")]
        public int RiskLevelID { get; set; } //Foreign key property

        public RiskLevel? RiskLevel { get; set; } // relacion de uno a varios obligatoria con RiskLevel

        // Relación muchos a muchos con PPE
        [Required(ErrorMessage = "Debe seleccionar uno o más EPP")]
        public ICollection<PPE> PPEs { get; set; } = new List<PPE>();

        // Relación muchos a muchos con Risk
        [Required(ErrorMessage = "Debe seleccionar uno o más riesgos")]
        public ICollection<Risk> Risks { get; set; } = new List<Risk>();

        public string? Image {  get; set; }

        [Required(ErrorMessage = "El campo Nombre del equipo es obligatorio")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "El campo Modelo del equipo es obligatorio")]
        public string? Model { get; set; }

        [Required(ErrorMessage = "El campo Función del equipo es obligatorio")]
        public string? Function { get; set; }

        [Required(ErrorMessage = "El campo Función específica del equipo es obligatorio")]
        public string? SpecificFunction { get; set; }

        [Required(ErrorMessage = "El campo Operadores es obligatorio")]
        public int Operators { get; set; }

        [Required(ErrorMessage = "El campo Último mantenimiento es obligatorio")]
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
        [Required(ErrorMessage = "Debe seleccionar uno o más EPP.")]
        public List<int> SelectedPPEs { get; set; } = new List<int>();

        [NotMapped]
        [Required(ErrorMessage = "Debe seleccionar uno o más riesgos.")]
        public List<int> SelectedRisks { get; set; } = new List<int>();

        public ICollection<FormSubmission>? FormSubmissions { get; set; }

    }
}
