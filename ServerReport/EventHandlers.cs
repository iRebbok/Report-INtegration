using System;
using System.Collections.Generic;
using System.Linq;
using EXILED;
using Grenades;
using MEC;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;
using System.Net.Http;

namespace ServerReports
{
    public class EventHandlers
    {
        public Plugin plugin;
        private string pingPongRoles;

        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnCheaterReport(ref CheaterReportEvent ev)
        {
            string Report = ev.Report;
            bool keywordFound = plugin.ignoreKeywords.Any(s => Report.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);
            if (keywordFound) return;
            ReferenceHub reportedPlayer = Plugin.GetPlayer(ev.ReportedId);
            ReferenceHub reportedBy = Plugin.GetPlayer(ev.ReportedId);

            Webhook webhk = new Webhook(plugin.WebhookURL);

            List<Embed> listEmbed = new List<Embed>();
          

            EmbedField reporterName = new EmbedField();
            reporterName.Name = "Report By";
            reporterName.Value = Plugin.GetPlayer(ev.ReporterId).nicknameSync.MyNick;
            reporterName.Inline = true;

            EmbedField reporterUserID = new EmbedField();
            reporterUserID.Name = "Reporter UserID";
            reporterUserID.Value = Plugin.GetPlayer(ev.ReporterId).characterClassManager.UserId;
            reporterUserID.Inline = true;

            EmbedField reportedName = new EmbedField();
            reportedName.Name = "Reported Player";
            reportedName.Value = Plugin.GetPlayer(ev.ReportedId).nicknameSync.MyNick;
            reportedName.Inline = true;

            EmbedField reportedUserID = new EmbedField();
            reportedUserID.Name = "Reported Player";
            reportedUserID.Value = Plugin.GetPlayer(ev.ReportedId).characterClassManager.UserId;
            reportedUserID.Inline = true;

            EmbedField Reason = new EmbedField();
            Reason.Name = "Report Reason";
            Reason.Value = ev.Report;
            Reason.Inline = true;

            Embed embed = new Embed();
            embed.Title = "New In-game Report";
            embed.Fields.Add(reporterName);
            embed.Fields.Add(reporterUserID);
            embed.Fields.Add(reportedName);
            embed.Fields.Add(reportedUserID);
            embed.Fields.Add(Reason);
            

            listEmbed.Add(embed);

           
            if (string.IsNullOrWhiteSpace(plugin.RoleIDsToPing)) webhk.Send(plugin.CustomMessage, null, null, false, embeds: listEmbed);
            else
            {
                if (!plugin.RoleIDsToPing.Contains(','))
                {
                    webhk.Send("<@&" + plugin.RoleIDsToPing + "> " + plugin.CustomMessage, null, null, false, embeds: listEmbed);
                }
                else
                {
                    string[] split = plugin.RoleIDsToPing.Split(',');
                    foreach (string roleid in split)
                    {
                        pingPongRoles += $"<@&{roleid}> ";
                    }
                    webhk.Send(pingPongRoles + "" + plugin.CustomMessage, null, null, false, embeds: listEmbed);
                }
            }
        }
    }
}