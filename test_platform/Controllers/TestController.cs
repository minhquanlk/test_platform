using Microsoft.AspNetCore.Mvc;
using test_platform.Models.Auth;
using test_platform.Models.Quiz;
using test_platform.Models;
using test_platform.Services;
using FireSharp.Response;
using Firebase.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace test_platform.Controllers
{
   
    public class TestController : Controller
    {
        private readonly DBService _clientManager;
        private readonly IUserContext _userContext;
        private readonly IQuizContext _quizContext;


        public TestController(DBService ClientManager, IUserContext userContext, IQuizContext quizContext)
        {
            _clientManager = ClientManager;
            _userContext = userContext;
            _quizContext = quizContext;
        }
        
        public IActionResult User(string id)
        {
            if (string.IsNullOrEmpty(id))
            {

                return RedirectToAction("Error");
            }
            var token = HttpContext.Session.GetString("_UserToken");
            if (token != null)
            {
                if (_userContext.UserWithRole.Role == "Admin")
                {
                    return RedirectToAction("Admin");
                }
                else
                {

                    return View(_quizContext.quizList);
                }
                
            }
            return RedirectToAction("SignIn", "Auth");
        }
        public IActionResult Admin()
        {
            
            return View(_quizContext.quizList);
            
        }
    }
}