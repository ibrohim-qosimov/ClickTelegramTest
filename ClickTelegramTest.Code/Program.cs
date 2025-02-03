using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ClickTelegramTest.Code
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient("7306566615:AAHIbipJji08j9OshR25KxdfvyRHEKmBtTs");

            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();

            //Har qanday o'zgarish shu method 
            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                var handler = update.Type switch
                {
                    UpdateType.Message => HandleMessageAsync(botClient, update, cancellationToken),
                    UpdateType.CallbackQuery => HandleCallbackQueryAsync(botClient, update, cancellationToken),
                    //Yana update larni davom ettirib tutishingiz mumkin
                };

                try
                {
                    await handler;
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Xato:{ex.Message}");
                }
            }

            //Polling errorlarni shu ushledi
            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }
        }

        private static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(update.CallbackQuery.Data == "sendinvoice")
            {
                // SendInvoice
            }
        }

        private static async Task HandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //betda update.Messageni null emasligini tekshirib,
            //null bo'lmasa message degan o'zgaruvchiga qiymatni olib beryapti
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            Console.WriteLine($"Received a '{messageText}' message in chat {update.Message.Chat.Id}.");

            if (messageText == "/start")
            {
                var inlineButton = new InlineKeyboardButton("To'lov")
                {
                    CallbackData = "sendinvoice",
                };

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        inlineButton
                    }
                });

                await botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"Assalomu alaykum <b> {update.Message.From.FirstName} </b>\n5000 sum to'lovoring!",
                    parseMode: ParseMode.Html,
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken);
            }
        }

    }
}
