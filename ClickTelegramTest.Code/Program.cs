using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using ClickTelegramTest.Code.Services;
using System.Text;
using System.Text.Json;
using Telegram.Bot.Types.Payments;
using System.Text.Json.Serialization;

namespace ClickTelegramTest.Code
{
    internal class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BotToken = "7306566615:AAHIbipJji08j9OshR25KxdfvyRHEKmBtTs"; // BotFather'dan olingan bot token
        private const string ProviderToken = "398062629:TEST:999999999_F91D8F69C042267444B74CC0B3C747757EB0E065"; // Click yoki boshqa payment provider token

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
                    UpdateType.PreCheckoutQuery => HandlePreCheckoutQueryAsync(botClient, update.PreCheckoutQuery, cancellationToken)
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

        private static async Task HandlePreCheckoutQueryAsync(ITelegramBotClient botClient, PreCheckoutQuery preCheckoutQuery, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonSerializer.Serialize(preCheckoutQuery));
            await AnswerPreCheckoutQuery(preCheckoutQuery.Id, true);
        }

        private static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.CallbackQuery.Data == "sendinvoice")
            {
                Console.WriteLine(update.CallbackQuery.From.Id.ToString());
                await SendInvoiceAsync(update.CallbackQuery.From.Id.ToString());
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

        public static async Task SendInvoiceAsync(string chatId)
        {
            var url = $"https://api.telegram.org/bot{BotToken}/sendInvoice";

            var invoise = new
            {
                chat_id = chatId,
                currency = "UZS",
                description = "Test",
                payload = "test",
                provider_token = ProviderToken,
                prices = new[]
                {
                new { label = "Kurs narxi", amount = 15000 } // 15 000 so'm
            },
                title = "Test"
            };

            var content = new StringContent(JsonSerializer.Serialize(invoise), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }

        public static async Task AnswerPreCheckoutQuery(string preCheckoutQueryId, bool ok, string errorMessage = "")
        {
            var url = $"https://api.telegram.org/bot{BotToken}/answerPreCheckoutQuery";

            var requestBody = new
            {
                pre_checkout_query_id = preCheckoutQueryId,
                ok = ok,
                error_message = !ok ? errorMessage : null
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(url, content);
        }

        public bool CheckProductAvailability(string payload)
        {
            // Bu yerda mahsulotni tekshirish logikasi (DB yoki cache)
            return true; // Mahsulot mavjud bo‘lsa true, aks holda false
        }

    }
}
