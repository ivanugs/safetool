using System;
using System.Collections.Generic;

namespace safetool.Models
{
    // Dependent (Parent Role)
    public class UserRole
    {
        public int ID { get; set; }
        public required string UserName { get; set; }
        public int RoleID { get; set; } // Foreign key property

        // Required reference navigation to principal
        public Role Role { get; set; } = null!;
    }
}
