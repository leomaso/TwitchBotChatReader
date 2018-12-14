using System;

namespace TwitchBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TwitchChatBot bot = new TwitchChatBot();
            try
            {
                bot.Connect();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
            
            do
            {
                String input = Console.ReadLine().ToLower();

                String argument = input.Substring(input.IndexOf(' ')+1);

                String command = input.Substring(0, input.IndexOf(' '));

                switch (command)
                {
                    case "volume":
                        bot.setVolume(Int32.Parse(argument));
                        break;
                    case "rate":
                        bot.setRate(Int32.Parse(argument));
                        break;
                    default:
                        break;
                }


            } while (true);
        }
    }
}
