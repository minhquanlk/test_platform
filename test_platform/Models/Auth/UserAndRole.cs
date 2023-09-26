using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_platform.Models.Auth
{
    public class UserAndRole
    { 
        public UserInfo User { get; set; }
        public string Role { get; set; }
    }
    

    
}