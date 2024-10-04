using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace safetool.Models
{
    //Principal
    public class Role
    {
        [Key]
        public int ID { get; set; }
        public required string Name { get; set; }
        public bool Active { get; set; }

        // Collection navigation containing dependents
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
