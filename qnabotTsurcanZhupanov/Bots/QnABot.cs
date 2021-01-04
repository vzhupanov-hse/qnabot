// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using QnABot.Bots;
using QnABot.Comands;
using QnABot.Images;
using QnABot.SystemMessages;
using QnABot.Users;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class QnABot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
    {
        protected readonly BotState ConversationState;  // Defines a state management object
        protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;  // Object of Base class for all Dialogs
        protected readonly BotState UserState;  // Defines a state management object
        protected SendImages send_image = new SendImages();  // Object creation 
        protected Start start = new Start();  // Object, which stores info about Commands
        protected UserInfo current_user;  // Object that gives us info about current user

        public QnABot(ConversationState conversationState, UserState userState, T dialog)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
        }

        /// <summary>
        /// Method handles an incoming Activity from user
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Activity</returns>
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //check what type of message was sent
            if (turnContext.Activity.Text == null)
                await turnContext.SendActivityAsync(MessageFactory.Text(Message.MessageType_Error), cancellationToken);
            else
            {
                RestartUser(turnContext, cancellationToken);
                //user definitions
                foreach (var item in Users.UsersList)
                    if (item.Id == turnContext.Activity.Recipient.Id) current_user = item;
                //checking the executable command
                CheckCommands(turnContext, current_user);
                //testing process implementation
                if (Check.Was_test)
                {
                    //sending the correct answer
                    if (turnContext.Activity.Text.ToLower() == "/prompt")
                    {
                        await send_image.SendRightReply(turnContext, cancellationToken, current_user);
                        await send_image.SendImageAsync(turnContext, cancellationToken, current_user);
                        //check if the user has passed all the questions
                        if (!SendImages.Was_ended)
                            await send_image.SendButtonAsync(turnContext, cancellationToken, current_user);
                        SendImages.Was_ended = false;
                    }
                    else
                    {
                        //validation of the answer
                        if (!SendImages.Was_answer)
                            await send_image.CheckReplyAsync(turnContext, cancellationToken, current_user);
                        //sending the next question if the previous one was correct
                        if (SendImages.Right_answer)
                        {
                            await send_image.SendImageAsync(turnContext, cancellationToken, current_user);
                            if (!SendImages.Was_ended)
                                await send_image.SendButtonAsync(turnContext, cancellationToken, current_user);
                            SendImages.Was_answer = false;
                            SendImages.Was_ended = false;
                        }
                    }
                }
                //sending a response from the knowledge base
                else
                {
                    if (turnContext.Activity.Text.ToLower() == "/prompt")
                        await turnContext.SendActivityAsync(MessageFactory.Text(Message.Prompt_Error), cancellationToken);
                    else
                        await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
                }
            }
        }

        /// <summary>
        /// Method greets new users
        /// </summary>
        /// <param name="membersAdded"></param>
        /// <param name="turnContext">activity received</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Activity(Task)</returns>
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(start.Welcome_message), cancellationToken);
                    //adding a new user to the user list
                    Users.AddUser(new UserInfo(turnContext.Activity.Recipient.Id));
                }
            }
        }

        /// <summary>
        /// Bot starts new dialog with the user
        /// </summary>
        /// <param name="turnContext">activity received</param>
        /// <param name="cancellationToken"></param>
        private void RestartUser(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.ToLower() == "/start")
            {
                int delete_user = -1;
                //search user in list
                for (int i = 0; i < Users.UsersList.Count; i++)
                    if (Users.UsersList[i].Id == turnContext.Activity.Recipient.Id) delete_user = i;
                //delete user from list
                if (delete_user != -1) Users.UsersList.Remove(Users.UsersList[delete_user]);
                //add user from list
                Users.AddUser(new UserInfo(turnContext.Activity.Recipient.Id));
                Check.Was_test = false;
                turnContext.SendActivityAsync(MessageFactory.Text(start.Welcome_message), cancellationToken);
            }
        }

        /// <summary>
        /// check if a command was entered
        /// </summary>
        /// <param name="turnContext">activity received</param>
        /// <param name="user">current user</param>
        private void CheckCommands(ITurnContext turnContext, UserInfo user)
        {
            if (turnContext.Activity.Text.ToLower() == "/test")
            {
                Check.Was_test = true;
                send_image = new SendImages();
            }
            if (turnContext.Activity.Text.ToLower() == "/answer")
            {
                Check.Was_test = false;
                SendImages.Was_answer = true;
                SendImages.Right_answer = true;
                if (user.Current != 0)
                    user.Current--;
            }
        }
    }
}
