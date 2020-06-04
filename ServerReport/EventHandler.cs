using DSharp4Webhook.Action;
using DSharp4Webhook.Core;
using EXILED;
using EXILED.Extensions;
using System;
using System.Linq;

namespace ServerReports
{
    public sealed class EventHandler
    {
        public ReportINtegration plugin;
        // cache to prevent creating new objects if nothing has changed
        private int _cacheHashcode;
        private IMessageMention _cache;

        public EventHandler(ReportINtegration plugin) =>
            this.plugin = plugin;

        public void OnCheaterReport(ref CheaterReportEvent ev)
        {
            bool allow = ev.Allow;
            OnReport(ref allow, Player.GetPlayer(ev.ReporterId), Player.GetPlayer(ev.ReportedId), ev.Report, "CheaterReport");
            ev.Allow = allow;
        }

        public void OnLocalReport(LocalReportEvent ev)
        {
            bool allow = ev.Allow;
            OnReport(ref allow, ev.Issuer, ev.Target, ev.Reason, "LocalReport");
            ev.Allow = allow;
        }

        private void OnReport(ref bool allow, ReferenceHub issuer, ReferenceHub target, string reason, string prefix)
        {
            // Verification is performed based on the following report,
            // because webhook can remain functional
            // after the message sending state fails
            if (plugin.webhook.Status == WebhookStatus.NOT_EXISTING)
            {
                Log.Debug("Webhook broken, disabling plugin...");
                plugin.OnDisable();
                return;
            }

            // Don't send if it's a debug build
#if DEBUG
            Log.Debug($"Report intercepted:\nIssuerId: {issuer.queryProcessor.PlayerId}\nTargetId: {target.queryProcessor.PlayerId}\nReport: {reason}");
            allow = false;
#endif

            if (plugin.ignoreKeywords.Any(s => reason.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) != -1))
                return;

            // Skip it if it is debug
            // Required for testing
#if !DEBUG
            // Comparing ids before getting second referenceHub
            if (issuer == target)
            {
                issuer.Broadcast(5, "You can't report yourself.", false);
                return;
            }
#endif

            var messageBuilder = WebhookHelper.PrepareMessage(prefix, reason, issuer, target);

            if ((plugin.roleIDsToPing.GetHashCode() + plugin.customMessage.GetHashCode()) != _cacheHashcode &&
                !string.IsNullOrWhiteSpace(plugin.roleIDsToPing))
            {
                var snowflakes = plugin.roleIDsToPing.Split(',').Select(sf => sf.Trim());
                messageBuilder.AppendLine(string.Join(" ", snowflakes.Select(sf => $"<@&{sf}>")));
                messageBuilder.Append(plugin.customMessage);

                var mentionBuilder = WebhookHelper.GetMentionBuilder();
                foreach (var snowflake in snowflakes)
                    mentionBuilder.Roles.Add(snowflake);

                _cache = mentionBuilder.Build();
                _cacheHashcode = plugin.roleIDsToPing.GetHashCode() + plugin.customMessage.GetHashCode();
            }

            // a null value won't break anything
            messageBuilder.MessageMention = _cache;

            plugin.webhook.SendMessage(messageBuilder.Build()).Queue((IResult result, bool isSuccessful) =>
            {
                var restResult = result as IRestResult;
                if (!isSuccessful)
                {
                    Log.Warn("Sending the report failed");
                }

                int statusCode = -1;
                if (restResult.LastResponse.HasValue)
                    statusCode = (int)restResult.LastResponse.Value.StatusCode;

                Log.Debug($"Action is performed: {isSuccessful}");
                Log.Debug($"StatusCode: {statusCode}");
            });
        }
    }
}