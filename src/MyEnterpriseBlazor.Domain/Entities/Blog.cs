using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEnterpriseBlazor.Domain
{
    [Table("blog")]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Handle { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var blog = obj as Blog;
            if (blog?.Id == null || blog?.Id == 0 || Id == 0) return false;
            return EqualityComparer<int>.Default.Equals(Id, blog.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Blog{" +
                    $"ID='{Id}'" +
                    $", Name='{Name}'" +
                    $", Handle='{Handle}'" +
                    "}";
        }
    }
}
