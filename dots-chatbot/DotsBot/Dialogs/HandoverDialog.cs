using System.Threading.Tasks;
using DotsBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;

namespace DotsBot.Dialogs
{
    public class HandoverDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            await ConnectToOperator(context, Resources.OperatorConnect_wait);
        }

        private async Task ConnectToOperator(IDialogContext context, string message)
        {
            var data = JObject.Parse(@"{ ""Activity"": ""Forward"" }");
            var act = context.MakeMessage();
            act.ChannelData = data;
            act.Text = message;
            await context.PostAsync(act);
        }
    }
}