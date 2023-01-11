using Telegram.Bot;



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Web;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace SportDemo
{
    class Program
    {

        static async Task Main(string[] args)
        {
            TelegramBotClient botClient = new("5773662344:AAFntj8qjsRTha1E2OuWp2Lj4Tk0HzHebSc");
            try
            {
                var apiKey = "xopZf0uRQXtNKr3T";
                var apiKey1 = "xopZf0uRQXtNKr3T";
                var apiKey2 = "0taOGJ7GoxZ0CDbK";
                var apiKey3 = "iY0UqDC3WH0yny3p";
                var date = DateTime.Now;
                if (date.Hour >= 13 && date.Hour <= 24 || date.Hour >= 0 && date.Hour <= 4)
                {

                    switch (date.Hour)
                    {
                        case >= 0 and <= 4:
                            apiKey = apiKey1;
                            break;
                        case >= 13 and <= 20:
                            apiKey = apiKey2;
                            break;
                        default:
                            apiKey = apiKey3;
                            break;
                    }
                    var PjFolder1 = $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot" + "\\app_data\\jobs\\continuous\\sendmessagetele\\allsend.txt";
                    string urlLivescores = $"http://api.isportsapi.com/sport/football/livescores?api_key={apiKey}";
                    string rspLivescores = sendGet(urlLivescores);
                    var livescoreData = JsonConvert.DeserializeObject<LivescoresRoot>(rspLivescores);
                    if (livescoreData != null)
                    {
                        var listLive = livescoreData.Data?.Where(z => z.Status > 0).ToList();
                        if (listLive != null)
                        {
                            if (listLive.Any())
                            {

                                string urlOdds = $"http://api.isportsapi.com/sport/football/odds/main?api_key={apiKey}&companyId=12";
                                string rspOdds = sendGet(urlOdds);
                                var oddsData = JsonConvert.DeserializeObject<OddsRoot>(rspOdds);
                                if (oddsData != null)
                                {
                                    //var urlScheedule = $"http://api.isportsapi.com/sport/football/schedule?api_key={apiKey}&date={DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
                                    //string rspScheedule = sendGet(urlScheedule);
                                    //var ScheeduleData = JsonConvert.DeserializeObject<ScheduleRoot>(rspScheedule);
                                    //var listScheedule = ScheeduleData?.Data?.ToList();


                                    var listHandicap = oddsData.Data.Handicap;
                                    foreach (var live in listLive)
                                    {
                                        // var handicapMatchId = listHandicap.FirstOrDefault()
                                        var matchItemStr = listHandicap.FirstOrDefault(stringToCheck => stringToCheck.Contains(live.MatchId));
                                        if (matchItemStr != null)
                                        {
                                            string[] matchItem = matchItemStr.Split(",");
                                            if (matchItem.Length > 0)
                                            {

                                                //var scheduleItemStr = listScheedule?.FirstOrDefault(stringToCheck => stringToCheck.MatchId == live.MatchId);

                                                if (double.Parse(matchItem[2]) >= 0.75)
                                                {

                                                    if (live?.HomeScore < live?.AwayScore)
                                                    {
                                                        var ms = $"{UnixTimeStampToDateTime(live.MatchTime)} Home - {matchItem[2]} => {live?.HomeName} {live?.HomeScore} - {live?.AwayScore} {live?.AwayName}";
                                                        string allMs = System.IO.File.ReadAllText(PjFolder1);
                                                        if (allMs != null)
                                                        {
                                                            if (!allMs.Contains(ms))
                                                            {
                                                                var tmm = await botClient.SendTextMessageAsync(5506051750, $"{ms}");//5771751358
                                                                System.IO.File.AppendAllLines(PjFolder1, new[] { $"{ms}" });
                                                            }

                                                        }

                                                    }
                                                }
                                                else if (double.Parse(matchItem[2]) <= -0.75)
                                                {

                                                    if (live?.HomeScore > live?.AwayScore)
                                                    {
                                                        var ms = $"{UnixTimeStampToDateTime(live.MatchTime)} Away - {matchItem[2]} => {live?.HomeName} {live?.HomeScore} - {live?.AwayScore} {live?.AwayName}";
                                                        string allMs = System.IO.File.ReadAllText(PjFolder1);
                                                        if (allMs != null)
                                                        {
                                                            if (!allMs.Contains(ms))
                                                            {
                                                                var tmm = await botClient.SendTextMessageAsync(5506051750, $"{ms}");//5771751358
                                                                System.IO.File.AppendAllLines(PjFolder1, new[] { $"{ms}" });
                                                            }

                                                        }

                                                    }
                                                }


                                            }
                                        }

                                    }
                                }

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                var tmm = await botClient.SendTextMessageAsync(5506051750, $"{ex.Message}");//5771751358
                throw;
            }





        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        /// <summary>
        /// Send a Get request
        /// </summary>
        /// <param name="url">Request URL</param>
        /// <param name="parameters">Request parameter</param>
        /// <returns>Response content</returns>
        static string sendGet(string url)
        {
            // Create request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // GET request
            request.Method = "GET";
            request.ReadWriteTimeout = 5000;
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

            // Return content
            string retString = myStreamReader.ReadToEnd();
            return retString;




        }
    }
    #region schedule
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ScheduleDatum
    {
        [JsonProperty("matchId")]
        public string MatchId { get; set; }

        [JsonProperty("leagueType")]
        public int LeagueType { get; set; }

        [JsonProperty("leagueId")]
        public string LeagueId { get; set; }

        [JsonProperty("leagueName")]
        public string LeagueName { get; set; }

        [JsonProperty("leagueShortName")]
        public string LeagueShortName { get; set; }

        [JsonProperty("leagueColor")]
        public string LeagueColor { get; set; }

        [JsonProperty("subLeagueId")]
        public string SubLeagueId { get; set; }

        [JsonProperty("subLeagueName")]
        public string SubLeagueName { get; set; }

        [JsonProperty("matchTime")]
        public int MatchTime { get; set; }

        [JsonProperty("halfStartTime")]
        public int HalfStartTime { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("homeId")]
        public string HomeId { get; set; }

        [JsonProperty("homeName")]
        public string HomeName { get; set; }

        [JsonProperty("awayId")]
        public string AwayId { get; set; }

        [JsonProperty("awayName")]
        public string AwayName { get; set; }

        [JsonProperty("homeScore")]
        public int HomeScore { get; set; }

        [JsonProperty("awayScore")]
        public int AwayScore { get; set; }

        [JsonProperty("homeHalfScore")]
        public int HomeHalfScore { get; set; }

        [JsonProperty("awayHalfScore")]
        public int AwayHalfScore { get; set; }

        [JsonProperty("homeRed")]
        public int HomeRed { get; set; }

        [JsonProperty("awayRed")]
        public int AwayRed { get; set; }

        [JsonProperty("homeYellow")]
        public int HomeYellow { get; set; }

        [JsonProperty("awayYellow")]
        public int AwayYellow { get; set; }

        [JsonProperty("homeCorner")]
        public int HomeCorner { get; set; }

        [JsonProperty("awayCorner")]
        public int AwayCorner { get; set; }

        [JsonProperty("homeRank")]
        public string HomeRank { get; set; }

        [JsonProperty("awayRank")]
        public string AwayRank { get; set; }

        [JsonProperty("season")]
        public string Season { get; set; }

        [JsonProperty("stageId")]
        public string StageId { get; set; }

        [JsonProperty("round")]
        public string Round { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("weather")]
        public string Weather { get; set; }

        [JsonProperty("temperature")]
        public string Temperature { get; set; }

        [JsonProperty("explain")]
        public string Explain { get; set; }

        [JsonProperty("extraExplain")]
        public ScheduleExtraExplain ExtraExplain { get; set; }

        [JsonProperty("hasLineup")]
        public bool HasLineup { get; set; }

        [JsonProperty("neutral")]
        public bool Neutral { get; set; }

        [JsonProperty("injuryTime")]
        public int InjuryTime { get; set; }

        [JsonProperty("updateTime")]
        public int UpdateTime { get; set; }
    }

    public class ScheduleExtraExplain
    {
        [JsonProperty("kickOff")]
        public int KickOff { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("homeScore")]
        public int HomeScore { get; set; }

        [JsonProperty("awayScore")]
        public int AwayScore { get; set; }

        [JsonProperty("extraTimeStatus")]
        public int ExtraTimeStatus { get; set; }

        [JsonProperty("extraHomeScore")]
        public int ExtraHomeScore { get; set; }

        [JsonProperty("extraAwayScore")]
        public int ExtraAwayScore { get; set; }

        [JsonProperty("penHomeScore")]
        public int PenHomeScore { get; set; }

        [JsonProperty("penAwayScore")]
        public int PenAwayScore { get; set; }

        [JsonProperty("twoRoundsHomeScore")]
        public int TwoRoundsHomeScore { get; set; }

        [JsonProperty("twoRoundsAwayScore")]
        public int TwoRoundsAwayScore { get; set; }

        [JsonProperty("winner")]
        public int Winner { get; set; }
    }

    public class ScheduleRoot
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<ScheduleDatum> Data { get; set; }
    }
    #endregion


    #region Live Score
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class LivescoresDatum
    {
        [JsonProperty("matchId")]
        public string MatchId { get; set; }

        [JsonProperty("leagueType")]
        public int LeagueType { get; set; }

        [JsonProperty("leagueId")]
        public string LeagueId { get; set; }

        [JsonProperty("leagueName")]
        public string LeagueName { get; set; }

        [JsonProperty("leagueShortName")]
        public string LeagueShortName { get; set; }

        [JsonProperty("leagueColor")]
        public string LeagueColor { get; set; }

        [JsonProperty("subLeagueId")]
        public string SubLeagueId { get; set; }

        [JsonProperty("subLeagueName")]
        public string SubLeagueName { get; set; }

        [JsonProperty("matchTime")]
        public int MatchTime { get; set; }

        [JsonProperty("halfStartTime")]
        public int HalfStartTime { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("homeId")]
        public string HomeId { get; set; }

        [JsonProperty("homeName")]
        public string HomeName { get; set; }

        [JsonProperty("awayId")]
        public string AwayId { get; set; }

        [JsonProperty("awayName")]
        public string AwayName { get; set; }

        [JsonProperty("homeScore")]
        public int HomeScore { get; set; }

        [JsonProperty("awayScore")]
        public int AwayScore { get; set; }

        [JsonProperty("homeHalfScore")]
        public int HomeHalfScore { get; set; }

        [JsonProperty("awayHalfScore")]
        public int AwayHalfScore { get; set; }

        [JsonProperty("homeRed")]
        public int HomeRed { get; set; }

        [JsonProperty("awayRed")]
        public int AwayRed { get; set; }

        [JsonProperty("homeYellow")]
        public int HomeYellow { get; set; }

        [JsonProperty("awayYellow")]
        public int AwayYellow { get; set; }

        [JsonProperty("homeCorner")]
        public int HomeCorner { get; set; }

        [JsonProperty("awayCorner")]
        public int AwayCorner { get; set; }

        [JsonProperty("homeRank")]
        public string HomeRank { get; set; }

        [JsonProperty("awayRank")]
        public string AwayRank { get; set; }

        [JsonProperty("season")]
        public string Season { get; set; }

        [JsonProperty("stageId")]
        public string StageId { get; set; }

        [JsonProperty("round")]
        public string Round { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("weather")]
        public string Weather { get; set; }

        [JsonProperty("temperature")]
        public string Temperature { get; set; }

        [JsonProperty("explain")]
        public string Explain { get; set; }

        [JsonProperty("extraExplain")]
        public LivescoresExtraExplain ExtraExplain { get; set; }

        [JsonProperty("hasLineup")]
        public bool HasLineup { get; set; }

        [JsonProperty("neutral")]
        public bool Neutral { get; set; }

        [JsonProperty("injuryTime")]
        public int InjuryTime { get; set; }

        [JsonProperty("updateTime")]
        public int UpdateTime { get; set; }
    }

    public class LivescoresExtraExplain
    {
        [JsonProperty("kickOff")]
        public int KickOff { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("homeScore")]
        public int HomeScore { get; set; }

        [JsonProperty("awayScore")]
        public int AwayScore { get; set; }

        [JsonProperty("extraTimeStatus")]
        public int ExtraTimeStatus { get; set; }

        [JsonProperty("extraHomeScore")]
        public int ExtraHomeScore { get; set; }

        [JsonProperty("extraAwayScore")]
        public int ExtraAwayScore { get; set; }

        [JsonProperty("penHomeScore")]
        public int PenHomeScore { get; set; }

        [JsonProperty("penAwayScore")]
        public int PenAwayScore { get; set; }

        [JsonProperty("twoRoundsHomeScore")]
        public int TwoRoundsHomeScore { get; set; }

        [JsonProperty("twoRoundsAwayScore")]
        public int TwoRoundsAwayScore { get; set; }

        [JsonProperty("winner")]
        public int Winner { get; set; }
    }

    public class LivescoresRoot
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<LivescoresDatum> Data { get; set; }
    }


    #endregion

    #region Odds
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class OddsData
    {
        [JsonProperty("handicap")]
        public List<string> Handicap { get; set; }

        [JsonProperty("europeOdds")]
        public List<string> EuropeOdds { get; set; }

        [JsonProperty("overUnder")]
        public List<string> OverUnder { get; set; }

        [JsonProperty("handicapHalf")]
        public List<string> HandicapHalf { get; set; }

        [JsonProperty("overUnderHalf")]
        public List<string> OverUnderHalf { get; set; }
    }

    public class OddsRoot
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public OddsData Data { get; set; }
    }



    #endregion
}
