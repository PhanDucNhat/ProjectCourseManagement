using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCourseManagement.Models
{
    [Table("Course")]
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        public string? CourseName { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Instructor { get; set; }

        
    }
}
