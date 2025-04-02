﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCourseManagement.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }  
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role {  get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
