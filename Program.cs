﻿using System;
using System.Timers;
using System.Threading;
using System.Collections.Generic;

namespace TG_bot
{
    class Program
    {
        static Bot bot;
        static void Main(string[] args)
        {
            bot = Bot.GetBot();
            bot.Start();
            Console.WriteLine("Press any key to exit");
            while(true)
                Thread.Sleep(1000);
            // Console.ReadKey();
        }
    }
}
