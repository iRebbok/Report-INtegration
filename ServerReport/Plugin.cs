using System;
using System.Collections.Generic;
using EXILED;


namespace ServerReports
{
	public class Plugin : EXILED.Plugin
	{
        public EventHandlers EventHandlers;
        public string WebhookURL;
        public string RoleIDsToPing;
        public string CustomMessage;

		
		public override void OnEnable()
		{
			try
			{
                WebhookURL = Config.GetString("srvreport_webhook", "");
                RoleIDsToPing = Config.GetString("srvreport_roleids", "");
                CustomMessage = Config.GetString("srvreport_custom_message", "A new in-game report has been made!");
                
                EventHandlers = new EventHandlers(this);
                Events.CheaterReportEvent += EventHandlers.OnCheaterReport;
                Info($"Report INtegration Loaded");
			}
			catch (Exception e)
			{
				Error($"There was an error loading the plugin: {e}");
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