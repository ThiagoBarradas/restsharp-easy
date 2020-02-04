namespace RestSharp.Easy.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new EasyRestClient("https://stglordof.mundipagg.com/customerservice/v1", requestKey: "12345");
        }
    }
}
