# cryptocurrency-price-tracker
This is cryptocurrency tracker Telegram Bot based on Telegram Bot API and CoinGecko API

To use this bot:
Downoad, open .sln file and run project. Database will be created while starting. 
Main commands are:
-help - info about bot
-trace_new - adds new coin to track, and traces it with default settings (message if price will grow or fall on 5%)
-trace_list - list of all coins are tracked
-trace_settings_list - list all coins with their settings
-edit_trace - change tracking mode
-get_my_info - send message with all current prices of tracked coins
-a few other minor commands

Work with commands made with switching different states. 
Class "Tracer" loads new info about price and checks, if there is a need to notify user about price change.
