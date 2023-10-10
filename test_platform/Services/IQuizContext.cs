using FireSharp.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using test_platform.Models.Auth;
using test_platform.Models.Quiz;

namespace test_platform.Services
{
    public interface IQuizContext
    {
        List<QuizInfo> quizList { get;  }
        void Add(QuizInfo _quiz);
        void Reset();
    }

    public class QuizContext : IQuizContext
    {
        public List<QuizInfo> quizList { get; private set; }
        public QuizContext()
        {
            quizList = new List<QuizInfo>();
        }
        public void Add(QuizInfo _quiz)
        {
            quizList.Add(_quiz);
        }
        public void Reset()
        {
            quizList.Clear();
        }    
    }

}
