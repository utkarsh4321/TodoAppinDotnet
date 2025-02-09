﻿using Microsoft.EntityFrameworkCore;

namespace todoApis.Models
{
    [Index(nameof(Email),IsUnique =true)]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Todo> Todos { get; set; }  = new List<Todo>();

    }
}
