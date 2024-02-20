using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_platform.Models.Quiz
{
    public class MultipleChoice
    {
       
        public int IdPic { get; set; }
        public string Id { get; set; }
        public string Topic { get; set; }
       
        public string Type { get; set; }
    
        public string QuestionPic { get; set; }
     
        public string QuestionText { get; set; }

     
        public Dictionary<string, string> Choices { get; set; }

  
        public Dictionary<string, string> CorrectAnswer { get; set; }

 
        public double Point { get; set; }

    }

}