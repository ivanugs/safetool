using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace safetool.Models
{
    public class FormSubmission
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Se requiere el nombre.")]
        public string? EmployeeNumber { get; set; }

        [Required(ErrorMessage = "Se requiere el numero.")]
        public string? EmployeeName { get; set; }

        public int DeviceID { get; set; } // Foreign key
        public Device? Device { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
