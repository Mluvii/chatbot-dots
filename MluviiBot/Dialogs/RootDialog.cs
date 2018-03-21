
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MluviiBot.BotAssets.Dialogs;
using MluviiBot.BotAssets.Extensions;
using MluviiBot.BotAssets.Models;
using MluviiBot.Properties;

namespace MluviiBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly IDialogFactory dialogFactory;

        private Order order;
        private ConversationReference conversationReference;
        
        public RootDialog(IDialogFactory dialogFactory)
        {
            this.dialogFactory = dialogFactory;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await this.WelcomeMessageAsync(context);
        }

        private async Task WelcomeMessageAsync(IDialogContext context)
        {
            context.Call(this.dialogFactory.Create<MluviiDialog>(), this.AfterOrderCompleted);
        }

        private async Task AfterOrderCompleted(IDialogContext context, IAwaitable<Order> result)
        {
            context.EndConversation("0");
        }
    }
}