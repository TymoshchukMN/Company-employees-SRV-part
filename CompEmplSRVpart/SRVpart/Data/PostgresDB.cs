using System;
using System.Collections.Generic;
using System.Data;
using Bogus;
using Bogus.DataSets;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using SRVpart.FakeDataGen;

namespace SRVpart.Data
{
    internal class PostgresDB : IDisposable
    {
        #region FIELDS
        
        private static PostgresDB _instance;
        private readonly string _connectionString;
        private readonly NpgsqlConnection _connection;
        private readonly string _server;
        private readonly string _dbName;
        private readonly int _port;

        #endregion FIELDS

        #region CTORs

        private PostgresDB(
            string server,
            string userName,
            string dataBase,
            int port)
        {
            _connectionString = string.Format(
                    $"Server={server};" +
                    $"Username={userName};" +
                    $"Database={dataBase};" +
                    $"Port={port};" +
                    $"Password={string.Empty}");
            _server = server;
            _dbName = dataBase;
            _port = port;

            _connection = new NpgsqlConnection(_connectionString);
            try
            {
                _connection.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        ~PostgresDB()
        {
            _connection.Close();
        }

        #endregion CTORs

        public static PostgresDB GetInstance(
            string server,
            string userName,
            string dataBase,
            int port)
        {
            if (_instance == null)
            {
                _instance = new PostgresDB(
                    server,
                    userName,
                    dataBase,
                    port);
            }

            return _instance;
        }

        public static PostgresDB GetInstance()
        {
            return _instance;
        }

        public void Dispose()
        {
            _connection.Close();
        }

        /// <summary>
        /// Получаем последний свободный номер карты.
        /// </summary>
        /// <param name="lastFreeVol">
        /// Последний номер карты.
        /// </param>
        public void GetLastFreeValue(out int lastFreeVol)
        {
            lastFreeVol = 0;
            NpgsqlCommand npgsqlCommand = _connection.CreateCommand();

            npgsqlCommand.CommandText = $"SELECT cardnumber FROM CARDS";

            NpgsqlDataReader data;
            data = npgsqlCommand.ExecuteReader();

            DataTable dataTable = new DataTable();
            dataTable.Load(data);

            if (dataTable.Rows.Count != 0)
            {
                data.Close();
                npgsqlCommand.CommandText = "Select max(cardnumber) as cardnumber from cards";
                data = npgsqlCommand.ExecuteReader();

                while (data.Read())
                {
                    lastFreeVol = (int)data["cardnumber"];
                }
            }

            data.Close();
        }

        /// <summary>
        /// Проверяем естьли карты с таким номером.
        /// </summary>
        /// <param name="cardNumber">
        /// Номер карты.
        /// </param>
        /// <returns>
        /// bool.
        /// </returns>
        public bool CheckIfCardExist(int cardNumber)
        {
            bool isExist = false;

            var queryArguments = new
            {
                card = cardNumber,
            };

            string sqlCommand = "SELECT COUNT(*) FROM cards WHERE cardnumber = @card";

            int count = _connection.QueryFirstOrDefault<int>(sqlCommand, queryArguments);

            if (count >= 1)
            {
                isExist = true;
            }

            return isExist;
        }

        /// <summary>
        /// Проверяем естьли карты с таким номером.
        /// </summary>
        /// <param name="phoneNumber">
        /// Номер карты.
        /// </param>
        /// <returns>
        /// bool.
        /// </returns>
        public bool CheckIfPhone(string phoneNumber)
        {
            bool isExist = false;

            var queryArguments = new
            {
                phone = phoneNumber,
            };

            string sqlCommand = "SELECT COUNT(*) FROM cards WHERE \"phoneNumber\" = @phone";

            int count = _connection.QueryFirstOrDefault<int>(sqlCommand, queryArguments);

            if (count >= 1)
            {
                isExist = true;
            }

            return isExist;
        }

        public void FillEmployeesTable(Name.Gender gender)
        {
            Faker faker = new Faker("ru");
            string query = string.Empty;
            Random random = new Random();

            // 10000 -значение по умолчанию, если таблица пустая
            int personnelNumber = 10000;

            NpgsqlCommand command = new NpgsqlCommand(query, _connection);

            query = "select \"PersonnelNumber\" from \"Employees\";";
            command.CommandText = query;
            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                personnelNumber = dr.GetInt32(0);
            }

            dr.Close();

            #region Заполнение таблицы Employees
            
            const int CountWorkers = 50;

            for (int i = 0; i < CountWorkers; i++)
            {
                ++personnelNumber;

                query = "INSERT INTO public.\"Employees\"(\"PersonnelNumber\", \"FirstName\", \"MiddleName\", \"Lastname\", \"isWorked\")" +
                    " VALUES(@Number, @FirstName, @MiddleName, @LastName, @isWorked);";

                string fullname = faker.Name.FullName(gender);

                string middleName = string.Empty;
                switch (gender)
                {
                    case Name.Gender.Male:
                        middleName =
                            ((MiddleNameMale)Enum.ToObject(
                            typeof(MiddleNameMale),
                            random.Next(1, 11))).ToString();
                        break;
                    case Name.Gender.Female:
                        middleName =
                            ((MiddleNameFemale)Enum.ToObject(
                                typeof(MiddleNameFemale),
                                random.Next(1, 11))).ToString();
                        break;
                }

                string firstName = fullname.Split(' ')[0];
                string lastName = fullname.Split(' ')[1];

                command = new NpgsqlCommand(query, _connection);
                command.Parameters.Add("@Number", NpgsqlDbType.Integer).Value = personnelNumber;
                command.Parameters.Add("@FirstName", NpgsqlDbType.Varchar).Value = firstName;
                command.Parameters.Add("@MiddleName", NpgsqlDbType.Varchar).Value = middleName;
                command.Parameters.Add("@LastName", NpgsqlDbType.Varchar).Value = lastName;
                
                if (i % 3 == 0)
                {
                    command.Parameters.Add("@isWorked", NpgsqlDbType.Bit).Value = false;
                }
                else
                {
                    command.Parameters.Add("@isWorked", NpgsqlDbType.Bit).Value = true;
                }

                command.ExecuteNonQuery();

                command.Dispose();
            }

            #endregion Заполнение таблицы Employees

        }

        public void FillBuisnessPhones()
        {
           
            string query = "select \"PersonnelNumber\" from \"Employees\";";
            NpgsqlCommand command = new NpgsqlCommand(query, _connection);

            command.CommandText = query;
            NpgsqlDataReader dr = command.ExecuteReader();

            List<int> personnelNumbers = new List<int>();

            while (dr.Read())
            {
                personnelNumbers.Add(dr.GetInt32(0));
            }

            dr.Close();

            PhoneNumerGenerator phoneNumerGenerator = PhoneNumerGenerator.GetInstance();
            foreach (int employee in personnelNumbers)
            {
                query = "INSERT INTO public.\"BusinessPhones\"(" +
                    "\"PersonnelNumber\", \"PhoneNumber\")" +
                    "VALUES(@Number, @Phone); ";

                command = new NpgsqlCommand(query, _connection);
                command.Parameters.Add(
                    "@Number",
                    NpgsqlDbType.Integer).Value = employee;

                command.Parameters.Add(
                    "@Phone",
                    NpgsqlDbType.Varchar, 15).Value = phoneNumerGenerator.GetNumber();

                try
                {
                    command.ExecuteNonQuery();
                }
                finally { }
            }
        }

        public void FillWorkPhones()
        {
            string query = "select \"PersonnelNumber\" from \"Employees\";";
            NpgsqlCommand command = new NpgsqlCommand(query, _connection);

            command.CommandText = query;
            NpgsqlDataReader dr = command.ExecuteReader();

            List<int> personnelNumbers = new List<int>();

            while (dr.Read())
            {
                personnelNumbers.Add(dr.GetInt32(0));
            }

            dr.Close();

            PhoneNumerGenerator phoneNumerGenerator = PhoneNumerGenerator.GetInstance();

            foreach (int employee in personnelNumbers)
            { 
                query = "SELECT COUNT(*) FROM \"BusinessPhones\" WHERE \"PhoneNumber\" = @phone";

                string number = phoneNumerGenerator.GetNumber();
                int count = _connection.QueryFirstOrDefault<int>(query, new
                {
                    phone = number,
                });

                if (count == 0)
                {
                    query = "INSERT INTO public.\"WorkPhone\"(" +
                   "\"PersonnelNumber\", \"WorkPhone\")" +
                   "VALUES(@Number, @Phone); ";

                    command = new NpgsqlCommand(query, _connection);

                    command.Parameters.Add(
                        "@Number",
                        NpgsqlDbType.Integer).Value = employee;

                    command.Parameters.Add(
                        "@Phone",
                        NpgsqlDbType.Varchar, 15).Value = number;

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    finally { }
                }
            }
        }
    }
}
