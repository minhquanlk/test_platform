using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_platform.Models.Auth
{
    public class UserLoginLog
    { 
        public string Email { get; set; }
        public DateTime time { get; set; }

        public string IP { get; set; }
    }
    

    
}