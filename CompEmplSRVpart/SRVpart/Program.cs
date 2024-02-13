using SRVpart.FakeDataGen;

namespace SRVpart
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BogusFaker.Run();
            Starter.Run();
        }
    }
}
