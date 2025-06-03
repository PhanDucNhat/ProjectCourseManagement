using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCourseManagement.Models
{
    [Table("Lesson")]
    public class Lesson
    {
        [Key]
        public int LessonID { get; set; }

        [Required(ErrorMessage = "Tên bài học không được để trống.")]
        public string LessonName { get; set; }

        public string? Description { get; set; }

        [Url(ErrorMessage = "URL video không hợp lệ.")]
        public string? VideoUrl { get; set; }

        public string? ExerciseContent { get; set; }

        public int CourseID { get; set; }

        [ForeignKey("CourseID")]
        public Course? Course { get; set; }
    }
}
