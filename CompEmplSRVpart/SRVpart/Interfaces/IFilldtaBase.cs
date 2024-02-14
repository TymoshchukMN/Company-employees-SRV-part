using Bogus.DataSets;

namespace SRVpart.Interfaces
{
    internal interface IFilldtaBase
    {
        void FillEmployeesTable(Name.Gender gender);

        void FillBuisnessPhones();

        void FillWorkPhones();

        void FillHistory();
    }
}