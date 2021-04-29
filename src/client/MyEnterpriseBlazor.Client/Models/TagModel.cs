using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyEnterpriseBlazor.Client.Models
{
    public class TagModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public IList<EntryModel> Entries { get; set; } = new List<EntryModel>();

    }
}
