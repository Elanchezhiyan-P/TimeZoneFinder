using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TimeZoneFinder
{
    internal class Program
    {

        public readonly static string GEOCODE_URL = "https://www.mapquestapi.com/geocoding/v1/address";
        public readonly static string GEOCODE_APIKEY = "xxxxx";
        public readonly static string TIMEZONE_URL = "http://api.timezonedb.com/v2/get-time-zone";
        public readonly static string TIMEZONE_APIKEY = "xxxxx";
        static async Task Main(string[] args)
        {
           var latLng = await GetTimeZone("94103");
            
        }

        public static async Task<string> GetTimeZone(string ZipCode)
        {
            string lat = string.Empty, lng = string.Empty, timeZoneName = string.Empty;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var uriBuilder = new UriBuilder(GEOCODE_URL);
                    var queryParameters = HttpUtility.ParseQueryString(string.Empty);
                    queryParameters.Add("key", GEOCODE_APIKEY);
                    queryParameters.Add("postalCode", ZipCode);

                    uriBuilder.Query = queryParameters.ToString();

                    var response = await httpClient.GetAsync(uriBuilder.ToString());

                    // Ensure the request was successful
                    response.EnsureSuccessStatusCode();

                    // Read the response content
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<GeocodeData>(content);

                    lat = result.results.FirstOrDefault().locations.FirstOrDefault().latLng.lat.ToString();
                    lng = result.results.FirstOrDefault().locations.FirstOrDefault().latLng.lng.ToString();                  
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return string.Empty;
                }
            }
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var uriBuilder = new UriBuilder(TIMEZONE_URL);
                    var queryParameters = HttpUtility.ParseQueryString(string.Empty);
                    queryParameters.Add("key", TIMEZONE_APIKEY);
                    queryParameters.Add("format", "json");
                    queryParameters.Add("by", "position");
                    queryParameters.Add("fields", "zoneName");
                    queryParameters.Add("lat", lat);
                    queryParameters.Add("lng", lng);

                    uriBuilder.Query = queryParameters.ToString();

                    var response = await httpclient.GetAsync(uriBuilder.ToString());
                    
                    response.EnsureSuccessStatusCode();

                    // Read the response content
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<TimeZoneData>(content);

                    timeZoneName = result.zoneName;
                }
                catch
                {
                    timeZoneName = string.Empty;
                }
                return timeZoneName;
            }
        }
    }
}
