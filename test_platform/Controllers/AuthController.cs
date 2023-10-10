using Microsoft.AspNetCore.Mvc;
using test_platform.Models.Auth;
using Firebase.Auth;
using Newtonsoft.Json;
using test_platform.Models.Quiz;
using test_platform.Services;
using FireSharp.Response;

namespace test_platform.Controllers
{
    
    public class AuthController : Controller
    {
        FirebaseAuthProvider auth;
        private readonly string apiAuth = "AIzaSyAmGHmcNm1Ob6lShTCIMF5sPOh8KSl5UsQ";
        private readonly DBService _clientManager;
        private UserInfo _candidate;
        private IUserContext _userContext;
        private readonly IQuizContext _quizContext;
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public AuthController(DBService ClientManager, IUserContext userContext, IQuizContext quizContext)
        {
            _clientManager = ClientManager;
            _userContext = userContext;
            _quizContext = quizContext;
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig(apiAuth));
        }

        public IActionResult Apply()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            var token = HttpContext.Session.GetString("_UserToken");
            if (token != null)
            {
                return RedirectToAction("User", "Test", new { id = _userContext.UserWithRole.User.Id });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInView loginModel)
        {
            
            try
            {
                //log in an existing user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                
                string token = fbAuthLink.FirebaseToken;
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);
                    var client = _clientManager.FirebaseClient;
                    var mail = loginModel.Email;
                    FirebaseResponse responseUser = client.Get("Candidates");
                    var candidates = responseUser.ResultAs<Dictionary<string, UserInfo>>();
                    FirebaseResponse responseQuiz = client.Get("QuizList");
                    var quizList = responseQuiz.ResultAs<Dictionary<string, QuizInfo>>();

                    foreach (var candidate in candidates)
                    {
                        if (candidate.Value.Email == mail)
                        {
                            UserAndRole _userWithRole = new UserAndRole();
                            _candidate = candidate.Value;
                            _userWithRole.User = _candidate;
                            _userWithRole.Role = _candidate.Role;
                            _userContext.SetUserWithRole(_userWithRole);
                            break;
                        }
                    }
                    foreach (var quiz in quizList)
                    {
                        QuizInfo _quizInfo = quiz.Value;
                        _quizContext.Add(_quizInfo);
                    }
                    return RedirectToAction("User", "Test", new { id = _candidate.Id });
                }

            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(loginModel);
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Apply(UserInfo candidate)
        {

            FirebaseAuthProvider auth = new FirebaseAuthProvider(
                                new FirebaseConfig(apiAuth));

            string randomString = GenerateRandomString(10);
            bool haved = false;
            try
            {
                //create the user

                await auth.CreateUserWithEmailAndPasswordAsync(candidate.Email, randomString);


            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);

                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                haved = true;


            }

            if (haved != true)
            {
                try
                {
                    var client = _clientManager.FirebaseClient;
                    var data = candidate;
                    data.Password = randomString;
                    data.Role = "User";
                    PushResponse response = client.Push("Candidates/", data);

                    data.Id = response.Result.name;
                    SetResponse setResponse = client.Set("Candidates/" + data.Id, data);

                    if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ModelState.AddModelError(string.Empty, "Added Succesfully");
                        return RedirectToAction("Create", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong!!");
                    }
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }


            return View();

        }

        public IActionResult SignOut()
        {
            HttpContext.Session.Remove("_UserToken");
            _userContext.SetUserWithRole(new UserAndRole());
            _quizContext.Reset();
            return RedirectToAction("SignIn");
        }
    }
}