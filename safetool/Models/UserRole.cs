using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace safetool.Models
{
    // Dependent (Parent Role)
    public class UserRole
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "El campo UID es obligatorio")]
        public required string UserName { get; set; }

        public int RoleID { get; set; } // Foreign key property

        // Required reference navigation to principal
        public virtual Role? Role { get; set; }
    }
}
