// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyEnterpriseBlazor.Dto
{
    public class TagDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
