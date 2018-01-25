using System;
using System.Speech.Synthesis;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Models.API.v5.Users;
using TwitchLib.Models.API.v5.Channels;
using TwitchLib.Events.Client;

namespace TwitchBot
{
    internal class TwitchChatBot
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();

        
        readonly ConnectionCredentials credential = new ConnectionCredentials(TwitchInfo.BotUserName,TwitchInfo.BotToken);
        TwitchClient client;
        public TwitchChatBot()
        {   

        }

        internal void Disconnect()
        {
            Console.WriteLine("Disconnecting");
        }

        internal void Connect()
        {
            Console.WriteLine("Connecting");

            client = new TwitchClient(credential, TwitchInfo.ChannelName, logging: false);

            client.ChatThrottler = new TwitchLib.Services.MessageThrottler(client, 10, TimeSpan.FromSeconds(30)); //new TwitchLib.Services.MessageThrottler(10, TimeSpan.FromSeconds(30));
            client.WhisperThrottler = new TwitchLib.Services.MessageThrottler(client, 10, TimeSpan.FromSeconds(30));
            client.OnLog += Client_OnLog;

            client.OnConnectionError += Client_OnConnectionError;
            client.OnMessageReceived += Client_OnMessageReceived;
            

            client.Connect();

        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            
            //if (e.ChatMessage.Message.StartsWith("!voice "))
            //{
                synth.Volume = 100;
                synth.SetOutputToDefaultAudioDevice();
                synth.Speak($"{e.ChatMessage.DisplayName} Disse: {e.ChatMessage.Message}");
            //}

            
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"Error!! {e.Error}");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}