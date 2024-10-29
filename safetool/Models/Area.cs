using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace safetool.Models
{
    //Dependent
    public class Area
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una localidad")]
        public int LocationID { get; set; } //Foreign key property

        [Required(ErrorMessage = "El campo Nombre del área es obligatorio")]
        public string? Name { get; set; }

        public bool Active { get; set; }

        // Relacion de uno a varios obligatoria con Location
        public Location? Location { get; set; }

        // Relacion de uno a varios obligatoria con Device
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
