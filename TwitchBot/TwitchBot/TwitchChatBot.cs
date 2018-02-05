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


            
            synth.SelectVoice("VE_Brazilian_Portuguese_Fernanda_22kHz");
            

            Console.WriteLine("Connecting");
            
            client = new TwitchClient(credential, TwitchInfo.ChannelName, logging: false);

            client.ChatThrottler = new TwitchLib.Services.MessageThrottler(client, 10, TimeSpan.FromSeconds(30)); //new TwitchLib.Services.MessageThrottler(10, TimeSpan.FromSeconds(30));
            client.WhisperThrottler = new TwitchLib.Services.MessageThrottler(client, 10, TimeSpan.FromSeconds(30));            
            client.OnLog += Client_OnLog;

            client.OnConnectionError += Client_OnConnectionError;
            client.OnMessageReceived += Client_OnMessageReceived;            

            client.Connect();

            Console.WriteLine("Success");
            synth.Speak("Conectado");


        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {

            String msg = e.ChatMessage.Message.ToLower();

            if (!msg.StartsWith("!voice "))
            {
                synth.Volume = volume;
                synth.Rate = rate;

                synth.SetOutputToDefaultAudioDevice();

                msg = msg.Replace("andrxd", "andré");

                synth.Speak($"{e.ChatMessage.DisplayName} Disse: {msg}");
                

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