using System;
using System.Threading.Tasks;
using DotsBot.BotAssets;
using DotsBot.BotAssets.Dialogs;
using DotsBot.BotAssets.Models;
using DotsBot.Models;
using DotsBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotsBot.Dialogs
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
                    var callParamsResponse =
                        JsonConvert.DeserializeObject<GetCallParamsResponse>(activity.ChannelData.ToString());
                    var personId =
                        callParamsResponse.CallParams.ValueOrDefault(ClientCallPredefParam.FACE_API_PERSON_ID);
                    if (personId != null)
                    {
                        context.Call(dialogFactory.Create<MainDialog, string>(personId), null);
                        return;
                    }
                }
                catch (Exception)
                {
                    context.Call(dialogFactory.Create<MainDialog, string>(null), null);
                    return;
                }

            if (maxAttempts > 0)
            {
                maxAttempts--;
                await AskCallParams(context);
                context.Wait(OnMessageRecieved);
                return;
            }

            context.Call(dialogFactory.Create<MainDialog, string>(null), null);
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