using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Payments;

namespace ClickTelegramTest.Code.Models
{
    public class SendInvoiceRequest
    {
        public object ChatId { get; set; } // Foydalanuvchi ID yoki @channelusername
        public int? MessageThreadId { get; set; } // Forum mavzu identifikatori (optional)
        public string Title { get; set; } // Mahsulot nomi (1-32 ta belgi)
        public string Description { get; set; } // Mahsulot tavsifi (1-255 ta belgi)
        public string Payload { get; set; } // Bot tomonidan aniqlangan invoice payload (1-128 bayt)
        public string ProviderToken { get; set; } // @BotFather dan olingan token
        public string Currency { get; set; } // Uch harfli valyuta kodi (UZS, USD, EUR)
        public List<LabeledPrice> Prices { get; set; } // Narx va boshqa tarkibiy qismlar ro‘yxati
    }
}
