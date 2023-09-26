using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_platform.Models.Quiz
{
    public class MultipleChoice
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public QuestionType Type { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public List<string> Choices { get; set; }

        [Required]
        public List<string> CorrectAnswer { get; set; }

        [Required]
        public int Point { get; set; }

    }

    public enum QuestionType
    {
        TracNghiemDon,
        TracNghiemNhieu,
        TuLuan
    }
}