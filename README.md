# Notification Service

API посылающий сообщения юзерам, сообщения могут быть как текстом так и файлом.
Способы посылать сообщения: Почта, Telegram.

## To run: 
```
docker-compose up --build
```
**API открывается на 80-ом порту**

## Stack:

+ C#
+ ASP.NET Core
+ Entity Framework + Postgres
+ Quartz.NET
+ MailKit
+ Telegram.Bot
+ Serilog
+ Docker

## API Overview:

#### TaskController (/api/tasks)

+ GET /{taskId}
+ GET /history
+ PATCH /cancel/{taskId}
+ DELETE /remove/{taskId}

#### NotificationController (/api/notify)

+ POST /by-email
+ POST /by-telegram

## User Secrets:

```json
{
  "UserSecrets": {
    "ApiKeyOfTelegramBot": "TELEGRAM_APIKEY",
    "Email": {
      "From": "EMAIL",
      "Password": "EMAIL_PASSWORD",
      "Host": "smtp.yandex.ru",
      "Port": 587
    },
    "PostgresConnectionString": "Database=notificationservicedb;Server=postgres_db;Port=5432;User Id =postgres;Password=POSTGRESQL_PASSWORD;Pooling=true"
  }
}
```