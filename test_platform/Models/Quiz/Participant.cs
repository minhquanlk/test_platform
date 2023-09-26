using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_platform.Models.Quiz
{
    public class Participant
    {
        
        public int userId { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }

        public int Point { get; set; }
    }



}