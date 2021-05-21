using System;
using System.Collections.Generic;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;
using System.Linq;

namespace TG_bot
{
    public class Bot
    {
        protected ITelegramBotClient botClient;
        // List<BotCommand> botCommands = new List<BotCommand>() {
        //     new BotCommand() {Command = "start", Description = "Запустить бота"},
        //     new BotCommand() {Command = "stop", Description = "Остановить бота"},
        //     new BotCommand() {Command = "settings", Description = "Настройки"},
        // };
        // List<BotCommand> botSettingsCommand = new List<BotCommand>() {
        //     new BotCommand() {Command = "setinterval", Description = "Установить интервал оповещений"},
        //     new BotCommand() {Command = "notifyonweekends", Description = "Уведомлять ли в выходные"},
        //     new BotCommand() {Command = "setstarttime", Description = "Время начала уведомлений"},
        //     new BotCommand() {Command = "setstoptime", Description = "Время начала уведомлений"},
        // };
        static readonly Dictionary<BotCommand, Func<Message, List<Message>>> commands = new Dictionary<BotCommand, Func<Message, List<Message>>>()
        {
            {
                new BotCommand(){Command = "start", Description = "Запустить бота"},
                startCommand
            },
            {
                new BotCommand() {Command = "stop", Description = "Остановить бота"},
                stopCommand
            },
            {
                new BotCommand() {Command = "settings", Description = "Настройки"},
                settingsCommand
            },
            {
                new BotCommand() {Command = "timeleft", Description = "Настройки"},
                timeleftCommand
            }
        };

        static List<Message> startCommand(Message msg)
        {
            var replays = new List<Message>();

            if (Bot.GetBot().chats.Where(chat => chat.chatId == msg.Chat.Id).Count() == 0)
            {
                Bot.GetBot().chats.Add(new Chat(msg.Chat.Id));
                var thisChat = Bot.GetBot().chats.Where(chat => chat.chatId == msg.Chat.Id).First();

                replays.Add(new Message() { Text = "Теперь я буду звать покурить примерно каждый час\nПогнали, йопта 😜" });

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
            else
            {
                replays.Add(new Message()
                {
                    Sticker = new Sticker()
                    {
                        FileId = "CAACAgIAAxkBAAPnYKeoBjJxguIE9tDH0LsvI_gE4pUAAs4IAAL6C7YI6rxUhlYWVUkfBA"
                    }
                });
            }
            return replays;
        }
        static List<Message> stopCommand(Message msg)
        {
            var replays = new List<Message>();

            if (Bot.GetBot().chats.Where(chat => chat.chatId == msg.Chat.Id).Count() == 0)
            {
                replays.Add(new Message() { Text = "WTF?\nМы ж не начинали, чтоб заканчивать" });
            }
            else
            {
                var thisChat = Bot.GetBot().chats.Where(chat => chat.chatId == msg.Chat.Id).First();
                Bot.GetBot().chats.Remove(thisChat);
                replays.Add(new Message() { Text = "Ты чо, бросить решил?! 😱" });
                replays.Add(new Message() { Text = "ещё увидимся, сучка 😉\nПиши, как осознаешь" });
            }
            return replays;
        }
        static List<Message> settingsCommand(Message msg)
        {
            var replays = new List<Message>();
            return replays;
        }
        static List<Message> timeleftCommand(Message msg)
        {
            var replays = new List<Message>();
            if (Bot.GetBot().chats.Count != 0)
            {
                var thisChat = Bot.GetBot().chats.Where(chat => chat.chatId == msg.Chat.Id).First();
                var tl = thisChat.timer.TimeLeft;
                var s = tl / 1000;
                var m = s / 60;
                var h = m / 60;
            }
            return replays;
        }

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
            var res = botClient.SetMyCommandsAsync(commands.Keys);
            // Console.WriteLine("Не удалось изменить список команд");
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
                var cmd = commands.Where(cmd => cmd.Key.Command == e.Message.Text.Split('@')[0].Trim('/')).FirstOrDefault();
                if (cmd.Key != null)
                {
                    var replays = cmd.Value.Invoke(e.Message);
                    foreach (var replay in replays)
                    {
                        if (replay.Sticker != null)
                        {
                            await Bot.GetBot().botClient.SendStickerAsync(
                                chatId: e.Message.Chat.Id,
                                sticker: replay.Sticker.FileId);
                        }
                        else
                        {
                            await Bot.GetBot().botClient.SendTextMessageAsync(
                                chatId: e.Message.Chat.Id,
                                text: replay.Text);
                        }
                    }
                }
            }
        }


    }
}