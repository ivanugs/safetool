using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace safetool.Models
{
    public class GeneralParameter
    {
        [Key]
        public int Id { get; set; }
        public string? EmailAccount { get; set; }
        public string? EmailAccountDisplayName { get; set; }
        public string? EmailAccountPassword { get; set; }
        public string? EmailAccountUser { get; set; }
        public string? EmailPort { get; set; }
        public string? EmailServer { get; set; }
        public bool EmailSsl { get; set; }
    }
}
