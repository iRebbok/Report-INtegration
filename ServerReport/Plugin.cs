using System;
using System.Collections.Generic;
using EXILED;


namespace ServerReports
{
	public class Plugin : EXILED.Plugin
	{
        public EventHandlers EventHandlers;
        public bool Enabled;
        public string WebhookURL;
        public string RoleIDsToPing;
        public string CustomMessage;
        public List<string> ignoreKeywords;

		
		public override void OnEnable()
		{
			try
			{
                Enabled = Config.GetBool("srvreport_enable", true);
                WebhookURL = Config.GetString("srvreport_webhook", "");
                RoleIDsToPing = Config.GetString("srvreport_roleids", "");
                CustomMessage = Config.GetString("srvreport_custom_message", "A new in-game report has been made!");
                ignoreKeywords = Config.GetStringList("srvreport_ignorekeywords");

                if (string.IsNullOrWhiteSpace(WebhookURL) || !Enabled)
                {
                    Log.Warn("There is no WebhookURL set in the config or you have disabled the plugin. Plugin is Disabled.");
                    return;
                }
                EventHandlers = new EventHandlers(this);
                Events.CheaterReportEvent += EventHandlers.OnCheaterReport;
                Log.Info($"Report INtegration Loaded");
			}
			catch (Exception e)
			{
                Log.Error($"There was an error loading the plugin: {e}");
			}
		}

        public override void OnDisable()
		{
            Events.CheaterReportEvent -= EventHandlers.OnCheaterReport;
            EventHandlers = null;
		}

		public override void OnReload()
		{
            //haha no
		}

		public override string getName { get; } = "Server Reports to Discord";
	}
}