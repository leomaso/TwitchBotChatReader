using System;
using System.Speech.Synthesis;
using TwitchLib;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Services;
using TwitchLib.Client;
using TwitchLib.Client.Events.Services;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.FollowerService;

using System.IO;
using Newtonsoft.Json;

namespace TwitchBot
{
    internal class TwitchChatBot
    {
        

        TwitchClient client;
        TwitchAPI twitchAPI;
        SpeechSynthesizer synth = new SpeechSynthesizer();
        Config config;

        int volume = 100;
        int rate = -3;

        public void setVolume (int value)
        {
            volume = value;
        }

        public void setRate(int value)
        {
            rate = value;
        }
        
        
        
        public TwitchChatBot()
        {
            
        }
        public void LoadConfig()
        {
            using (StreamReader r = new StreamReader("default.json"))
            {
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(json);
            }
        }
        internal void Disconnect()
        {
            Console.WriteLine("Disconnecting");
        }
        
        internal void Connect()
        {



            LoadConfig();

            ConnectionCredentials credential = new ConnectionCredentials(config.BotUserName, config.BotToken);



            client = new TwitchClient();
            twitchAPI = new TwitchAPI();

            FollowerService followerService = new FollowerService(twitchAPI, 1, 25);
            
            client.Initialize(credential, config.ChannelName);

            synth.SelectVoice("VE_Brazilian_Portuguese_Fernanda_22kHz");            

            Console.WriteLine("Connecting");
            
            
            client.ChatThrottler = new MessageThrottler(client, 10, TimeSpan.FromSeconds(30)); 
            client.WhisperThrottler = new MessageThrottler(client, 10, TimeSpan.FromSeconds(30));

            followerService.OnNewFollowersDetected += Follow_OnNewFollowerDetected;
            client.OnLog += Client_OnLog;

            client.OnConnectionError += Client_OnConnectionError;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnUserJoined += Client_OnUserJoined;
            client.OnBeingHosted += Client_OnBeingHosted;
            
            client.Connect();

            Console.WriteLine("Success");
            synth.Speak("Conectado");


        }

        private void Follow_OnNewFollowerDetected(object sender, OnNewFollowersDetectedArgs e)
        {
            foreach (var item in e.NewFollowers)
            {
                synth.Speak("obrigado pelo fólou " + item.User.Name);
            }
            
        }

        private void Client_OnBeingHosted(object sender, OnBeingHostedArgs e)
        {
            synth.Speak("Obrigado pelo host "+e.BeingHostedNotification.HostedByChannel.ToString() + " Jesus te ama");
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            Console.WriteLine("Usuario: {0} entrou as {1}", e.Username, DateTime.Now.ToString());            
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {

            String msg = e.ChatMessage.Message.ToLower();

            
            if (!msg.StartsWith("!voice "))
            {
                synth.Volume = volume;
                synth.Rate = rate;

                synth.SetOutputToDefaultAudioDevice();

                if (e.ChatMessage.DisplayName == "andrxd")
                {
                    synth.Speak($"André Disse: {msg}");
                }
                else
                {
                    synth.Speak($"{e.ChatMessage.DisplayName} Disse: {msg}");
                }                
            }           

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