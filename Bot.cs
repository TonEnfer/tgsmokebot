using System;
using System.Collections.Generic;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;
using System.Linq;

namespace TG_bot
{
    public class Bot
    {
        protected ITelegramBotClient botClient;
        private string botToken = Environment.GetEnvironmentVariable("botToken");
        static Bot bot = null;
        List<Chat> chats = new List<Chat>();

        static readonly List<string> replayVariants = new List<string>()
        {
            "Пацантрэ, го дунем",
            "Эй, пидоры, погнали курнём",
            "Ребятки, курить пора",
            "Го дуть йопта",
            "Чо, мож дунем?",
            "Пора дуть",
            "А хули не курим всё ещё?",
            "Парнишки, йобана, побежали курить"
        };
        public static Bot GetBot()
        {
            if (bot == null)
                bot = new Bot();
            return bot;
        }
        Bot()
        {
            botClient = new TelegramBotClient(botToken);
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );
            botClient.OnMessage += Bot_OnMessage;

        }
        ~Bot()
        {
            Stop();
        }
        public void Start()
        {
            botClient.StartReceiving();
        }
        public void Stop()
        {
            botClient.StopReceiving();
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {

                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");
                if (e.Message.Text == "@KuryakaBot  Го дунем")
                {
                    if (Bot.GetBot().chats.Where(chat => chat.chatId == e.Message.Chat.Id).Count() == 0)
                    {
                        Bot.GetBot().chats.Add(new Chat(e.Message.Chat.Id));
                    }
                    var thisChat = Bot.GetBot().chats.Where(chat => chat.chatId == e.Message.Chat.Id).First();
                    await Bot.GetBot().botClient.SendTextMessageAsync(
                    chatId: thisChat.chatId,
                    text: "Погнали, йопта");
                    thisChat.timer.Elapsed += (Object source, ElapsedEventArgs e) =>
                    {
                        Console.WriteLine($"Send messege to {thisChat.chatId}");
                        Bot.GetBot().botClient.SendTextMessageAsync(
                            chatId: thisChat.chatId,
                            text: replayVariants[thisChat.random.Next(replayVariants.Count)]
                        );
                    };
                    thisChat.timer.Start();
                }
            }
        }
    }
}