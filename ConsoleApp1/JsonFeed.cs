using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConsoleApp1
{	

	class JsonFeed
    {
        static string _url = "";
		static int _number = 1;

		static ILogger _logger;

		public JsonFeed(ILogger<JsonFeed> logger)
		{
			_logger = logger;
		}

		public JsonFeed(string endpoint, int results)
        {
            _url = endpoint;
			_number = results;
		}       


        public static string[] GetRandomJokes(string firstname, string lastname, string category)
		{
			_logger.LogInformation($"Generating random jokes with firstname {firstname} lastname {lastname} and category {category} ");
			string[] jokes = new string[_number];

			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(_url);
			string url = "jokes/random";
			if (category != null)
			{
				if (url.Contains('?'))
					url += "&";
				else url += "?";
				url += "category=";
				url += category;
			}

			for (int i = 0; i < _number; i++)
			{
				string joke = Task.FromResult(client.GetStringAsync(url).Result).Result;

				if (firstname != null && lastname != null)
				{
					int index = joke.IndexOf("Chuck Norris");
					string firstPart = joke.Substring(0, index);
					string secondPart = joke.Substring(0 + index + "Chuck Norris".Length, joke.Length - (index + "Chuck Norris".Length));
					joke = firstPart + " " + firstname + " " + lastname + secondPart;
				}
				jokes[i]=(JsonConvert.DeserializeObject<dynamic>(joke).value);
			}
			return jokes; 
        }

        /// <summary>
        /// returns an object that contains name and surname
        /// </summary>
        /// <param name="client2"></param>
        /// <returns></returns>
		public static dynamic Getnames()
        {
			try
			{
				HttpClient client = new HttpClient();
				client.BaseAddress = new Uri(_url);
				var result = client.GetStringAsync("").Result;
				return JsonConvert.DeserializeObject<dynamic>(result);
			}
			catch (Exception ex)
			{
				_logger.LogInformation($"Error in Getnames {ex.Message}");
				return null;
			}
		}

		public static string[] GetCategories()
        {
			try
			{
				HttpClient client = new HttpClient();
				client.BaseAddress = new Uri(_url);
				var result = Task.FromResult(client.GetStringAsync("jokes/categories").Result).Result;
				return JsonConvert.DeserializeObject<string[]>(result);
			}
			catch(Exception ex)
            {
				_logger.LogInformation($"Error in GetCategories {ex.Message}");
				return null;
			}           
		}
    }
}
