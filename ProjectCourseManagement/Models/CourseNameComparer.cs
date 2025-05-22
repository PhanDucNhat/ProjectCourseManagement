using System;
using System.Collections.Generic;

namespace ProjectCourseManagement.Models
{
    public class CourseNameComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            // So sánh không phân biệt hoa/thường
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Hàm băm chính (hash1) tối ưu cho băm kép
            unchecked
            {
                int hash = 17;
                foreach (char c in obj.ToLowerInvariant())
                {
                    hash = hash * 31 + c;
                    // Xoay bit để cải thiện phân bố
                    hash = (hash << 7) | (hash >> 25);
                }
                return hash;
            }
        }
    }
}