using test_platform.Models.Quiz;

namespace test_platform.Services
{
    public interface IHistoryContext
    {
        Answer History { get; }
        void SetHistory(Answer history);
    }

    public class HistoryContext : IHistoryContext
    {
        public Answer History { get; private set; }

        public void SetHistory(Answer history)
        {
            History = history;
        }
    }

}
