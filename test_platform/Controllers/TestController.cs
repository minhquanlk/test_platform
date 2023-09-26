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

        private List<QuizInfo> _quiz = new List<QuizInfo>();
        private TestView _testView = new TestView();
        
        public TestController(DBService ClientManager, IUserContext userContext)
        {
            _clientManager = ClientManager;
            _userContext = userContext;
        }
        public IActionResult Admin(TestView testview)
        {
            return View(testview);
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
                try
                {
                    var client = _clientManager.FirebaseClient;
                    FirebaseResponse responseQuiz = client.Get("QuizList");
                    var quizList = responseQuiz.ResultAs<Dictionary<string, QuizInfo>>();
                    // Tìm candidate dựa trên email (thay "example@email.com" bằng email thực tế)
                   
                    foreach (var quiz in quizList)
                    {
                        QuizInfo _quizInfo = quiz.Value;
                        _quiz.Add(_quizInfo);
                    }
                    _testView.user = _userContext.UserWithRole.User;
                    _testView.quizs = _quiz;
                    if (_userContext.UserWithRole.Role == "Admin")
                    {
                        return RedirectToAction("Admin", _testView);
                    }    
                    return View(_testView);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, "connect database error");
                   
                }
            }
            return RedirectToAction("SignIn", "Auth");
        }
        
    }
}