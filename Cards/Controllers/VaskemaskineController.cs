using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Cards.Controllers
{
    public class VaskemaskineController : Controller
    {
        private readonly Dictionary<int, string> vaskemaskiner = new Dictionary<int, string>(){
            {1, "wP1sjG9sd2f4rc3eI" },
            {2, "wN5sjG9sd2f4rc3e9" },
            {3, "wP5sjG9sd2f4rc3eX" },
            {4, "wN1QjG9sd2f4rc3eg" },
            {5, "wP1QjG9sd2f4rc3ej" },
            {6, "wN5QjG9sd2f4rc3ea" },
            {7, "wP5QjG9sd2f4rc3ed" },
            {8, "wN1xjG9sd2f4rc3eB" },
            {9, "wP1xjG9sd2f4rc3eU" }
        };

        public VaskemaskineController ()
        {

        }

        public IActionResult Index()
        {
            return View("SubProjects/Vaskemaskine");
        }

        public IActionResult Vaskemaskine()
        {
            return View("SubProjects/Vaskemaskine");
        }


        public string Time(int args)
        {
            if (!vaskemaskiner.ContainsKey(args)) return "-404";

            return ExtractTime(GetStringRequest(GetVaskemaskineLink(args)));
        }

        public string Status(int args)
        {
            if (!vaskemaskiner.ContainsKey(args)) return "-404";
           
            return ExtractStatus(GetStringRequest(GetVaskemaskineLink(args)));
        }

        // Return the amount of time until next reservation
        public string Until()
        {
            // List of what each number in occupied means 
            // https://backend.nortec1.dk/User/Bookings4/?App=TUK&Session=&.help
            // https://backend.nortec1.dk/Location/Calendar5/?App=TUK&session=Qg1hAS0VUFJZ0CMADC02H0xwtZOl8CtrmC6-OJPPyCyctC8lgCvJcZ1uoZOifZoPZMqYi2_IuzdQinf1JTjLWeBtuwGEP2tg3vLVkTDqM7Cjn2iI8xertxce5xtW572bX7dOd7XnNujgmwynatmAygpNehZ5U9dBb7Nib7m_Ehg&date=&days=1&calendarid=1&tick=1612620351826&native=false&.json

            string session = "QOGIAPPcmHBRBz6A2zP6FQbwLAV8zz_1jzD_QVGPizq9LzT8CzzSRASEsAVtWARP9CNGxKxpNgt01zNX-LADP0Bsu7JWZ7EumcorCbOqE46Yx4HXc7bOOlv";
            string link = "https://backend.nortec1.dk/Location/Calendar5/?App=TUK&" +
                "session=" + session + "&" +
                "date=1&" +
                "days=1&" +
                "calendarid=1&" +
                "tick=1612620351826&" +
                "native=false&" +
                ".json";

            string json = GetStringRequest(link);
            //dynamic thing = System.Text.Json.JsonSerializer.Deserialize<dynamic>(json);
            dynamic thing = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            dynamic turns = thing.Sections[0].Rows[0].Calendar.Days[0].Turns;

            int i = 0;
            for (i = 0; i < turns.Length; i++)
            {
                if (turns[i].Type != 3) break;
            }
            float sessionTime = 6f + 1.5f * i;
            int h = (int)sessionTime;
            int m = (int)((sessionTime - h) * 60f);
            DateTime dt = new DateTime();
            dt.AddHours(h);
            dt.AddMinutes(m);
            dynamic maskiner =  turns[i].Occupied;

            return turns.ToString();
        }


        public string Info(int args)
        {
            if (!vaskemaskiner.ContainsKey(args)) return "-404";

            string response = GetStringRequest(GetVaskemaskineLink(args));

            return ExtractTime(response) + "," + ExtractStatus(response);
        }


        string ExtractTime (string jsCode)
        {
            string result = jsCode;

            // https://regex101.com/r/cO8lqs/25415
            /* 
            // Safe version 
            string regex_var    = "B673L1U\\d+=[\\s\\S]*?;";  // GET VARIABLE NAME AND VALUE
            string regex_trim   = "^[ \\t]+";                // GET ALL WHITE _ SPACE AND TABS
            string regex_value  = "(?<==)\\d+";             // GET THE VALUE OF THE VARIABLE

            result = Regex.Match(result, regex_var).Value;
            result = Regex.Replace(result, regex_trim, "");
            result = Regex.Match(result, regex_value).Value;
            */

            // Faster version 
            // Get the second number after a '=' character
            result = Regex.Matches(result, "(?<==)\\d+")[1].Value;
            return result;
        }

        string ExtractStatus (string jsCode)
        {
            // Get the first word after the "='" characters
            string result = Regex.Matches(jsCode, "(?<==')\\w*")[2].Value;
            return result;
        }

        string GetVaskemaskineLink (int number)
        {
            return "https://backend.nortec1.dk/download/unit5/Ajax.ashx?" + vaskemaskiner[number];
        }
        string GetStringRequest (string link)
        {
            using var client = new HttpClient();
            //HTTP GET
            var responseTask = client.GetStringAsync(link);
            responseTask.Wait();

            return responseTask.Result;
        } 

        int GetNextReserveSession ()
        {
            
            return 0;
        }
    }
}
