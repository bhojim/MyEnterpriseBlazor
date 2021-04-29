using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MyEnterpriseBlazor.Domain
{
    [Table("tag")]
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [JsonIgnore]
        public IList<Entry> Entries { get; set; } = new List<Entry>();

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var tag = obj as Tag;
            if (tag?.Id == null || tag?.Id == 0 || Id == 0) return false;
            return EqualityComparer<int>.Default.Equals(Id, tag.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Tag{" +
                    $"ID='{Id}'" +
                    $", Name='{Name}'" +
                    "}";
        }
    }
}
