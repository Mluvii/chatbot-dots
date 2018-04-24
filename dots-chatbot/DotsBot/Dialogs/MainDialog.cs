using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotsBot.BLL;
using DotsBot.BotAssets;
using DotsBot.BotAssets.Dialogs;
using DotsBot.BotAssets.Extensions;
using DotsBot.BotAssets.Models;
using DotsBot.Models;
using DotsBot.Properties;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;

#pragma warning disable 1998
namespace DotsBot.Dialogs
{
    public class MainDialog : IDialog
    {
        private const int MaxAttempts = 5;
        private readonly ICrmService crmService;
        private readonly DebugOptions debugOptions;
        private readonly IDialogFactory dialogFactory;
        private ConversationReference conversationReference;
        private CrmEntity crmEntity;
        private string personId;
        private readonly double? defaultInterestRate = 1.0;

        public MainDialog(IDialogFactory dialogFactory, ICrmService crmService, string personId,
            DebugOptions debugOptions = DebugOptions.None)
        {
            this.dialogFactory = dialogFactory;
            this.crmService = crmService;
            this.personId = personId;
            this.debugOptions = debugOptions;
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (personId == null) personId = Customer.DefaultPersonId;
            if (crmEntity == null) crmEntity = await crmService.GetCrmData(personId);
            if (crmEntity != null)
            {
                SetCallParams(context);
                var reply = context.MakeMessage();
                reply.AddHeroCard(
                    crmEntity.Product?.ProductName,
                    string.Format(Resources.ProductPrice, crmEntity.Product?.ProductPrice.Value.ToString("F")),
                    string.Format(Resources.WelcomeMessage_prompt, crmEntity.Salutation ?? crmEntity.Customer.FullName, $"<b>{crmEntity.Product?.ProductName}</b>"),
                    new[]
                    {
                        Resources.WelcomeMessage_operator,
                        Resources.MluviiDialog_virtual_assistant
                    },
                    crmEntity.Product?.ProductPhotoUrl != null ? new[] {crmEntity.Product.ProductPhotoUrl} : null);
                await context.PostAsync(reply);
                context.Wait(MessageReceivedAsync);
                
                return;
            }

            await context.SayAsync(Resources.CrmQueryFailed);
            var endreply = context.MakeMessage();
            endreply.AddHeroCard(
                "",
                "",
                string.Format(Resources.goodbye, crmEntity.Customer.Email, crmEntity.Product.ProductName),
                new[]
                {
                    Resources.HelpDialog_end,
                });
            await context.PostAsync(endreply);
            context.Wait(onFinished);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                await result;
            }
            catch (TooManyAttemptsException)
            {
                context.Call(dialogFactory.Create<HelpDialog, bool>(false), null);
                return;
            }

            var message = await result;
            //Some bug in PromptDialog.Choice causes message.Type to be null
            if (message.Text == null) //message.Type != ActivityTypes.Message)
            {
                context.Wait(MessageReceivedAsync);
                return;
            }

            if (conversationReference == null) conversationReference = message.ToConversationReference();

            if (message.Text.ContainsAnyIgnoreCaseAndAccents("operator", "clovek"))
            {
                await CheckAvailableOperators(context);
                return;
            }

            if (message.Text.ContainsAnyIgnoreCaseAndAccents("virtual", "asistent", "bot"))
            {
                await OnBotSelected(context);
                return;
            }

            await context.SayAsync(Resources.RetryText);
            await StartOver(context);
        }

        private async Task StartOver(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.AddHeroCard(
                crmEntity.Product?.ProductName,
                string.Format(Resources.ProductPrice, crmEntity.Product?.ProductPrice.Value.ToString("F")),
                string.Format(Resources.WelcomeMessage_prompt, crmEntity.Salutation ?? crmEntity.Customer.FullName, $"<b>{crmEntity.Product?.ProductName}</b>"),
                new[]
                {
                    Resources.WelcomeMessage_operator,
                    Resources.MluviiDialog_virtual_assistant
                },
                crmEntity.Product?.ProductPhotoUrl != null ? new[] {crmEntity.Product.ProductPhotoUrl} : null);
            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        private async Task OnBotSelected(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.AddHeroCard(
                "",
                "",
                Resources.MluviiDialog_instalments_prompt,
                new[] { 3, 6, 12, 24 });

            await context.PostAsync(reply);
            context.Wait(OnInstalmentsSelected);
        }

        private async Task OnInstalmentsSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                await result;
            }
            catch (TooManyAttemptsException)
            {
                context.Call(dialogFactory.Create<HelpDialog, bool>(false), null);
                return;
            }

            int instalmentCount;
            if (!int.TryParse((await result).Text, out instalmentCount) || instalmentCount < 2 || instalmentCount > 24)
            {
                await OnBotSelected(context);
                return;
            }
            var interest = crmEntity.Product.InterestRate;
            var emi = CalculateEmi(instalmentCount, crmEntity.Product.ProductPrice.Value, interest ?? defaultInterestRate.Value);
            var reply = context.MakeMessage();
            reply.AddHeroCard(
                string.Format(Resources.MluviiDialog_product_offer_title, DateTime.Now.ToString("yyyyddMMHHmmss")),
                string.Format(Resources.MluviiDialog_product_offer_subTitle, $"{interest}%"),
                string.Format(Resources.MluviiDialog_product_offer, 
                    $"<b>{crmEntity.Product.ProductName}</b>",
                    string.Format(Resources.ProductPrice, crmEntity.Product?.ProductPrice.Value.ToString("F")),
                    instalmentCount,
                    emi, 
                    interest),
                new[]
                {
                    Resources.MluviiDialog_product_offer_choice_sign_online,
                    Resources.MluviiDialog_product_offer_choice_not_tincans,
                });

