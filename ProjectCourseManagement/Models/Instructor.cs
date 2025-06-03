using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCourseManagement.Models
{
    [Table("Instructor")]
    public class Instructor
    {
        [Key]
        public int InstructorId { get; set; }

        [Required]
        [StringLength(100)]
        public string InstructorName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
