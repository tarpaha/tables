# Tables

## Description
A program for organizing tournaments where people play several rounds at tables, and each player should not meet at the tables in new rounds with those they have already played with.

### App
Console application

### Bot
Telegram bot, use
```text
docker build -t tables-bot .
```
to build docker container and
```text
docker run -d --restart always --name tables-bot -e "TELEGRAM_BOT_TOKEN=<TOKEN>" tables-bot
```
to run it in detached state with auto restart

`<TOKEN>` is the telegram bot token received from [BotFather](https://t.me/BotFather)
