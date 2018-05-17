using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FinanceBot.BLL;
using FinanceBot.BotAssets;
using FinanceBot.BotAssets.Dialogs;
using FinanceBot.BotAssets.Extensions;
using FinanceBot.BotAssets.Models;
using FinanceBot.Properties;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;

#pragma warning disable 1998
namespace FinanceBot.Dialogs
{
    public class MainDialog : IDialog
    {
        private const int MaxAttempts = 5;
        private readonly ICrmService crmService;
        private readonly IDialogFactory dialogFactory;
        private ConversationReference conversationReference;
        private CrmEntity crmEntity;
        private string personId;

        public MainDialog(IDialogFactory dialogFactory, ICrmService crmService, string personId)
        {
            this.dialogFactory = dialogFactory;
            this.crmService = crmService;
            this.personId = personId;
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (personId == null) personId = Customer.DefaultPersonId;
            if (crmEntity == null)
                crmEntity = await crmService.GetCrmData(personId, Thread.CurrentThread.CurrentCulture);
            if (crmEntity != null)
            {
                SetCallParams(context);
                await PostWelcomeCard(context);
                context.Wait(MessageReceivedAsync);

                return;
            }

            await context.SayAsync(Resources.CrmQueryFailed);
            var endreply = context.MakeMessage();
            endreply.AddHeroCard(
                "",
                "",
                string.Format(Resources.goodbye),
                new[]
                {
                    Resources.HelpDialog_end
                });
            await context.PostAsync(endreply);
            context.Wait(onFinished);
        }

        private async Task PostWelcomeCard(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.AddThumbnailCard(
                crmEntity.Product?.ProductName,
                "",
                string.Format(Resources.WelcomeMessage_prompt, crmEntity.Salutation ?? crmEntity.Customer.FullName,
                    $"<b>{crmEntity.InsuranceOffer.YearlyDiscountRate}</b>",
                    $"<b>{crmEntity.Product?.ProductName}</b>"),
                new[]
                {
                    Resources.WelcomeMessage_operator,
                    Resources.MluviiDialog_virtual_assistant
                },
                crmEntity.Product?.ProductPhotoUrl);
            await context.PostAsync(reply);
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
            if (message.Text == null)
            {
                context.Wait(MessageReceivedAsync);
                return;
            }

            if (conversationReference == null) conversationReference = message.ToConversationReference();

            if (message.Text.ContainsAnyIgnoreCaseAndAccents(Resources.WelcomeMessage_operator, "clovek"))
            {
                await CheckAvailableOperators(context);
                return;
            }

            if (message.Text.ContainsAnyIgnoreCaseAndAccents(Resources.MluviiDialog_virtual_assistant, "virtual",
                "asistent", "bot", "assistant"))
            {
                await OnBotSelected(context);
                return;
            }

            await context.SayAsync(Resources.RetryText);
            context.Wait(MessageReceivedAsync);
        }

        private async Task StartOver(IDialogContext context)
        {
            await PostWelcomeCard(context);
            context.Wait(MessageReceivedAsync);
        }

        private async Task OnBotSelected(IDialogContext context)
        {
            PromptDialog.Choice(context, OnUsageSelected,
                new[]
                {
                    Resources.MluviiDialog_usage_personal,
                    Resources.MluviiDialog_usage_work
                },
                Resources.MluviiDialog_usage_prompt,
                Resources.RetryText);
        }

        private async Task OnUsageSelected(IDialogContext context, IAwaitable<string> result)
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

            PromptDialog.Choice(context, OnCoverageSelected,
                new PromptOptionsWithSynonyms<string>(
                    string.Format(Resources.MluviiDialog_coverage_prompt, $"<b>{crmEntity.Product?.ProductName}</b>"),
                    Resources.RetryText,
                    Resources.TooManyAttempts,
                    new Dictionary<string, IReadOnlyList<string>>
                    {
                        {
                            Resources.MluviiDialog_coverage_device_only,
                            new[] {"only"}
                        },
                        {
                            Resources.MluviiDialog_coverage_device_and_display,
                            new[] {"display"}
                        }
                    }));
        }

        private async Task OnCoverageSelected(IDialogContext context, IAwaitable<string> result)
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

            var choice = await result;

            if (choice.ContainsAnyIgnoreCaseAndAccents(Resources.MluviiDialog_coverage_device_and_display,
                "display"))
            {
                crmEntity.InsuranceSelection =
                    new InsuranceSelection {SelectedCoveragePlan = PlanTypes.DeviceAndDisplay};
            }
            else if (choice.ContainsAnyIgnoreCaseAndAccents(Resources.MluviiDialog_coverage_device_only, "only"))
            {
                crmEntity.InsuranceSelection = new InsuranceSelection {SelectedCoveragePlan = PlanTypes.DeviceOnly};
            }
//            else
//            {
//                await context.SayAsync(Resources.RetryText);
//                context.Wait(OnCoverageSelected);
//                return;
//            }

