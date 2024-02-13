﻿using System.IO;
using Newtonsoft.Json;
using SRVpart.JSON;

namespace SRVpart
{
    internal class BL
    {
        /// <summary>
        /// Получить конфиг подключениея к БД.
        /// </summary>
        /// <returns>
        /// Конфиг подключение.
        /// </returns>
        public static DBConfigJSON GetDBConfig()
        {
            const string ConfFilePathDB = @"\\172.16.112.40\share\TymoshchukMN\DBEmployeesConfigFile.json";
            string dbConfigFile = File.ReadAllText(ConfFilePathDB);
            DBConfigJSON dbConfigJSON = JsonConvert.DeserializeObject<DBConfigJSON>(dbConfigFile);

            return dbConfigJSON;
        }
    }
}
