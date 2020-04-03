# Report INtegration

> When an player uses the in-game player report functionality, it posts to the Discord Webhook provided.

# Config Options:

- srvreport_enable: true
- srvreport_webhook: https://discordapp.com/api/webhooks/xxxx/xxxx
- srvreport_roleids: 651673227952914432,651673227952914431
- srvreport_custom_message: A new in-game report has been made
- srvreport_ignorekeywords: hack,cheat,noclip,fly,instakill,oneshot

# Explanation
- srvreport_enable Enables/Disables plugin.
- srvreport_webhook is the webhook URL from a discord channel.
- srvreport_roleids is the ID of a discord role. Used to ping a role. Leave empty if you dont want a ping.
- srvreport_custom_message is the message the plugin will post in the channel. 
- srvreport_ignorekeywords is a string list that contains keywords which the plugin will use to forward reports to the Northwood Global Moderation team instead of forwarding it to your discord webhook.
