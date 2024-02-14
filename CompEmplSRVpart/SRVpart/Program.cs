using SRVpart.FakeDataGen;

namespace SRVpart
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // BogusFaker.Run(); // -наполнение БД фейковыми данными
            Starter.Run();
        }
    }
}