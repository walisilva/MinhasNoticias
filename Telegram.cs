using System.Collections.Specialized;
using System.Configuration;
using Telegram.Bot;

namespace MinhasNoticias
{
    internal class Telegram
    {
        public static async Task EnviarAsync(string txt)
        {
            var TelegramInfo = GetTelegramInfo();
            string botToken = TelegramInfo["telegram"]["token"];
            string chatid = TelegramInfo["telegram"]["chatid"];

            var botClient = new TelegramBotClient(botToken);

            await botClient.SendTextMessageAsync(
                chatId: chatid,
                text: txt
            );
        }

        private static Dictionary<string, Dictionary<string, string>> GetTelegramInfo()
        {
            var Dict = new Dictionary<string, Dictionary<string, string>>();

            var Section = ConfigurationManager.GetSection("telegram") as NameValueCollection;

            if (Section != null)
            {
                foreach (var key in Section.AllKeys)
                {
                    var Prefix = key.Split(':')[0];

                    if (!Dict.ContainsKey(Prefix))
                    {
                        Dict[Prefix] = new Dictionary<string, string>();
                    }

                    var InfoKey = key.Split(':')[1];
                    var InfoValue = Section[key];

                    Dict[Prefix][InfoKey] = InfoValue;
                }
            }

            return Dict;
        }
    }
}