            await context.PostAsync(reply);
            context.Wait(ProductOfferReacted);
        }

        private async Task ProductOfferReacted(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var choice = await result;

            if (choice.Text.ContainsAnyIgnoreCaseAndAccents("elektronicky", "online"))
            {
                await SignOnlineSelected(context);
                return;
            }

            if (choice.Text.ContainsAnyIgnoreCaseAndAccents("operator"))
            {
                await CheckAvailableOperators(context);
                return;
            }

            PromptDialog.Choice(context, async (dialogContext, subResult) =>
                {
                    var fakeMessage = dialogContext.MakeMessage();
                    fakeMessage.Text = await subResult;
                    await ProductOfferReacted(dialogContext, new AwaitableFromItem<IMessageActivity>(fakeMessage));
                },
                new[]
                {
                    Resources.MluviiDialog_product_offer_choice_sign_online,
                    Resources.MluviiDialog_product_offer_choice_not_tincans,
                },
                string.Format(Resources.RetryText),
                Resources.RetryText, MaxAttempts);
        }

        private async Task SignOnlineSelected(IDialogContext context, bool isRetry = false)
        {
            PromptDialog.Text(context, OnOnlineSigned, 
                isRetry ? Resources.MluviiDialog_product_offer_your_signature_here_retry : Resources.MluviiDialog_product_offer_your_signature_here);
        }

        private async Task OnOnlineSigned(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                await result;
            }
            catch (TooManyAttemptsException)
            {
                context.Call(dialogFactory.Create<HelpDialog, bool>(false), null);
                return;
            }

            var message = await result;

            if (message.ContainsIgnoreCaseAndAccents(crmEntity.Customer.FirstName) &&
                message.ContainsIgnoreCaseAndAccents(crmEntity.Customer.LastName))
            {
                var reply = context.MakeMessage();
                reply.AddHeroCard(
                    "",
                    "",
                    string.Format(Resources.MluviiDialog_product_offer_signed, crmEntity.Customer.Email, crmEntity.Product.ProductName),
                    new[]
                    {
                        Resources.HelpDialog_end,
                    });
                await context.PostAsync(reply);
                context.Wait(onFinished);
                return;
            }

            if (--crmEntity.Customer.SignAttempts < 1)
            {
                await CheckAvailableOperators(context, Resources.MluviiDialog_product_offer_sign_failed);
                return;
            }

            await SignOnlineSelected(context, true);
        }

        private decimal CalculateEmi(long instalmentCount, decimal orderProductPrice, double interest)
        {
            var numberOfPayments = instalmentCount;

            var rateOfInterest = interest / (numberOfPayments * 100);
            var paymentAmount = rateOfInterest * (double) orderProductPrice /
                                (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));

            return decimal.Round((decimal) paymentAmount, 2);
        }


        private async Task CheckAvailableOperators(IDialogContext context, string text = null)
        {
            await context.SayAsync(text ?? Resources.MluviiDialog_wait_checking_available_operators);
            context.Call(dialogFactory.Create<AvailibleOperatorsDialog>(), OnAvailibleOperatorsResponse);
        }

        private async Task OnAvailibleOperatorsResponse(IDialogContext context,
            IAwaitable<AvailableOperatorInfo> result)
        {
            var selectedOperator = await result;

            if (selectedOperator == null)
            {
                PromptDialog.Confirm(context, OnStartOverSelected, Resources.MluviiDialog_operator_failed, Resources.RetryText, MaxAttempts);
                return;
            }

            await ConnectToOperator(context, string.Format(Resources.OperatorConnect_wait, selectedOperator.DisplayName), selectedOperator.UserId);
        }

        private async Task OnStartOverSelected(IDialogContext context, IAwaitable<bool> result)
        {
            try
            {
                await result;
            }
            catch (TooManyAttemptsException)
            {
                context.Call(dialogFactory.Create<HelpDialog, bool>(false), null);
                return;
            }

            var response = await result;

            if (response)
            {
                await StartOver(context);
                return;
            }

            var reply = context.MakeMessage();
            reply.AddHeroCard(
                "",
                "",
                string.Format(Resources.goodbye, crmEntity.Customer.Email, crmEntity.Product.ProductName),
                new[]
                {
                    Resources.HelpDialog_end,
                });
            await context.PostAsync(reply);
            context.Wait(onFinished);
        }

        private async Task ConnectToOperator(IDialogContext context, string message, int? userID = null)
        {
            var data = JObject.Parse(@"{ ""Activity"": ""Forward"" }");
            if (userID != null) data.Add("UserId", userID.Value);

            var act = context.MakeMessage();
            act.ChannelData = data;
            act.Text = message;
            await context.PostAsync(act);
        }

        private async Task AskCallParams(IDialogContext context)
        {
            var data = JObject.Parse(@"{ ""Activity"": ""GetCallParams"" }");

            var act = context.MakeMessage();
            act.ChannelData = data;
            await context.PostAsync(act);
        }


        private async void SetCallParams(IDialogContext context)
        {
            var dict = new Dictionary<string, string>
            {
                {ClientCallPredefParam.GUEST_IDENTITY, crmEntity.Customer.FullName},
                {ClientCallPredefParam.GUEST_EMAIL, crmEntity.Customer.Email},
                {ClientCallPredefParam.GUEST_PHONE, crmEntity.Customer.Phone}
            };
            var callParams = JObject.FromObject(dict);
            var data = JObject.Parse(@"{ ""Activity"": ""SetCallParams"" }");
            data.Add("CallParams", callParams);

            var act = context.MakeMessage();
            act.ChannelData = data;
            await context.PostAsync(act);
        }

        private async Task onFinished(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);
        }
    }
}