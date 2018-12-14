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
using System.Collections.Generic;

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

        public void setVolume(int value)
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
        
        internal void Disconnect()
        {
            Console.WriteLine("Disconnecting");
        }        

        internal void Connect()
        {

            LoadConfig();
            ConnectionCredentials credential = new ConnectionCredentials(config.BotUserName, config.BotToken);
            createSynth();

            client = new TwitchClient();
            twitchAPI = new TwitchAPI();

            FollowerService followerService = new FollowerService(twitchAPI, 1, 25);

            client.Initialize(credential, config.ChannelName);

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
            synth.Speak("Obrigado pelo host " + e.BeingHostedNotification.HostedByChannel.ToString() + " Jesus te ama");
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            Console.WriteLine("Usuario: {0} entrou as {1}", e.Username, DateTime.Now.ToString());
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {

            String msg = e.ChatMessage.Message.ToLower();
            SpeechSynthesizer synth = createSynth();

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
                synth.Dispose();
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

        public void LoadConfig()
        {
            using (StreamReader r = new StreamReader("default.json"))
            {
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(json);
            }
        }

        internal SpeechSynthesizer createSynth()
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();

            LoadConfig();            
            int count = 0;
            if (config.Voice == "")
            {
                Console.WriteLine("Choose one voice: ");

                foreach (InstalledVoice voice in synth.GetInstalledVoices())
                {
                    Console.WriteLine("[" + count + "] - " + voice.VoiceInfo.Name);
                    count++;
                }

                count = Convert.ToInt32(Console.ReadLine());

                config.Voice = synth.GetInstalledVoices()[count].VoiceInfo.Name;

                using (StreamWriter file = File.CreateText(@"default.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, config);
                }

            }

            synth.SelectVoice(config.Voice);
            return synth;
        }
    }
}