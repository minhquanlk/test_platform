using FireSharp.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using test_platform.Models.Auth;
using test_platform.Models.Quiz;

namespace test_platform.Services
{
    public interface IQuizContext
    {
        List<QuizInfo> quizList { get;  }
        void Add(QuizInfo _quiz);
        void Reset();
        void Shuffle();
        List<QuizInfo> getQuizList();
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
        public List<QuizInfo> getQuizList()
        {
            return quizList;
        }
        public void Reset()
        {
            quizList.Clear();
        }    
        public void Shuffle()
        {
            var rnd = new Random();
            int n = quizList.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                QuizInfo value = quizList[k];
                quizList[k] = quizList[n];
                quizList[n] = value;
            }

        }
    }

}
