using System;
using System.Collections.Generic;
using System.Timers;

namespace TG_bot
{
    public class Chat
    {
        public long chatId = -1;
        public List<int> users;
        public double replayTime;
        const double defaultReplayTime = 1000 * 60 * 60; // ms * sec * min;
        // const double defaultReplayTime = 5000; // ms * sec * min;
        public TimerPlus timer;

        public Random random;

        public Chat(long chatId, double replayTime = defaultReplayTime)
        {
            this.chatId = chatId;
            this.replayTime = replayTime;

            random = new Random();
            // timer = new TimerPlus();
            timer = new TimerPlus(replayTime + random.Next(1000 * 60 * 5));
            // timer = new Timer(replayTime);
            timer.AutoReset = true;
        }
        
    }
}