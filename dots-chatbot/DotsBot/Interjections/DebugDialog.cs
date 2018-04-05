using System;
using System.Threading.Tasks;
using DotsBot.BotAssets.Dialogs;
using DotsBot.Models;
using Microsoft.Bot.Builder.Dialogs;

#pragma warning disable 1998

namespace DotsBot.Dialogs
{
    public class DebugDialog : IDialog
    {
        private readonly IDialogFactory dialogFactory;

        public DebugDialog(IDialogFactory dialogFactory)
        {
            this.dialogFactory = dialogFactory;
        }

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(context, async (dialogContext, result) =>
                {
                    var choice = await result;
                    DebugOptions selected;
                    Enum.TryParse<DebugOptions>(choice, out selected);
                    var dialog = dialogFactory.Create<MainDialog, DebugOptions>(selected);
                    dialogContext.Call(dialog, null);
                },
                new[] {"GotoFinalConfirmation", "GotoOperatorSearch", "GotoMap"},
                "DEBUG MENU");
        }
    }
}