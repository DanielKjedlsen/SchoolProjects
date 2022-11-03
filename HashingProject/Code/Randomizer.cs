using System;
using System.Numerics;
using System.Collections.Generic;
using System.Text;

namespace RAD
{
    public static class Randomizer
    {
        public static ulong getRandomUlong()
        {
            var response = UrlReader.ReadResponseFromUrl("https://www.random.org/cgi-bin/randbyte?nbytes=8&format=b");
            string responseWithoutSpaces = String.Concat(response.Where(c => !Char.IsWhiteSpace(c)));
            string unevenResponseWithoutSpaces = responseWithoutSpaces.Substring(0,responseWithoutSpaces.Length-1) + "1";
            ulong a = Convert.ToUInt64(unevenResponseWithoutSpaces, 2);
            return a;
        }

        public static BigInteger getRandomBigInt()
        {
            var response = UrlReader.ReadResponseFromUrl("https://www.random.org/cgi-bin/randbyte?nbytes=12&format=b");
            string responseWithoutSpaces = String.Concat(response.Where(c => !Char.IsWhiteSpace(c)));
            BigInteger p = new BigInteger(Math.Pow(2, 89)) - 1;
            BigInteger a = new BigInteger(Encoding.ASCII.GetBytes(responseWithoutSpaces));
            a = a&p;
            if (a == p) return getRandomBigInt();
            else return a;
        }
    }


    public class UrlReader
    {
        public static string ReadResponseFromUrl(string url)
        {
            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(url)
            };
            using (var response = httpClient.GetAsync(url))
            {
                string responseBody = response.Result.Content.ReadAsStringAsync().Result;
                return responseBody;
            }
        }
    }
}