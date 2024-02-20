using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using test_platform.Models.Auth;
using Firebase.Auth;
using Newtonsoft.Json;
using test_platform.Models.Quiz;
using test_platform.Services;
using FireSharp.Response;
using test_platform.Models;

namespace test_platform.Controllers
{
    
    public class AuthController : Controller
    {
        FirebaseAuthProvider auth;
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _env;
        private readonly IHistoryContext _historyContext;
        private readonly string apiAuth = "AIzaSyAmGHmcNm1Ob6lShTCIMF5sPOh8KSl5UsQ";
        private string bucket = "quizapp-d066c.appspot.com";
        private readonly DBService _clientManager;
        private UserInfo _candidate;
        private IUserContext _userContext;
        private readonly IQuizContext _quizContext;
        private readonly string tokenID = "_userIdPOh8KSl5UsQ";


        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public AuthController(DBService ClientManager, IUserContext userContext, IQuizContext quizContext, IWebHostEnvironment env, IMailService mailService, IHistoryContext historyContext)
        {
            _clientManager = ClientManager;
            _userContext = userContext;
            _quizContext = quizContext;
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig(apiAuth));
            _env = env;
            _mailService = mailService;
            _historyContext = historyContext;   
        }

        public IActionResult Apply()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            var token = HttpContext.Session.GetString("_UserToken");
            var idUser = HttpContext.Session.GetString(tokenID);
            if (token != null)
            {
                if (idUser == "admin")
                {
                    return RedirectToAction("UserList", "Admin");
                }    
                return RedirectToAction("QuizDetail", "Test", new { id = idUser });
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
                    UserLoginLog log = new UserLoginLog();
                    int index = 0;
                    HttpContext.Session.SetString("_UserToken", token);
                    var client = _clientManager.FirebaseClient;
                    var mail = loginModel.Email;
                    FirebaseResponse responseUser = client.Get("Candidates");
                    var candidates = responseUser.ResultAs<Dictionary<string, UserInfo>>();
                    FirebaseResponse responseAccount = client.Get("TestAccount");
                    var testAccount = responseAccount.ResultAs<Dictionary<string, UserInfo>>();
                    FirebaseResponse responseQuiz = client.Get("QuizList");
                    var quizList = responseQuiz.ResultAs<Dictionary<string, QuizInfo>>();
                    FirebaseResponse responseQuestion = client.Get("TestQuestion");
                    var quesList = responseQuestion.ResultAs<Dictionary<string, MultipleChoice>>();
                    FirebaseResponse responseUserQuest = client.Get("TestAccountQuest");
                    var userQuest = responseUserQuest.ResultAs<Dictionary<string, List<string>>>();
                    Dictionary<string, MultipleChoice> testList = new Dictionary<string, MultipleChoice>();

                    if (!mail.ToLower().Contains("testaccount"))
                    {
                        foreach (var candidate in candidates)
                        {
                            if (candidate.Value.Email.ToLower() == mail.ToLower())
                            {
                                UserAndRole _userWithRole = new UserAndRole();
                                _candidate = candidate.Value;
                                //_userWithRole.User = _candidate;
                                //_userWithRole.Role = _candidate.Role;
                                //_userContext.SetUserWithRole(_userWithRole);
                                HttpContext.Session.SetString(tokenID, candidate.Value.Id);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var candidate in testAccount)
                        {
                            if (candidate.Value.Email.ToLower() == mail.ToLower())
                            {
                                UserAndRole _userWithRole = new UserAndRole();
                                _candidate = candidate.Value;
                                //_userWithRole.User = _candidate;
                                //_userWithRole.Role = _candidate.Role;
                                //_userContext.SetUserWithRole(_userWithRole);
                                HttpContext.Session.SetString(tokenID, candidate.Value.Id);
                                break;
                            }
                        }
                    }
                    //foreach (var pair in quesList)
                    //{
                    //    index++;
                    //    if (_candidate.Id != "Admin" && _candidate.Id != "User")
                    //    {
                    //        if (userQuest[_candidate.Id].Contains(pair.Key))
                    //        {
                    //            testList.Add(pair.Key, pair.Value);
                    //        }
                    //    }
                    //}
                    //foreach (var quiz in quizList)
                    //{
                    //    QuizInfo _quizInfo = quiz.Value;
                    //    _quizInfo.QuestionArr = testList;
                    //    _quizContext.Add(_quizInfo);
                    //}
                    log.Email = _candidate.Email;
                    log.time = DateTime.Now;
                    var ipAdd = HttpContext.Connection.RemoteIpAddress;
                    
                    if (ipAdd != null)
                    {
                        log.IP = ipAdd.ToString();
                    };
                
                    PushResponse response = client.Push("LoginLog/" + _candidate.Id, log);
                    if (_candidate.Role == "Admin")
                    {
                        return RedirectToAction("UserList", "Admin", new { id = _candidate.Id });
                    }    
                    return RedirectToAction("QuizDetail", "Test", new { id = _candidate.Id });
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
        public string SendMail(MailData mailData)
        {
            return _mailService.SendMail(mailData);
        }

        [HttpPost]
        public async Task<IActionResult> Apply(UserInfo candidate)
        {
            MailData _dataMail = new MailData();
            FirebaseAuthProvider auth = new FirebaseAuthProvider(
                                new FirebaseConfig(apiAuth));
            string htmlFilePath = Path.Combine(_env.WebRootPath, "mail.html");
            
            _dataMail.EmailBody = System.IO.File.ReadAllText(htmlFilePath);
            _dataMail.EmailSubject = "Registration Succesful! - Masan RnD Talents";
            _dataMail.EmailToName = candidate.FullName;
            _dataMail.EmailToId = candidate.Email;
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
                FileStream fs;
                try
                {
                    var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(candidate.Email, randomString);
                    var client = _clientManager.FirebaseClient;
                    var data = candidate;
                    var fileUpload = data.CV;
                    var cancellation = new CancellationTokenSource();
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    var filePath = Path.Combine(uploads, fileUpload.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(fileStream);
                    }

                    data.CVName = data.CV.FileName;
                    data.CV = null;
                    string sendMail = SendMail(_dataMail);
                    if (sendMail != "true")
                    {
                        ModelState.AddModelError(string.Empty, sendMail);
                    }
                     
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
            HttpContext.Session.Remove(tokenID);
            HttpContext.Session.Remove("Submit");
            _userContext.SetUserWithRole(new UserAndRole());
            _quizContext.Reset();
            _historyContext.SetHistory(null);
            return RedirectToAction("SignIn");
        }
    }
}