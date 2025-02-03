using ClickTelegramTest.Code.Models;
using System.Text;
using System.Text.Json;
using Telegram.Bot.Types.Payments;

namespace ClickTelegramTest.Code.Services;
public class TelegramInvoiceService
{
    private readonly HttpClient _httpClient;
    private const string BotToken = "7306566615:AAHIbipJji08j9OshR25KxdfvyRHEKmBtTs"; // BotFather'dan olingan bot token
    private const string ProviderToken = "398062629:TEST:999999999_F91D8F69C042267444B74CC0B3C747757EB0E065"; // Click yoki boshqa payment provider token

    public TelegramInvoiceService()
    {
        _httpClient = new HttpClient();
    }

    public static async Task SendInvoiceAsync(string chatId)
    {
        var url = $"https://api.telegram.org/bot{BotToken}/sendInvoice";

        var invoise = new 
        {
            ChatId = chatId,
            Currency = "UZS",
            Description = "Test",
            Payload = "test",
            ProviderToken = ProviderToken,
            prices = new[]
            {
                new { label = "Kurs narxi", amount = 15000 } // 15 000 so'm
            },
            Title = "Test"
        };

        var content = new StringContent(JsonSerializer.Serialize(invoise), Encoding.UTF8, "application/json");

        //var response = await _httpClient.PostAsync(url, content);
      //  var result = await response.Content.ReadAsStringAsync();
        //Console.WriteLine(result);
    }
}

