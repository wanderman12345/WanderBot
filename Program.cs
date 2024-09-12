// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Discord;
using Discord.WebSocket;
using DotNetEnv;

namespace WanderBot
{
    
    internal class Program
    {
        private readonly DiscordSocketClient client;
        private readonly string token;
        public Program()
        {
            Env.Load();
            token = Environment.GetEnvironmentVariable("TOKEN");
            this.client = new DiscordSocketClient();
            this.client.MessageReceived += MessageHandler;
        }
        public async Task StartBotAsync(){
            await this.client.LoginAsync(TokenType.Bot, token);
            await this.client.StartAsync();
            await Task.Delay(-1);
        }
        private async Task MessageHandler(SocketMessage message){
            if (message.Author.IsBot) return;
            await ReplyAsync(message, "C# response works!");
        }
        private async Task ReplyAsync(SocketMessage message, string response){
            await message.Channel.SendMessageAsync(response);
        }
        static async Task Main(string[] args)
        {
           var myBot = new Program();
           await myBot.StartBotAsync();
        }
    }
}
