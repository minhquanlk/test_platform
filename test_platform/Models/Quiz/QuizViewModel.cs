namespace test_platform.Models.Quiz
{
    public class QuizViewModel
    {
        public List<QuizInfo> QuizInfoList { get; set; }
        public Answer Answer { get; set; }
        public string userID { get; set; }
        public int Minutes { get; set; }

        public int Seconds { get; set; }
    }

}
