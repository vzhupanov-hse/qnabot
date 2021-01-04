using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using QnABot.Users;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QnABot.Images
{
    public class SendImages
    {
        public static bool Right_answer { get; set; }
        public static bool Was_answer { get; set; }
        public static bool Was_ended { get; set; }

        Parser images;
        Activity reply;

        public SendImages()
        {
            images = new Parser();
        }

        /// <summary>
        /// send image to user
        /// </summary>
        /// <param name="turnContext">recieved activity</param>
        /// <param name="cancellationToken"></param>
        /// <param name="user">current user</param>
        /// <returns></returns>
        public async Task SendImageAsync(ITurnContext turnContext, CancellationToken cancellationToken, UserInfo user)
        {
            try
            {
                var reply = MessageFactory.Attachment(GetAttachment(user));
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            //if all tests were done
            catch (ImageException ex)
            {
                user.Current = 0;
                Activity message = MessageFactory.Text(ex.Message);
                await turnContext.SendActivityAsync(message, cancellationToken);
            }
        }

        /// <summary>
        /// check if reply waas right
        /// </summary>
        /// <param name="turnContext">recieved activity</param>
        /// <param name="cancellationToken"></param>
        /// <param name="user">current user</param>
        /// <returns></returns>
        public async Task CheckReplyAsync(ITurnContext turnContext, CancellationToken cancellationToken, UserInfo user)
        {
            if (user.Current == 0)
                Right_answer = true;
            else
            {
                if (turnContext.Activity.Text == images.Images[user.Current - 1].Right_answer)
                {
                    //send message
                    reply = MessageFactory.Text("Ответ верный!");
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    Right_answer = true;
                }
                else
                {
                    //send message
                    reply = MessageFactory.Text("Ответ неверный!\n\nДля получения подсказки: /prompt");
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    Right_answer = false;
                }
            }
        }

        public async Task SendRightReply(ITurnContext turnContext, CancellationToken cancellationToken, UserInfo user)
        {
            reply = MessageFactory.Text("Правильный ответ: " + images.Images[user.Current - 1].Right_answer);
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        public async Task SendButtonAsync(ITurnContext turnContext, CancellationToken cancellationToken, UserInfo user)
        {
            //create herocard with button
            var heroCard = new HeroCard
            {
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Открыть изображение в источнике", value: images.Images[user.Current++].Web_path) }
            };
            var reply = MessageFactory.Attachment(heroCard.ToAttachment());
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        /// <summary>
        /// create attachment
        /// </summary>
        /// <param name="user">current user</param>
        /// <returns>attachment with image</returns>
        private Attachment GetAttachment(UserInfo user)
        {
            if (user.Current < images.Images.Count)
            {
                var attachment = new Attachment
                {
                    ContentUrl = images.Images[user.Current].Image_path,
                    ContentType = "image/png",
                    Name = images.Images[user.Current].Number
                };
                return attachment;
            }
            else
            {
                Was_ended = true;
                throw new ImageException("Вы прорешали все доступные задания!\n\nЧтобы начать заново: /test\n\nЧтобы перейти в режим ответов на вопросы: /answer");
            }
        }
    }
}

