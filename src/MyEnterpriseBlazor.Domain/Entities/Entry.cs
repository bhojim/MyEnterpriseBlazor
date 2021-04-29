using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MyEnterpriseBlazor.Domain
{
    [Table("entry")]
    public class Entry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime? Date { get; set; }

        public int? BlogId { get; set; }
        public Blog Blog { get; set; }
        public IList<Tag> Tags { get; set; } = new List<Tag>();

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var entry = obj as Entry;
            if (entry?.Id == null || entry?.Id == 0 || Id == 0) return false;
            return EqualityComparer<int>.Default.Equals(Id, entry.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Entry{" +
                    $"ID='{Id}'" +
                    $", Title='{Title}'" +
                    $", Content='{Content}'" +
                    $", Date='{Date}'" +
                    "}";
        }
    }
}
