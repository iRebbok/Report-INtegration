using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Constructor;
using EXILED;
using System;

namespace ServerReports
{
    public static class WebhookHelper
    {
        private static readonly EmbedBuilder embedBuilder;
        private static readonly EmbedFieldBuilder fieldBuilder;
        private static readonly MessageBuilder messageBuilder;
        private static readonly MessageMentionBuilder mentionBuilder;

        static WebhookHelper()
        {
            embedBuilder = ConstructorProvider.GetEmbedBuilder();
            fieldBuilder = ConstructorProvider.GetEmbedFieldBuilder();
            messageBuilder = ConstructorProvider.GetMessageBuilder();
            mentionBuilder = ConstructorProvider.GetMentionBuilder();
        }

        public static MessageBuilder PrepareMessage(LocalReportEvent report, ReferenceHub issuer, ReferenceHub target)
        {
            // clearing past data
            // this is necessary to avoid duplicating the old report
            embedBuilder.Reset();
            fieldBuilder.Reset();
            messageBuilder.Reset();

            fieldBuilder.Inline = true;

            fieldBuilder.Name = "Report By";
            fieldBuilder.Value = $"{issuer.nicknameSync.MyNick} {issuer.characterClassManager.UserId}";
            embedBuilder.AddField(fieldBuilder.Build());

            fieldBuilder.Name = "Reported UserID";
            fieldBuilder.Value = target.characterClassManager.UserId;
            embedBuilder.AddField(fieldBuilder.Build());

            fieldBuilder.Name = "Reported Player";
            fieldBuilder.Value = target.nicknameSync.MyNick;
            embedBuilder.AddField(fieldBuilder.Build());
            fieldBuilder.Value = target.characterClassManager.UserId;
            embedBuilder.AddField(fieldBuilder.Build());

            fieldBuilder.Name = "Report Reason";
            fieldBuilder.Value = report.Reason;
            embedBuilder.AddField(fieldBuilder.Build());

            embedBuilder.Title = "New In-game Report";
            // report time in UTC
            embedBuilder.Timestamp = DateTimeOffset.UtcNow;

            messageBuilder.AddEmbed(embedBuilder.Build());

            return messageBuilder;
        }

        public static MessageMentionBuilder GetMentionBuilder()
        {
            mentionBuilder.Reset();
            return mentionBuilder;
        }
    }
}