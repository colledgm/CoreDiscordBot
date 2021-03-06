﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Core_Discord
{
    internal sealed class Program
    {
        static public IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] arg)
        {
            try
            {
                MainAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an exception: {ex.ToString()}");
            }
        }
        /// <summary>
        /// Provides the main way to instaniate the bot
        /// This will have to take in a List using a built in scheduler to make it possible to run multiple bots form one instance
        /// </summary>
        /// <returns></returns>
        public static async Task MainAsync()
        {

            var config = new CoreConfig();
            var json = string.Empty;

            if (!File.Exists("config.json"))
            {
                json = JsonConvert.SerializeObject(config);
                File.WriteAllText("config.json", json, new UTF8Encoding(false));
                Console.WriteLine("Config file not found, a new one is generated. Please fill it with the proper values and re run this program");
                Console.ReadKey();

                return;
            }

            json = File.ReadAllText("config.json", new UTF8Encoding(false));
            config = JsonConvert.DeserializeObject<CoreConfig>(json);

            var tasklist = new List<Task>();
            for(var i = 0; i < config.ShardCount; i++)
            {
                var bot = new Core(Process.GetCurrentProcess().Id, i);
                tasklist.Add(bot.RunAsync());
            }
            await Task.WhenAll(tasklist).ConfigureAwait(false);

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}
