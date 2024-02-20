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
using Newtonsoft.Json;
using NuGet.Common;
using System.Data.Entity.Migrations.History;
using FireSharp.Interfaces;
using System.Data.Entity;

namespace test_platform.Controllers
{
   
    public class TestController : Controller
    {
        private readonly DBService _clientManager;
        private readonly IUserContext _userContext;
        private readonly IHistoryContext _historyContext;
        private UserInfo _candidate;
        private readonly IQuizContext _quizContext;
        private readonly string tokenID = "_userIdPOh8KSl5UsQ";
        public DateTime now;
        
        private string apiDB = "ToidM7c2xdGklPLnZco0h5xobznmqFTZx3HeXkzK";
        private string basePath = "https://quizapp-d066c-default-rtdb.asia-southeast1.firebasedatabase.app";
        public IFirebaseClient FirebaseClientSubmit { get; private set; }
        public TestController(DBService ClientManager, IUserContext userContext, IQuizContext quizContext, IHistoryContext historyContext)
        {
            _clientManager = ClientManager;
            _userContext = userContext;
            _quizContext = quizContext;
            _historyContext = historyContext;
        }
        public List<QuizInfo> getQuizList()
        {
            var idUser = HttpContext.Session.GetString(tokenID);
            int index = 0;
            var client = _clientManager.FirebaseClient;
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


            foreach (var candidate in candidates)
            {
                if (candidate.Key == idUser)
                {
                    UserAndRole _userWithRole = new UserAndRole();
                    _candidate = candidate.Value;
                    _userWithRole.User = _candidate;
                    _userWithRole.Role = _candidate.Role;
                    _userContext.SetUserWithRole(_userWithRole);
                    break;
                }
            }


            foreach (var candidate in testAccount)
            {
                if (candidate.Key == idUser)
                {
                    UserAndRole _userWithRole = new UserAndRole();
                    _candidate = candidate.Value;
                    _userWithRole.User = _candidate;
                    _userWithRole.Role = _candidate.Role;
                    _userContext.SetUserWithRole(_userWithRole);
                    break;
                }
            }

            foreach (var pair in quesList)
            {
                index++;
                if (_candidate.Id != "Admin" && _candidate.Id != "User")
                {
                    if (userQuest[_candidate.Id].Contains(pair.Key))
                    {
                        testList.Add(pair.Key, pair.Value);
                    }
                }
            }
            foreach (var quiz in quizList)
            {
                QuizInfo _quizInfo = quiz.Value;
                _quizInfo.QuestionArr = testList;
                _quizContext.Add(_quizInfo);
            }
            return _quizContext.quizList;
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

            return RedirectToAction("UserList", "Admin");


        }
        public IActionResult QuizComplete()
        {
            HttpContext.Session.SetString("Submit", "true");
            return View();
        }
        public IActionResult QuizID()
        { 

            var token = HttpContext.Session.GetString("_UserToken");
            var isSubmit = HttpContext.Session.GetString("Submit");
            if (token == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }
            if (isSubmit == "true")
            {
                return RedirectToAction("QuizDetail");
            }
           
            var idUser = HttpContext.Session.GetString(tokenID);
            DateTime dateTimeA;
            DateTime dateTimeB;
            TimeSpan timeDifference;
            var client = _clientManager.FirebaseClient;
            List<QuizInfo> quizList = getQuizList();
            Answer answer = new Answer();
            FirebaseResponse responseUser = client.Get("Start");
            var start = responseUser.ResultAs<Dictionary<string, string>>();
            if (start.Keys.Contains(idUser))
            {
                dateTimeA = DateTime.Parse(start[idUser]);
            }
            else 
            {
                
                dateTimeA = DateTime.Now;
                SetResponse response = client.Set("Start/" + idUser, dateTimeA.ToString());
            }
            dateTimeB = dateTimeA.AddMinutes(60);
            timeDifference = DateTime.Now.Subtract(dateTimeB);
            double minutesDifference = Math.Abs(timeDifference.TotalSeconds);
            
            QuizViewModel viewModel = new QuizViewModel
            {
                QuizInfoList = quizList,
                Answer = answer,
                userID = idUser,
                Minutes = (int)minutesDifference/60,
                Seconds = (int)minutesDifference%60
            };
            return View(viewModel);
        }
        public IActionResult QuizResult()
        {
            
            List<QuizInfo> quizList = _quizContext.quizList;
            QuizViewModel viewModel = new QuizViewModel
            {
                QuizInfoList = quizList,
                Answer = _historyContext.History
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(Answer answer)
        {
            IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
            {
                AuthSecret = apiDB,
                BasePath = basePath
            };
            FirebaseClientSubmit = new FireSharp.FirebaseClient(config);
            
            var client = FirebaseClientSubmit;
            //FirebaseResponse responseUser = client.Get("Start");

            var data = answer;
            //var start = responseUser.ResultAs<Dictionary<string, string>>();
            //data.Points = 0;
            //data.Start = DateTime.Parse(start[_userContext.UserWithRole.User.Id]);
            data.End = DateTime.Now;
            //foreach (var item in _quizContext.quizList[0].QuestionArr)
            //{
            //    if (item.Value.Type == "single")
            //    {
            //        if (data.UserChoices[item.Key]["ans"] == item.Value.CorrectAnswer["ans1"])
            //        {
            //            data.Points += item.Value.Point;
            //        }    
            //    }
            //    else
            //    {
            //        int correct = 0;
            //        int countCorrect = item.Value.CorrectAnswer.Count;
            //        foreach (var ans in item.Value.CorrectAnswer)
            //        {
            //            if (data.UserChoices[item.Key].ContainsValue(ans.Value))
            //            {
            //                correct++;
                             
            //            }
            //        }
            //        data.Points += (double)correct / countCorrect * item.Value.Point;
            //    } 
            //}
            
            //data.During = (int)(data.End - data.Start).TotalSeconds;
            SetResponse response = await client.SetAsync("History/" + data.userID, data);
            //_historyContext.SetHistory(data);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError(string.Empty, "Succesfully");
                return RedirectToAction("QuizComplete"); 
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Something went wrong!!");
            }
            return RedirectToAction("QuizComplete");

        }
        public IActionResult QuizRedirect()
        {
            if (_historyContext.History != null)
            {
                return RedirectToAction("QuizDetail");
            }
            else
            {
                HttpContext.Session.SetString("_QuizStart", "true");
                return RedirectToAction("QuizDetail");
            }    
            
        }

        public IActionResult QuizDetail()
        {
            var token = HttpContext.Session.GetString("_UserToken");
            var idUser = HttpContext.Session.GetString(tokenID);
            if (token == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }
            QuizViewModel viewModel;

            DateTime dateTimeA = new DateTime();
            var client = _clientManager.FirebaseClient;
            List<QuizInfo> quizListModel = getQuizList();
            FirebaseResponse responseUserHis = client.Get("History");
            var history = responseUserHis.ResultAs<Dictionary<string, Answer>>();
            FirebaseResponse responseUser = client.Get("Start");
            var start = responseUser.ResultAs<Dictionary<string, string>>();
            if (start.Keys.Contains(idUser))
            {
                dateTimeA = DateTime.Parse(start[idUser]);
            }
            else
            {
                dateTimeA = DateTime.Now;
            }    
            DateTime dateTimeB = new DateTime();
            
            if (history != null)
            {
                if (history.Keys.Contains(idUser))
                {
                    HttpContext.Session.SetString("Submit", "true");
                    _historyContext.SetHistory(history[idUser]);
                    dateTimeB = _historyContext.History.End;
                    viewModel = new QuizViewModel
                    {
                        QuizInfoList = quizListModel,
                        Answer = _historyContext.History
                    };
                    
                }
                else
                {
                    dateTimeB = DateTime.Now;
                    viewModel = new QuizViewModel
                    {
                        QuizInfoList = quizListModel,
                        Answer = null
                    };
                } 
                    
            }
            else
            {
                dateTimeB = DateTime.Now;
                viewModel = new QuizViewModel
                {
                    QuizInfoList = quizListModel,
                    Answer = null
                };
            }
            double soPhut = (dateTimeB - dateTimeA).TotalMinutes;
            if (soPhut > 70)
            {
                HttpContext.Session.SetString("Submit", "true");
                viewModel = new QuizViewModel
                {
                    QuizInfoList = quizListModel,
                    Answer = new Answer
                    {
                        userID = "notSubmitted"
                    }
                };
            }
            else if (soPhut > 1)
            {
                var isSubmit = HttpContext.Session.GetString("Submit");
                if (isSubmit != "true")
                {
                    viewModel = new QuizViewModel
                    {
                        QuizInfoList = quizListModel,
                        Answer = new Answer
                        {
                            userID = "continue"
                        }
                    };
                }
                
            }    
            return View(viewModel);
        }
        
    }
}