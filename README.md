# Report INtegration

> When an player uses the in-game player report functionality, it posts to the Discord Webhook provided.

# Config Options:

- srvreport_enable: true
- srvreport_webhook: https://discordapp.com/api/webhooks/xxxx/xxxx
- srvreport_roleids: 651673227952914432,651673227952914431
- srvreport_custom_message: A new in-game report has been made

# Explanation
- srvreport_enable Enables/Disbles plugin.
- srvreport_webhook is the webhook URL from a discord channel.
- srvreport_roleids is the ID of a discord role. Used to ping a role. Leave empty if you dont want a ping.
- srvreport_custom_message is the message the plugin will post in the channel. 
