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
                    "",
                    "",
                    string.Format(Resources.WelcomeMessage_prompt, crmEntity.Salutation ?? crmEntity.Customer.FullName, crmEntity.Product?.ProductName),
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
            await context.SayAsync(Resources.goodbye);
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

            StartOver(context);
        }

        private void StartOver(IDialogContext context)
        {
            PromptDialog.Choice(context, async (dialogContext, subResult) =>
                {
                    var fakeMessage = dialogContext.MakeMessage();
                    fakeMessage.Text = await subResult;
                    await MessageReceivedAsync(dialogContext, new AwaitableFromItem<IMessageActivity>(fakeMessage));
                },
                new[] {Resources.WelcomeMessage_operator, Resources.MluviiDialog_virtual_assistant},
                string.Format(Resources.WelcomeMessage_prompt, crmEntity.Salutation ?? crmEntity.Customer.FullName, crmEntity.Product?.ProductName),
                Resources.RetryText, MaxAttempts);
        }

        private async Task OnBotSelected(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.AddHeroCard(
                "",
                "",
                Resources.MluviiDialog_instalments_prompt,
                new[] { 4, 6, 12 });

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
            if (!int.TryParse((await result).Text, out instalmentCount) || instalmentCount < 2 || instalmentCount > 120)
            {
                await OnBotSelected(context);
                return;
            }
            var interest = crmEntity.Product.InterestRate;
            var emi = CalculateEmi(instalmentCount, crmEntity.Product.ProductPrice ?? new decimal(399), interest ?? defaultInterestRate.Value);
            var reply = context.MakeMessage();
            reply.AddHeroCard(
                "",
                "",
                string.Format(Resources.MluviiDialog_product_offer, crmEntity.Product.ProductName,
                    crmEntity.Product.ProductPrice, emi, interest),
                new[]
                {
                    Resources.MluviiDialog_product_offer_choice_sign_online,
                    Resources.MluviiDialog_product_offer_choice_not_tincans,
                    Resources.MluviiDialog_product_offer_choice_offer_no_good
                },
                new[] {crmEntity.Product.ProductPhotoUrl});

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

            if (choice.Text.ContainsAnyIgnoreCaseAndAccents("nesouhlasim", "urok"))
            {
                await context.SayAsync(Resources.MluviiDialog_product_offer_choice_offer_no_good_selected);
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
                    Resources.MluviiDialog_product_offer_choice_offer_no_good
                },
                string.Format(Resources.RetryText),
                Resources.RetryText, MaxAttempts);
        }

        private async Task SignOnlineSelected(IDialogContext context)
        {
            PromptDialog.Text(context, OnOnlineSigned, Resources.MluviiDialog_product_offer_your_signature_here);
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
                await context.SayAsync(string.Format(Resources.MluviiDialog_product_offer_signed, crmEntity.Customer.Email, crmEntity.Product.ProductName));
                context.Wait(onFinished);
                return;
            }

            if (crmEntity.Customer.SignAttempts-- < 1)
            {
                await CheckAvailableOperators(context);
                return;
            }

            await SignOnlineSelected(context);
        }

        private decimal CalculateEmi(long instalmentCount, decimal orderProductPrice, double interest)
        {
            var numberOfPayments = instalmentCount;

            var rateOfInterest = interest / (numberOfPayments * 100);
            var paymentAmount = rateOfInterest * (double) orderProductPrice /
                                (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));

            return decimal.Round((decimal) paymentAmount, 2);
        }


        private async Task CheckAvailableOperators(IDialogContext context)
        {
            await context.SayAsync(Resources.MluviiDialog_wait_checking_available_operators);
            context.Call(dialogFactory.Create<AvailibleOperatorsDialog>(), OnAvailibleOperatorsResponse);
        }

        private async Task OnAvailibleOperatorsResponse(IDialogContext context,
            IAwaitable<AvailableOperatorInfo> result)
        {
            var selectedOperator = await result;

            if (selectedOperator == null)
            {
                PromptDialog.Confirm(context, OnStartOverSelected, Resources.MluviiDialog_operator_failed,
                    Resources.RetryText, MaxAttempts);
                return;
            }

            await ConnectToOperator(context, Resources.OperatorConnect_wait, selectedOperator.UserId);
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

            if (response) StartOver(context);

            await context.SayAsync(Resources.goodbye);
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