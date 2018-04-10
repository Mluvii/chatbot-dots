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
            var heroCard = GenerateHeroCard(title, subtitle, text, options, images);

            if (message.Attachments == null) message.Attachments = new List<Attachment>();

            message.Attachments.Add(heroCard.ToAttachment());
        }

        public static void AddHeroCard(this IMessageActivity message, string title, string subtitle, string text,
            IList<KeyValuePair<string, string>> options, IEnumerable<string> images = default(IEnumerable<string>))
        {
            var heroCard = GenerateHeroCard(title, subtitle, text, options, images);

            if (message.Attachments == null) message.Attachments = new List<Attachment>();

            message.Attachments.Add(heroCard.ToAttachment());
        }

        private static HeroCard GenerateHeroCard(string title, string subtitle, string text,
            IEnumerable<KeyValuePair<string, string>> options, IEnumerable<string> images)
        {
            var actions = new List<CardAction>();

            foreach (var option in options)
                actions.Add(new CardAction
                {
                    Title = option.Key,
                    Type = ActionTypes.ImBack,
                    Value = option.Value
                });

            var cardImages = new List<CardImage>();

            if (images != default(IEnumerable<string>))
                foreach (var image in images)
                    cardImages.Add(new CardImage
                    {
                        Url = image
                    });

            return new HeroCard(title, subtitle, text, images: cardImages, buttons: actions);
        }

        private static HeroCard GenerateHeroCard<T>(string title, string subtitle, string text, IEnumerable<T> options,
            IEnumerable<string> images)
        {
            return GenerateHeroCard(title, subtitle, text,
                options.Select(option => new KeyValuePair<string, string>(option.ToString(), option.ToString())),
                images);
        }
    }
}