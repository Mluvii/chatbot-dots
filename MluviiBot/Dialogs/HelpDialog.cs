using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using MluviiBot.BotAssets.Dialogs;
using MluviiBot.Properties;

#pragma warning disable 1998
namespace MluviiBot.Dialogs
{
    [Serializable]
    public class HelpDialog : IDialog
    {
        private readonly bool canGoBack;
        private readonly IDialogFactory dialogFactory;

        public HelpDialog(IDialogFactory dialogFactory, bool canGoBack = true)
        {
            this.canGoBack = canGoBack;
            SetField.NotNull(out this.dialogFactory, nameof(dialogFactory), dialogFactory);
        }

        public async Task StartAsync(IDialogContext context)
        {
            var preferencesOptions = new[]
            {
                Resources.HelpDialog_start_over,
                Resources.HelpDialog_connect_operator,
                Resources.HelpDialog_end,
                canGoBack ? Resources.CancellableDialog_back : ""
            }.Except(new[] {""});

            CancelablePromptChoice<string>.Choice(
                context,
                ResumeAfterOptionSelected,
                preferencesOptions,
                Resources.HelpDialog_Prompt);
        }

        private async Task ResumeAfterOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            var option = await result;

            if (option == null)
            {
                context.Done<object>(null);
                return;
            }

            if (option.Equals(Resources.HelpDialog_start_over, StringComparison.OrdinalIgnoreCase))
            {
                context.Call(dialogFactory.Create<IDialog<object>>(), null);
                return;
            }

            if (option.Equals(Resources.HelpDialog_connect_operator, StringComparison.OrdinalIgnoreCase))
            {
                context.Call(dialogFactory.Create<HandoverDialog>(), null);
                return;
            }

            if (option.Equals(Resources.HelpDialog_end, StringComparison.OrdinalIgnoreCase))
            {
                await context.SayAsync(Resources.goodbye);
                return;
            }

            await StartAsync(context);
        }
    }
}