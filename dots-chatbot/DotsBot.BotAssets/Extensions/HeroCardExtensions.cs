using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Connector;

namespace DotsBot.BotAssets.Extensions
{
    public static class HeroCardExtensions
    {
        public static void AddHeroCard<T>(this IMessageActivity message, string title, string subtitle, string text,
            IEnumerable<T> options, IEnumerable<string> images = default(IEnumerable<string>))
        {
            var actions = GetActions(options);
            var cardImages = GetImages(images);
            if (message.Attachments == null) message.Attachments = new List<Attachment>();

            var heroCard = new HeroCard(title, subtitle, text, images: cardImages, buttons: actions);
            message.Attachments.Add(heroCard.ToAttachment());
        }
        
        public static void AddThumbnailCard<T>(this IMessageActivity message, string title, string subtitle, string text,
            IEnumerable<T> options, string image = default(string))
        {
            var actions = GetActions(options);
            var cardImages = GetImages(new[] {image});
            if (message.Attachments == null) message.Attachments = new List<Attachment>();

            var heroCard = new ThumbnailCard(title, subtitle, text, images: cardImages, buttons: actions);
            message.Attachments.Add(heroCard.ToAttachment());
        }

        private static List<CardAction> GetActions(IEnumerable<KeyValuePair<string, string>> options)
        {
            var actions = new List<CardAction>();

            foreach (var option in options)
                actions.Add(new CardAction
                {
                    Title = option.Key,
                    Type = ActionTypes.ImBack,
                    Value = option.Value
                });
            return actions;
        }

        private static List<CardAction> GetActions<T>(IEnumerable<T> options)
        {
            return GetActions(options.Select(option => new KeyValuePair<string, string>(option.ToString(), option.ToString())));
        }

        private static List<CardImage> GetImages(IEnumerable<string> images)
        {
            var cardImages = new List<CardImage>();

            if (images != default(IEnumerable<string>))
                foreach (var image in images)
                    cardImages.Add(new CardImage
                    {
                        Url = image
                    });
            return cardImages;
        }
    }
}
