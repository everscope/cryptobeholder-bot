using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace CryptoBeholderBot
{
    public static class ApiClient
    {
        public static HttpClient Client;

        public static void Initialize()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
