using DSharp4Webhook.Action;
using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using EXILED;
using ServerReports.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;

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
            Events.LocalReportEvent += eventHandler.OnLocalReport;
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
            AppDomain.CurrentDomain.AssemblyResolve -= OnResolveAssembly;
            Events.LocalReportEvent -= eventHandler.OnLocalReport;
            webhook?.Dispose();
        }

        public override void OnReload() { }

        public Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Log.Debug($"Attempt to solve the dependency by name: {args.Name}");

            if (args.Name.StartsWith("Newtonsoft.Json"))
                return Assembly.Load(Resources.Newtonsoft_Json);

            Log.Debug("The dependency could not be resolved");
            return null;
        }
    }
}