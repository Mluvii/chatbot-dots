using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceBot.BotAssets.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace FinanceBot.BotAssets.Dialogs
{
    [Serializable]
    public abstract class PagedCarouselDialog<T> : IDialog<T>
    {
        private int pageNumber = 1;
        private readonly int pageSize = 5;

        public virtual string Prompt { get; }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(Prompt ?? Resources.PagedCarouselDialog_DefaultPrompt);

            await ShowProducts(context);

            context.Wait(MessageReceivedAsync);
        }

        public abstract PagedCarouselCards GetCarouselCards(int pageNumber, int pageSize);

        public abstract Task ProcessMessageReceived(IDialogContext context, string message);

        protected async Task ShowProducts(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = new List<Attachment>();

            var productsResult = GetCarouselCards(pageNumber, pageSize);
            foreach (var productCard in productsResult.Cards) reply.Attachments.Add(productCard.ToAttachment());

            await context.PostAsync(reply);

            if (productsResult.TotalCount > pageNumber * pageSize) await ShowMoreOptions(context);
        }

        protected async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            // TODO: validation
            if (message.Text.Equals(Resources.PagedCarouselDialog_ShowMe, StringComparison.InvariantCultureIgnoreCase))
            {
                pageNumber++;
                await StartAsync(context);
            }
            else
            {
                await ProcessMessageReceived(context, message.Text);
            }
        }

        private async Task ShowMoreOptions(IDialogContext context)
        {
            var moreOptionsReply = context.MakeMessage();
            moreOptionsReply.Attachments = new List<Attachment>
            {
                new HeroCard
                {
                    Text = Resources.PagedCarouselDialog_MoreOptions,
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, Resources.PagedCarouselDialog_ShowMe,
                            value: Resources.PagedCarouselDialog_ShowMe)
                    }
                }.ToAttachment()
            };

            await context.PostAsync(moreOptionsReply);
        }

        public class PagedCarouselCards
        {
            public IEnumerable<HeroCard> Cards { get; set; }

            public int TotalCount { get; set; }
        }
    }
}