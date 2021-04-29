using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyEnterpriseBlazor.Client.Models
{
    public class EntryModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public int? BlogId { get; set; }

        public IList<TagModel> Tags { get; set; } = new List<TagModel>();

    }
}
