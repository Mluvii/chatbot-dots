using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using MluviiBot.BotAssets.Dialogs;
using MluviiBot.BotAssets.Models;

#pragma warning disable 1998
namespace MluviiBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly IDialogFactory dialogFactory;

        public RootDialog(IDialogFactory dialogFactory)
        {
            this.dialogFactory = dialogFactory;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await WelcomeMessageAsync(context);
        }

        private async Task WelcomeMessageAsync(IDialogContext context)
        {
            context.Call(dialogFactory.Create<MluviiDialog>(), AfterOrderCompleted);
        }

        private async Task AfterOrderCompleted(IDialogContext context, IAwaitable<Order> result)
        {
            context.EndConversation("0");
        }
    }
}