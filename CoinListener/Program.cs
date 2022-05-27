using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using System;
using System.Globalization;

namespace CoinListener // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static bool listen = true;

        static BinanceClient binanceClient = new BinanceClient(new BinanceClientOptions
        {
            

            ApiCredentials = new ApiCredentials(Environment.GetEnvironmentVariable("binance_api"), Environment.GetEnvironmentVariable("binance_secret")),


        });
        static async Task Main(string[] args)
        {
            //string[] args = new string[4];
            //args[0] = "BTCUSDT";
            //args[1] = "50000";
            //args[2] = "50000";
            //args[3] = "10";


            while (listen)
            {
                var result = await binanceClient.SpotApi.ExchangeData.GetPriceAsync(args[0]);

             

                Console.WriteLine(args[0] +" price is: "+ result.Data.Price);

                Console.WriteLine("Buy and sell limits are "+ args[1]+" - "+ args[2]);

                switch (result.Data.Price)
                {
                    case var value when value <= decimal.Parse(args[1], CultureInfo.InvariantCulture):
                        // code block
                        var orderResultBuy = await binanceClient.SpotApi.Trading.PlaceOrderAsync(
                        args[0],
                        OrderSide.Buy,
                        SpotOrderType.Market,
                        quoteQuantity: Int64.Parse(args[3]));
                        listen = false;
                        if (!orderResultBuy.Success)
                        {
                            Console.WriteLine("Buy order failed, check the funds in your wallet.");
                        }
                        break;
                    case var value when value >= decimal.Parse(args[2], CultureInfo.InvariantCulture):
                        // code block

                        var orderResultSell = await binanceClient.SpotApi.Trading.PlaceOrderAsync(
                        args[0],
                        OrderSide.Sell,
                        SpotOrderType.Market,
                        quoteQuantity: Int64.Parse(args[3]));
                        listen = false;
                        if (!orderResultSell.Success)
                        {
                            Console.WriteLine("Sell order failed, check the tokens in your wallet.");
                        }
                        break;
                
                }
                System.Threading.Thread.Sleep(1000);

                
            }

        }
    }
}