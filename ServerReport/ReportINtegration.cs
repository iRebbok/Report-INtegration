using DSharp4Webhook.Action;
using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using EXILED;
using System.Collections.Generic;

namespace ServerReports
{
    public class ReportINtegration : Plugin
    {
        public EventHandler eventHandler;

        public IWebhook webhook;
        public bool enabled;
        public string webhookURL;
        public string roleIDsToPing;
        public string customMessage;
        public List<string> ignoreKeywords;

        public override string getName { get; } = "Server Reports to Discord";

        public override void OnEnable()
        {
            enabled = Config.GetBool("srvreport_enable", true);
            roleIDsToPing = Config.GetString("srvreport_roleids", string.Empty);
            webhookURL = Config.GetString("srvreport_webhook", string.Empty);
            customMessage = Config.GetString("srvreport_custom_message", "A new in-game report has been made!");
            ignoreKeywords = Config.GetStringList("srvreport_ignorekeywords");

            if (string.IsNullOrWhiteSpace(webhookURL) || !enabled)
            {
                Log.Warn("There is no WebhookURL set in the config or you have disabled the plugin. Plugin is Disabled.");
                return;
            }

            // an exception is thrown here if the url is incorrect
            webhook = WebhookProvider.CreateStaticWebhook(webhookURL);
            eventHandler = new EventHandler(this);
            Events.CheaterReportEvent += eventHandler.OnCheaterReport;
            Log.Info("Report INtegration Loaded");
            webhook.GetInfo().Queue((IResult result, bool isSuccessfully) =>
            {
                if (!isSuccessfully)
                {
                    Log.Info("Webhook broken");
                    OnDisable(); // this method is literally like Dispose()
                    return;
                }

                var infoResult = result as IInfoResult;
                Log.Info($"Webhook confirmed its existence:\nWebhookName: {infoResult.WebhookInfo.Name}\nWebhookSnowflake: {infoResult.WebhookInfo.Id}\nGuildId: {infoResult.WebhookInfo.GuildId}\nChannelId: {infoResult.WebhookInfo.ChannelId}");
            });
        }

        public override void OnDisable()
        {
            Events.CheaterReportEvent -= eventHandler.OnCheaterReport;
            webhook?.Dispose();
        }

        public override void OnReload() { }
    }
}