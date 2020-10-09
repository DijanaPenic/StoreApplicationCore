﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Book
{
    public class BookPatchApiModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Author { get; set; }

        [Required]
        public Guid BookstoreId { get; set; }
    }
}