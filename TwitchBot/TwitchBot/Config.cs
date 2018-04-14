using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    public class Config
    {
        public string BotToken { get; set; }
        public string BotUserName { get; set; }
        public string ChannelName { get; set; }
        public string ClientID { get; set; }
        public string OAuth { get; set; }
        public string Voice { get; set; }
        
    }

}
