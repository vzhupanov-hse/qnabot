using QnABot.Tools;

namespace QnABot.Comands
{
    public class Prompt : ITool
    {
        public string Description { get; set; }
        public string CommandsName { get; set; }

        public Prompt()
        {
            Description = "помощь при затруднении в процессе тестирования";
            CommandsName = "/prompt";
        }
    }
}
