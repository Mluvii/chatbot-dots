using System;
using System.Threading.Tasks;
using FinanceBot.BotAssets.Extensions;
using FinanceBot.BotAssets;
using FinanceBot.BotAssets.Dialogs;
using FinanceBot.BotAssets.Models;
using iCord.OnifWebLib.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FinanceBot.Dialogs
{
    public class EntryDialog : IDialog
    {
        private readonly IDialogFactory dialogFactory;
        private int maxAttempts = 3;

        public EntryDialog(IDialogFactory dialogFactory)
        {
            this.dialogFactory = dialogFactory;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await AskCallParams(context);
            context.Wait(OnMessageRecieved);
        }

        private async Task OnMessageRecieved(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            if (activity.AsEventActivity() != null && activity.ChannelData != null)
                try
                {
                    var callParamsResponse = JsonConvert.DeserializeObject<GetCallParamsResponse>((string)activity.ChannelData.ToString());
                    var personId = callParamsResponse.CallParams.ValueOrDefault(ClientCallPredefParam.GUEST_IDENTITY);
                    if (personId != null)
                    {
                        personId = personId.Replace(" ", "").RemoveDiacritics();
                        context.Call(dialogFactory.Create<MainDialog, string>(personId), onFinished);
                        return;
                    }
                }
                catch (Exception)
                {
                    context.Call(dialogFactory.Create<MainDialog, string>(null), onFinished);
                    return;
                }

            if (maxAttempts > 0)
            {
                maxAttempts--;
                await AskCallParams(context);
                context.Wait(OnMessageRecieved);
                return;
            }

            context.Call(dialogFactory.Create<MainDialog, string>(null), onFinished);
        }

        private async Task onFinished(IDialogContext context, IAwaitable<object> result)
        {
            context.EndConversation("0");
        }

        private async Task AskCallParams(IDialogContext context)
        {
            var data = JObject.Parse(@"{ ""Activity"": ""GetCallParams"" }");
            var act = context.MakeMessage();
            act.ChannelData = data;
            await context.PostAsync(act);
        }
    }
}
