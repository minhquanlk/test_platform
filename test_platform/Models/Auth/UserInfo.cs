using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_platform.Models.Auth
{
    public class UserInfo
    {
        public string? Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string Email { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Required]
        [RegularExpression(@"[0-9].{9,11}")]
        public string PhoneNumber { get; set; }

        [Required]
        public string DegreeClassification { get; set; }

        [Required]
        public string School { get; set; }

        [Required]
        public string GraduationYear { get; set; }

        [Required]
        public string Faculty { get; set; }

        public string Password { get; set; }
        public string Role { get; set; }

        
    }
    


}