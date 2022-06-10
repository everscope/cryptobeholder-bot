
# CryptoBeholder bot

Cryptobehoder bot is a C# Telegram bot written with Telegram Bot API and CoinGecko API.

  

CryptoBeholder bot provides tracking coin prices. Users can add or remove a coin from the tracking list, and edit tracking settings. Users can configure notifications from the bot, messages could be sent when:

-   price reaches selected value
-   price decreased or increased for the last 24 hours more, then selected percentage
-   after some time passes

## List of Commands

-   `/help` - to show description of bot and short instructions how to use it
-   `/trace_new` - to start tracking new Coins pricee
-   `/edit_trace` - to edit settings of tracked coin
-   `/delete_trace` - stop tracking coin
-   `/trace_list` - to show list of all tracked coins
-   `/trace_settings_list` - to show list of all tracked coins and thier settings
-   `/get_my_info -` to show list of all tracked coins with their price at the moment
-   `/change_vs_currency` - to change default currency
