using QnABot.Tools;

namespace QnABot.Comands
{
    public class Start : ITool
    {
        protected Test test = new Test();
        protected Answers answers = new Answers();
        protected Prompt prompt = new Prompt();

        public string Description { get; set; }
        public string CommandsName { get; set; }

        public string Welcome_message { get => "Здравствуйте, уважаемый пользователь!\n\nЯ - бот, который призван помочь Bам. На данный момент я могу:\n\n" +
                $"{answers.CommandsName} - {answers.Description}\n\n{test.CommandsName} - {test.Description}\n\n"+
                $"{prompt.CommandsName} - {prompt.Description}\n\n{CommandsName} - {Description}"; }

        public Start()
        {
            Description = "начать работу с ботом";
            CommandsName = "/start";
        }
    }
}
