using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

using Cards.Models.Spotify;

namespace Cards.Controllers
{
    public class SpotifyController : Controller
    {
        private static readonly HttpClient client = HttpClientFactory.Create();
        string received;


        // SPOTIFY (FILL OUT)
        string client_id = "7a4c451081c147f6ac752b7ab246ab42";
        string client_secret = "4ebeb559c6744c12b3b8afbf40359e8c";
        // code can only be used once
        string code = "AQBieJtA0Hf4w0rhD3Gs3PJg8Nxq6jS_nvBUZvdcer78-Zqjbx1TkJHs__AgJG9MoNUm5DYnUErPrf4-6mCW8i6cogbt1T6lbDT9g74GVYYrwes1cdyKnBqQrx9Yku1UKU0mCBys0PlzbcMDcE4QxPbSmObBqCTJsB5IJV-t1xUdOSZ-KtHKCyqPzig2B7cRmbMCgRTzzvurrjzZcMEutq1wHcqeVup4dk0WgySvUtAKiKSEAapejCS2O2V45ob0rILwaASaOWS_FLfTUNoyCEE9uuKhHdwPqG67yuJHT3Ez-XeTAA";


        string encoded = "N2E0YzQ1MTA4MWMxNDdmNmFjNzUyYjdhYjI0NmFiNDI6NGViZWI1NTljNjc0NGMxMmIzYjhhZmJmNDAzNTllOGM=";   // Calculated this way: encoded = base64(client_id + ":" + client_secret);
        string refreshToken = "AQC5yOlVlCnB8mcOtXZxLDKE2tbdCDKSUi013LNODTrIljaCKivAEq80t1mRBacn4RYwRxk7n-u3qI-5n9o1WC8gfg9W83-0aRc_205fnLo09pey7Qa0y9rZJn1m2Jd5rXs";
        
        
        public IActionResult Index()
        {
            var task = GetRequest();
            task.Wait();
            return View("Index");
        }

        public IActionResult Test ()
        {
            var task = SpotifyPostRequest("https://accounts.spotify.com/api/token", "{\"grant_type\":\"refresh_token\",\"refresh_token\":\"" + refreshToken + "\"}", true, encoded);
            task.Wait();
            return View("GetQueue", received);
        }

        public async Task SpotifyGetRequest (string serverName, string body, bool authorization, string auth)
        {
            if (authorization) 
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth);
            client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Add("Content-Length", "0");
            var response = await client.GetAsync(serverName + "?" + body);

            string content = await response.Content.ReadAsStringAsync();

            received = content;
            //SpotifyJSON sp = JsonConvert.DeserializeObject<SpotifyJSON>(content);
        }


        public async Task SpotifyPostRequest (string serverName, string body, bool authorization, string auth)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Add("Content-Length", "0");

            if (authorization)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

            
            HttpContent httpContent = new StringContent(body, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync(serverName, httpContent);

            string content = await response.Content.ReadAsStringAsync();

            received = content;

        }

        public async Task GetRequest()
        {
            var response = await client.GetAsync("https://www.metaweather.com/api/location/2471217/");
            if (response.IsSuccessStatusCode)
            {
                 var responseStream = await response.Content.ReadAsStringAsync();
                received = responseStream;
            }
        }


    }
}
