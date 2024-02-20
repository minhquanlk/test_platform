namespace test_platform.Models.Quiz
{
    public class Answer
    {
        public string userID { get; set; }
        public string QuizID { get; set; }

        public Dictionary<string, Dictionary<string, string>> UserChoices { get; set; }

        public DateTime Start { get; set; }
        
        public DateTime End { get; set; }

        public int During { get; set; }
        public double Points { get; set; }
    }
}
