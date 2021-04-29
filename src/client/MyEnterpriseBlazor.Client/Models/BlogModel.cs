using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyEnterpriseBlazor.Client.Models
{
    public class BlogModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Handle { get; set; }
        public string UserId { get; set; }

    }
}
