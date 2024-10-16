using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace safetool.Models
{
    public class FormSubmission
    {
        [Key]
        public int ID { get; set; }

        public string? EmployeeUID { get; set; }

        public string? EmployeeName { get; set; }
        public string? EmployeeEmail { get; set; }

        public int DeviceID { get; set; } // Foreign key
        public Device? Device { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
