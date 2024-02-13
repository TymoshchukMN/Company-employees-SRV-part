using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using Dapper;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
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

            // pgDB.FillEmployeesTable(Name.Gender.Male);
            // pgDB.FillEmployeesTable(Name.Gender.Female);
            // pgDB.FillBuisnessPhones();
            pgDB.FillWorkPhones();

            Console.ReadKey();
        }
    }
}
