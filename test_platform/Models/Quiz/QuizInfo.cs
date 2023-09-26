using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_platform.Models.Quiz
{
    public class QuizInfo
    {
        
        public int Id { get; set; }

        
        public string Title { get; set; }

       
        public string Description { get; set; }

        public List<MultipleChoice> QuestionArr { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public List<Participant> listParticipants { get; set; }
    }



}