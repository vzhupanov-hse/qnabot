using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnABot.SystemMessages
{
    public class Message
    {
        public const string default_message = "Пока я не могу ответить вам на этот вопрос, но в скором времени обещаю исправиться!";

        public static string Prompt_Error { get => "Данная команда доступна только в разделе тестирования!"; }

        public static string MessageType_Error { get => "К сожалению я пока поддерживаю только текстовые запросы"; }
    }
}
