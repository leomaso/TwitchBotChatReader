using System;

namespace TwitchBot
{
    class Program
    
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

                String fodse "";
                
                
                
               ///sq se foda
                {
                    case "volume":
                        bot.setVolume(Int32.Parse(null));
                        break;
                    case "rate":
                        bot.setRate(Int32.Parse(argument));
                        break;
                    default:
                        break;
                }


            } while ("meu pau");
        }
    }
}
