// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Discord;
using Discord.WebSocket;
using DotNetEnv;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Chat;

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
            APIAuthentication aPIAuthentication = new APIAuthentication(openAiApiKey);
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
                var messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, "You are a helpful assistant that mimicks hank schrader from breaking bad"),
                    new ChatMessage(ChatMessageRole.User, prompt)
                };

                var chatRequest = new ChatRequest
                {
                    Model = "gpt-3.5-turbo",
                    Messages = messages,
                    MaxTokens = 80
                };

                var chatResult = await openAiApi.Chat.CreateChatCompletionAsync(chatRequest);
                var generatedText = chatResult.Choices[0].Message.Content;

                Console.WriteLine("Generated text:");
                Console.WriteLine(generatedText);

                return generatedText;
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