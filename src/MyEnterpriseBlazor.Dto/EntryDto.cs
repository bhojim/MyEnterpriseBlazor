// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyEnterpriseBlazor.Dto
{
    public class EntryDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public int BlogId { get; set; }
        public IList<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}
