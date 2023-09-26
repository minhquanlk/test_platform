using test_platform.Models.Quiz;
using test_platform.Models.Auth;

namespace test_platform.Models
{
    public class TestView
    {
        public UserInfo user { get; set; } 
        public List<QuizInfo> quizs { get; set; }
    }
}
