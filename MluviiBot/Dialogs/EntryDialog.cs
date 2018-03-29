
using System;
using System.Threading.Tasks;
using iCord.OnifWebLib.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MluviiBot.BotAssets;
using MluviiBot.BotAssets.Dialogs;
using MluviiBot.BotAssets.Models;
using MluviiBot.Models;
using MluviiBot.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MluviiBot.Dialogs
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
            {
                try
                {
                    var callParamsResponse = JsonConvert.DeserializeObject<GetCallParamsResponse>(activity.ChannelData.ToString());
                    var personId = callParamsResponse.CallParams.ValueOrDefault(ClientCallPredefParam.FACE_API_PERSON_ID);
                    if (personId != null)
                    {
                        context.Call(dialogFactory.Create<MluviiDialog, string>(personId), null);
                        return;
                    }
                }
                catch (Exception)
                {
                    context.Done<GetCallParamsResponse>(null);
                    return;
                }
            }
            if (maxAttempts > 0)
            {
                maxAttempts--;
                await AskCallParams(context);
                context.Wait(OnMessageRecieved);
                return;
            }

            await context.SayAsync(Resources.OperatorSelection_none_availible);
            context.Done<AvailableOperatorInfo>(null);
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