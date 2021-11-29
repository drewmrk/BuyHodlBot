using System.Collections.Specialized;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using Tweetinvi;

Console.WriteLine("BuyHodlBot");

TwitterAPI twitterAPI = new();

Console.WriteLine("\nTwitter API\n");
Console.Write("Consumer Key: ");
twitterAPI.consumerKey = Console.ReadLine();
Console.Write("Consumer Secret: ");
twitterAPI.consumerSecret = Console.ReadLine();
Console.Write("Access Key: ");
twitterAPI.accessKey = Console.ReadLine();
Console.Write("Access Secret: ");
twitterAPI.accessSecret = Console.ReadLine();

CoinMarketCapAPI coinMarketCapAPI = new();
Console.WriteLine("\nCoinMarketCap API\n");
Console.Write("Key: ");
coinMarketCapAPI.key = Console.ReadLine();

Cryptocurrency cryptocurrency = new();
Console.WriteLine("\nCryptocurrency\n");
Console.Write("Name: ");
cryptocurrency.name = Console.ReadLine();
Console.Write("Ticker: ");
cryptocurrency.ticker = Console.ReadLine();

Console.WriteLine($"\nUsing: {cryptocurrency.name} ({cryptocurrency.ticker})");

/// <summary>
///   Get the price of the specified cryptocurrency
/// </summary>
string GetCryptocurrencyPrice(string key, string ticker) {
  UriBuilder URL = new("https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest");
  NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
  queryString["convert"] = "USD";
  queryString["symbol"] = ticker;

  URL.Query = queryString.ToString();

  WebClient client = new();
  client.Headers.Add("X-CMC_PRO_API_KEY", key);
  client.Headers.Add("Accepts", "application/json");

  JObject data = JObject.Parse(client.DownloadString(URL.ToString()));

  float price = (float)data["data"][ticker]["quote"]["USD"]["price"];

  return price.ToString("N10");
}

/// <summary>
///   Tweet the current price of the specified cryptocurrency
/// </summary>
async Task TweetMessage(string message, TwitterClient twitterClient) {
  try {
    Tweetinvi.Models.ITweet tweet = await twitterClient.Tweets.PublishTweetAsync(message);
  } catch {
    Console.WriteLine("An error occurred while tweeting");
  }
}

TwitterClient twitterClient = new(twitterAPI.consumerKey, twitterAPI.consumerSecret,
                                  twitterAPI.accessKey, twitterAPI.accessSecret);

while (true) {
  Thread.Sleep(30000);

  string message =
      $@"
${GetCryptocurrencyPrice(coinMarketCapAPI.key, cryptocurrency.ticker)}

#crypto #cryptocurrency #{cryptocurrency.ticker} #{cryptocurrency.name} ${cryptocurrency.ticker} #SHIBARMY
";
  Console.WriteLine(message);

  await TweetMessage(message, twitterClient);
}
