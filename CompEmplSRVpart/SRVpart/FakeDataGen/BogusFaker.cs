// Author: Tymoshchuk Maksym
// Created On : 13/02/2024
// Last Modified On :
// Description: Заполнение новой базы фейковыми значениями
// Project: SRVpart

using System;
using Bogus.DataSets;
using SRVpart.Data;
using SRVpart.JSON;

namespace SRVpart.FakeDataGen
{
    internal class BogusFaker
    {
        public static void Run()
        {
            DBConfigJSON dBConfig = BL.GetDBConfig();

            PostgresDB pgDB = PostgresDB.GetInstance(
               dBConfig.DBConfig.Server,
               dBConfig.DBConfig.UserName,
               dBConfig.DBConfig.DBname,
               dBConfig.DBConfig.Port);

            pgDB.FillEmployeesTable(Name.Gender.Male);
            pgDB.FillEmployeesTable(Name.Gender.Female);
            pgDB.FillBuisnessPhones();
            pgDB.FillWorkPhones();
            pgDB.FillHistory();

            Console.ReadKey();
        }
    }
}
