﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace safetool.Models
{
    public class PPE
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "El campo Nombre del EPP es obligatorio")]
        public required string Name { get; set; }

        public string? Image { get; set; }

        public bool Active { get; set; }

        // Relación muchos a muchos con Device
        public ICollection<Device> Devices { get; set; } = new List<Device>();

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