            PromptDialog.Choice(context, OnPaymentPlanSelected,
                new PromptOptionsWithSynonyms<string>(
                    Resources.MluviiDialog_price_prompt,
                    Resources.RetryText,
                    Resources.TooManyAttempts,
                    new Dictionary<string, IReadOnlyList<string>>
                    {
                        {
                            string.Format(Resources.MluviiDialog_price_monthly,
                                GetPrice(crmEntity.InsuranceOffer, crmEntity.InsuranceSelection.SelectedCoveragePlan, BillingPeriods.Monthly)),
                            new[] {"monthly"}
                        },
                        {
                            string.Format(Resources.MluviiDialog_price_yearly, 
                                GetPrice(crmEntity.InsuranceOffer, crmEntity.InsuranceSelection.SelectedCoveragePlan,BillingPeriods.Yearly),
                                crmEntity.InsuranceOffer.YearlyDiscountRate),
                            new[] {"yearly"}
                        }
                    }));
        }


        private async Task OnPaymentPlanSelected(IDialogContext context, IAwaitable<string> result)
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

            if (message.ContainsAnyIgnoreCaseAndAccents(Resources.MluviiDialog_price_monthly, "monthly"))
            {
                crmEntity.InsuranceSelection.SelectedBillingPlan = BillingPeriods.Monthly;
                crmEntity.InsuranceSelection.FinalPrice = GetPrice(crmEntity.InsuranceOffer,
                    crmEntity.InsuranceSelection.SelectedCoveragePlan, BillingPeriods.Monthly);
            }

            if (message.ContainsAnyIgnoreCaseAndAccents(Resources.MluviiDialog_price_yearly, "yearly"))
            {
                crmEntity.InsuranceSelection.SelectedBillingPlan = BillingPeriods.Yearly;
                crmEntity.InsuranceSelection.FinalPrice = GetPrice(crmEntity.InsuranceOffer,
                    crmEntity.InsuranceSelection.SelectedCoveragePlan, BillingPeriods.Yearly);
            }

            PromptDialog.Choice(context, OnSignConfirmed,
                new PromptOptionsWithSynonyms<string>(
                    string.Format(Resources.MluviiDialog_sign_prompt, crmEntity.InsuranceOffer.YearlyDiscountRate),
                    Resources.RetryText,
                    Resources.TooManyAttempts,
                    new Dictionary<string, IReadOnlyList<string>>
                    {
                        {
                            Resources.MluviiDialog_sign_button,
                            new[] {"sign"}
                        }
                    }));
        }

        private async Task OnSignConfirmed(IDialogContext context, IAwaitable<string> result)
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

            var endreply = context.MakeMessage();
            endreply.AddHeroCard(
                "",
                "",
                "",
                new[]
                {
                    string.Format(Resources.MluviiDialog_pay_button, crmEntity.InsuranceSelection.FinalPrice)
                });
            await context.PostAsync(endreply);
            context.Wait(OnPayConfirmed);
        }

        private async Task OnPayConfirmed(IDialogContext context, IAwaitable<IMessageActivity> result)
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
            if (response.Text.ContainsAnyIgnoreCaseAndAccents(Resources.MluviiDialog_pay_button, "pay"))
            {
                await context.SayAsync(Resources.MluviiDialog_phishing_card);
                context.Wait(OnPayConfirmSuccess);
                return;
            }
            
            await context.SayAsync(Resources.RetryText);
            context.Wait(OnPayConfirmed);
        }

        private async Task OnPayConfirmSuccess(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var cardNumber = message.Text.Replace(" ", "").Replace("-", "");
            if (RegexConstants.CardNumber.IsMatch(cardNumber))
            {
                await context.SayAsync(Resources.MluviiDialog_phishing_exp);
                context.Wait(OnExpPhished);
                return;
            }

            await context.SayAsync(Resources.MluviiDialog_phishing_card_wrong);
            context.Wait(OnPayConfirmSuccess);
        }

        private async Task OnExpPhished(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (RegexConstants.Expiration.IsMatch(message.Text.Trim()))
            {
                await context.SayAsync(Resources.MluviiDialog_phishing_cvv);
                context.Wait(OnCvvPhished);
                return;
            }

            await context.SayAsync(Resources.MluviiDialog_phishing_exp_wrong);
            context.Wait(OnExpPhished);
        }

        private async Task OnCvvPhished(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var plan = crmEntity.InsuranceOffer.Plans.Single(p =>
                p.PlanType == crmEntity.InsuranceSelection.SelectedCoveragePlan);
            
            var message = await result;
            if (!RegexConstants.Cvv.IsMatch(message.Text.Trim()))
            {
                await context.SayAsync(Resources.MluviiDialog_phishing_cvv_wrong);
                context.Wait(OnCvvPhished);
                return;
            }

            var reply = context.MakeMessage();
            reply.AddHeroCard(
                "",
                "",
                string.Format(Resources.MluviiDialog_phishing_successful, DateTime.Now.ToString("yyyyddMMHHmmss"),
                    plan.PriceMonthly),
                new[]
                {
                    Resources.HelpDialog_end
                });
            await context.PostAsync(reply);
            context.Wait(onFinished);
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
                PromptDialog.Confirm(context, OnStartOverSelected, Resources.MluviiDialog_operator_failed,
                    Resources.RetryText, MaxAttempts);
                return;
            }

            await ConnectToOperator(context,
                string.Format(Resources.OperatorConnect_wait, selectedOperator.DisplayName), selectedOperator.UserId);
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
                    Resources.HelpDialog_end
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

        private decimal GetPrice(InsuranceOffer offer, PlanTypes planType, BillingPeriods period)
        {
            var plan = offer.Plans.Single(p => p.PlanType == planType);
            switch (period)
            {
                case BillingPeriods.Monthly:
                    return plan.PriceMonthly;
                case BillingPeriods.Yearly:
                    return Math.Round(12 * plan.PriceMonthly * (decimal.One - offer.YearlyDiscountRate / 100));
                default:
                    throw new ArgumentOutOfRangeException(nameof(period), period, null);
            }
        }
    }
}
