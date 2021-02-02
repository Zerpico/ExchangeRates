using System;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCbr
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            MainAsync().GetAwaiter().GetResult();
           

            Console.WriteLine("Hello World!");
        }

        private static async Task MainAsync()
        {
            ConnectionCbr connectionCbr = new ConnectionCbr();
            var models = await connectionCbr.GetValuteModel();
            var modelNominals = await connectionCbr.GetValuteNominals(new DateTime(2001,03,02),new DateTime(2001, 03, 14), "R01235");
        }
    }
}
