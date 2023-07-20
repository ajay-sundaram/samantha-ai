using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace samantha_ai
{
    public class Samantha
    {
        private static readonly HttpClient client = new HttpClient();
        private static string AIEngine = Engine.Advanced;
        public async Task<string> BotReplyText(string formatted_text)
        {
            if (!string.IsNullOrWhiteSpace(formatted_text))
            {
                //get the sequence number
                var sequence_Response = await client.GetAsync("http://projectdecember.net/novemberServer/server.php?action=get_client_sequence_number&email=ajay.sundaram@yahoo.com");
                sequence_Response.EnsureSuccessStatusCode();
                string sequence_number_response = await sequence_Response.Content.ReadAsStringAsync();
                string[] sequence_number_array = sequence_number_response.Split('\n');


                //compute the hash
                string passKey = "vanitystudenttreeerrand";
                string sequence_number = sequence_number_array[0].ToString();
                string hash = HMAC.CalculateHash(passKey, sequence_number);


                //send to BOT
                string requestUri = $"http://projectdecember.net/novemberServer/server.php?action=single_response&email=ajay.sundaram@yahoo.com&sequence_number={sequence_number}&hash_value={hash}&ai_name={AIEngine}&client_command={formatted_text}";
                var bot_Response = await client.GetAsync(requestUri);
                bot_Response.EnsureSuccessStatusCode();
                string bot_reply = await bot_Response.Content.ReadAsStringAsync();

                return bot_reply;
            }
            return string.Empty;
        }

    }
}
