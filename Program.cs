// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Discord;
using Discord.WebSocket;
using DotNetEnv;
using OpenAI_API;
using OpenAI_API.Completions;

namespace WanderBot
{
    internal class Program
    {
        private readonly DiscordSocketClient client;
        private readonly OpenAIAPI openAiApi;
        private readonly string token;

        public Program()
        {
            Env.Load();
            token = Environment.GetEnvironmentVariable("TOKEN");
            string openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            openAiApi = new OpenAIAPI(openAiApiKey);

            this.client = new DiscordSocketClient();
            this.client.MessageReceived += MessageHandler;
        }

        public async Task StartBotAsync()
        {
            await this.client.LoginAsync(TokenType.Bot, token);
            await this.client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task MessageHandler(SocketMessage message)
        {
            if (message.Author.IsBot) return;
            Console.WriteLine($"Received message from {message.Author.Username}: {message.Content}");
            string response = await GetOpenAIResponse(message.Content);
            await ReplyAsync(message, response);
        }

        private async Task<string> GetOpenAIResponse(string prompt)
        {
            var completionRequest = new CompletionRequest
            {
                Prompt = prompt,
                Model = "asst_3uOiy53Sypw1p4BjGJbynnAr",
                MaxTokens = 50
            };

            Console.WriteLine($"Waiting for response");
            var completionResult = await openAiApi.Completions.CreateCompletionAsync(completionRequest);
              Console.WriteLine("Received response from OpenAI.");
            Console.WriteLine($"OpenAI response: {completionResult.Completions[0].Text}");
            return completionResult.Completions[0].Text.Trim();
        }

        private async Task ReplyAsync(SocketMessage message, string response)
        {
            await message.Channel.SendMessageAsync(response);
        }

        static async Task Main(string[] args)
        {
            var myBot = new Program();
            await myBot.StartBotAsync();
        }
    }
}