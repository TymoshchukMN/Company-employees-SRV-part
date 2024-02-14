// Author: Tymoshchuk Maksym
// Created On : 13/02/2024
// Last Modified On :
// Description: Класс для рабоы с БД
// Project: SRVpart

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Bogus;
using Bogus.DataSets;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using SRVpart.Enums;
using SRVpart.FakeDataGen;
using SRVpart.Interfaces;

namespace SRVpart.Data
{
    internal class PostgresDB : IDisposable, IFilldtaBase
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

        #region IFilldtaBase

        /// <summary>
        /// Заполнение таблицы сотрудников.
        /// </summary>
        /// <param name="gender">
        /// Пол.
        /// </param>
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
            IdentCodeGenerator codeGenerator = IdentCodeGenerator.GetInstance();
            for (int i = 0; i < CountWorkers; i++)
            {
                ++personnelNumber;

                query = "INSERT INTO public.\"Employees\"(\"PersonnelNumber\", \"FirstName\", \"MiddleName\", \"Lastname\", \"isWorked\", \"identificationCode\")" +
                    " VALUES(@Number, @FirstName, @MiddleName, @LastName, @isWorked, @identificationCode);";

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
                command.Parameters.Add("@identificationCode", NpgsqlDbType.Integer).Value = codeGenerator.GetCode();
                
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

        /// <summary>
        /// Заполнение табоицы с бизнес-телефонами.
        /// </summary>
        public void FillBuisnessPhones()
        {
            const int VarcharLenth = 15;
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
                    NpgsqlDbType.Varchar,
                    VarcharLenth).Value = phoneNumerGenerator.GetNumber();

                try
                {
                    command.ExecuteNonQuery();
                }
                finally
                {
                }

                command.Dispose();
            }
        }

        /// <summary>
        /// Заполнение таблицы с рабочими номерами.
        /// </summary>
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
                        NpgsqlDbType.Varchar,
                        15).Value = number;

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Заполнение таблицы истории сотрудника.
        /// </summary>
        public void FillHistory()
        {
            string query = "SELECT \"PersonnelNumber\", \"isWorked\" " +
                " FROM \"Employees\";";

            NpgsqlCommand command = new NpgsqlCommand(query, _connection)
            {
                CommandText = query,
            };

            NpgsqlDataReader dr = command.ExecuteReader();
            Random random = new Random();

            Hashtable table = new Hashtable();
            while (dr.Read())
            {
                table.Add(dr.GetInt32(0), dr.GetBoolean(1));
            }

            dr.Close();

            foreach (DictionaryEntry item in table)
            {
                string title = GetRandomTitle(random);

                DateTime startDate;
                DateTime endDate;
                const int VarcharLenth = 15;

                // true - если сотрудник работает
                if ((bool)item.Value)
                {
                    startDate = DateTime.Today.AddMonths(-random.Next(1, 20));

                    command = new NpgsqlCommand();
                    command.Connection = _connection;
                    command.CommandText =
                        "INSERT INTO public.\"History\" " +
                        "(\"PersonnelNumber\", \"Title\", \"StartDate\") " +
                        " VALUES( @PersonnelNumber, @Title, @StartDate);";

                    command.Parameters.Add(
                        "@PersonnelNumber",
                        NpgsqlDbType.Integer).Value = item.Key;

                    command.Parameters.Add(
                        "@Title",
                        NpgsqlDbType.Varchar,
                        VarcharLenth).Value = title;

                    command.Parameters.Add(
                        "@StartDate",
                        NpgsqlDbType.Date).Value = startDate;

                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        Console.Write($"{item.Key}\t" +
                            $"{title}\t" +
                            $"{startDate}\t");

                        UI.PrintColorfull("Еще работает\n");
                    }

                    command.Dispose();
                }
                else
                {
                    endDate = DateTime.Today.AddMonths(-random.Next(1, 20));
                    startDate = endDate.AddMonths(-random.Next(1, 20));
                    command = new NpgsqlCommand();
                    command.Connection = _connection;
                    command.CommandText =
                        "INSERT INTO public.\"History\" " +
                        "(\"PersonnelNumber\", \"Title\", \"StartDate\", \"EndDate\") " +
                        "VALUES(@PersonnelNumber, @Title, @StartDate, @EndDate);";

                    command.Parameters.Add(
                        "@PersonnelNumber",
                        NpgsqlDbType.Integer).Value = item.Key;

                    command.Parameters.Add(
                        "@Title",
                        NpgsqlDbType.Varchar,
                        VarcharLenth).Value = title;

                    command.Parameters.Add(
                        "@StartDate",
                        NpgsqlDbType.Date).Value = startDate;

                    command.Parameters.Add(
                        "@EndDate",
                        NpgsqlDbType.Date).Value = endDate;
                    command.ExecuteNonQuery();
                    command.Dispose();

                    Console.Write($"{item.Key}" +
                            $"\t{title}\t" +
                            $"\t{startDate}\t" +
                            $"\t{endDate}\t");

                    UI.PrintColorfull("Уволен\n", ConsoleColor.Red);
                }

                command.Dispose();
            }
        }

        #endregion IFilldtaBase

        #region IDBprocessing

        /// <summary>
        /// Добавление сотрудника в БД.
        /// </summary>
        /// <param name="employee">
        /// Сотрудник.
        /// </param>
        /// <returns>
        /// Результат операции.
        /// </returns>
        public OperationsResults CreateEmployee(Employee employee)
        {
            OperationsResults operationsResults = OperationsResults.None;

            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = _connection;

            // прверка есть ли в базе сотрудник с таким идентификационным кодом
            string query = 
                "Select count(*) from  \"Employees\" " +
                "where \"identificationCode\" = @code;";

            command.CommandText = query;
            command.Parameters.Add(
                "@code",
                NpgsqlDbType.Integer).Value = employee.IdentificationCode;

            int countWors = command.ExecuteNonQuery();

            // в БД нет сотрудника с указанным ИНН. Добавляем
            if (countWors == 0)
            {
                query = "select \"PersonnelNumber\" from \"Employees\";";
                command.CommandText = query;
                NpgsqlDataReader dr = command.ExecuteReader();
                int affectedRows = 0;
                int personnelNumber = 0;
                while (dr.Read())
                {
                    personnelNumber = dr.GetInt32(0);
                }

                dr.Close();

                query = "INSERT INTO public.\"Employees\"(" +
                        "\"PersonnelNumber\", \"FirstName\", \"MiddleName\", \"Lastname\", \"isWorked\")" +
                        " VALUES(@Number, @FirstName, @MiddleName, @LastName, @isWorked);" +

                        "INSERT INTO public.\"BusinessPhones\"(" +
                        "\"PersonnelNumber\", \"PhoneNumber\")" +
                        "VALUES(@Number, @Phone); " +

                        "INSERT INTO public.\"WorkPhone\"(" +
                        "\"PersonnelNumber\", \"WorkPhone\")" +
                        "VALUES(@Number, @WorkPhone); ";

                const int VarcharSize = 15;
                command.Parameters.Add(
                    "@Number",
                    NpgsqlDbType.Integer).Value = ++personnelNumber;

                command.Parameters.Add(
                    "@FirstName",
                    NpgsqlDbType.Varchar,
                    VarcharSize).Value = employee.FirstName;

                command.Parameters.Add(
                    "@MiddleName",
                    NpgsqlDbType.Varchar,
                    VarcharSize).Value = employee.MiddleName;

                command.Parameters.Add(
                    "@LastName",
                    NpgsqlDbType.Varchar,
                    VarcharSize).Value = employee.LastName;

                command.Parameters.Add(
                    "@isWorked",
                    NpgsqlDbType.Bit).Value = (byte)0b1;

                command.Parameters.Add(
                    "@Phone",
                    NpgsqlDbType.Varchar,
                    VarcharSize).Value = employee.PhoneNumber;

                if (employee.WorkPhone.Count == 1)
                {
                    command.Parameters.Add(
                        "@WorkPhone",
                        NpgsqlDbType.Varchar,
                        VarcharSize).Value = employee.WorkPhone[0];

                    affectedRows = command.ExecuteNonQuery();
                }
                else
                {
                    if (employee.WorkPhone.Count > 1)
                    {
                        affectedRows = command.ExecuteNonQuery();

                        for (int i = 1; i < employee.WorkPhone.Count; i++)
                        {
                            query = "INSERT INTO public.\"WorkPhone\"(" +
                               "\"PersonnelNumber\", \"WorkPhone\")" +
                               "VALUES(@Number, @Phone); ";

                            command.Parameters.Add(
                                "@WorkPhone",
                                NpgsqlDbType.Varchar,
                                VarcharSize).Value = employee.WorkPhone[i];

                            command.ExecuteNonQuery();
                        }
                    }
                }

                if (affectedRows > 0)
                {
                    operationsResults = OperationsResults.Success;
                }

                command.Dispose();
            }
            else // if (contWors == 0)
            {
                query =
                    "SELECT \"isWorked\" " +
                    "FROM \"Employees\" WHERE \"identificationCode\" = @code;";
                command.CommandText = query;
                command.Parameters.Add(
                    "@code",
                    NpgsqlDbType.Integer).Value = employee.IdentificationCode;

                var reader = command.ExecuteReader();
                bool isWork = false;
                
                while (reader.Read())
                {
                    isWork = reader.GetBoolean(0);
                }

                if (!isWork)
                {
                    #region ОБНОВЛЕНИЕ ДАННЫХ Сотрудка который ранее работал

                    // принимаем на работу и ОБНОВЛЯЕМ ДАННЫЕ сотрудника
                    query =
                        "UPDATE \"Employees\" " +
                        "SET \"isWorked\" = @isWorked " +
                        "WHERE \"PersonnelNumber\" = @PersonnelNumber;";
                    command.CommandText = query;
                    command.Parameters.Add("@isWorked", NpgsqlDbType.Boolean).Value = true;

                    command.ExecuteNonQuery();

                    const int VarcharSize = 15;
                    query =
                        "INSERT INTO \"History\"(\"PersonnelNumber\", \"Title\", \"StartDate\", \"EndDate\") " +
                        "VALUES(@PersonnelNumber, @Title, @StartDate);";

                    command.CommandText = query;
                    command.Parameters.Add(
                        "@Number",
                        NpgsqlDbType.Integer).Value = employee.PersonnelNumber;

                    command.Parameters.Add(
                        "@Title",
                        NpgsqlDbType.Varchar,
                        VarcharSize).Value = employee.Title;

                    command.Parameters.Add(
                        "@StartDate",
                        NpgsqlDbType.Date).Value = employee.EmploymentDate;

                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        operationsResults = OperationsResults.Success;
                    }

                    #endregion ОБНОВЛЕНИЕ ДАННЫХ Сотрудка который ранее работал
                }
                else
                {
                    operationsResults = OperationsResults.AlreadyWorking;

                    Console.WriteLine("Уже работет");
                }

                return operationsResults;
            }

            return operationsResults;
        }

        /// <summary>
        /// Увольнеие сотрудника.
        /// </summary>
        /// <param name="employe">
        /// Сотрудник.
        /// </param>
        /// <returns>
        /// Рузультат операции.
        /// </returns>
        public OperationsResults FireEmployee(Employee employe)
        {
            OperationsResults operationsResults = OperationsResults.None;

            NpgsqlCommand command = new NpgsqlCommand();

            string query =
                "Select count(*) from  \"Employees\" " +
                "where \"PersonnelNumber\" = @PersonnelNumber;";

            int count = _connection.QueryFirstOrDefault<int>(query, new
            {
                PersonnelNumber = employe.PersonnelNumber,
            });

            if (count > 0)
            {
                query =
                       "UPDATE \"Employees\" " +
                       "SET \"isWorked\" = @isWorked " +
                       "WHERE \"PersonnelNumber\" = @PersonnelNumber;";
                command.CommandText = query;
                command.Parameters.Add("@isWorked", NpgsqlDbType.Boolean).Value = false;

                command.ExecuteNonQuery();

                query =
                    "UPDATE \"History\" " +
                    "SET \"EndDate\" =? " +
                    "WHERE \"EndDate\" is NULL AND \"PersonnelNumber\" = @PersonnelNumber;";

                command.CommandText = query;
                command.Parameters.Add(
                    "@PersonnelNumber",
                    NpgsqlDbType.Integer).Value = employe.PersonnelNumber;
                int affectedRows = command.ExecuteNonQuery();
                
                if (affectedRows > 0)
                {
                    operationsResults = OperationsResults.Success;
                }
                else
                {
                    operationsResults = OperationsResults.NotChanged;
                }
            }
            else
            {
                operationsResults = OperationsResults.DoestExist;
            }

            command.Dispose();
            return operationsResults;
        }

        /// <summary>
        /// Получить пользователя по табельному номеру.
        /// </summary>
        /// <param name="employee">
        /// Класс для маппинга.
        /// </param>
        /// <param name="personnelNumber">
        /// Табельный номер.
        /// </param>
        /// <returns>
        /// Резуотат выполнения операции.
        /// </returns>
        public OperationsResults GetEmployee(out Employee employee, int personnelNumber)
        {
            OperationsResults operationsResults = OperationsResults.None;
            string query =
                "Select emp.\"PersonnelNumber\"," +
                "emp.\"FirstName\"," +
                "emp.\"MiddleName\"," +
                "emp.\"isWorked\"," +
                "emp.\"identificationCode\"," +
                "wp.\"WorkPhone\"," +
                "bp.\"PhoneNumber\" " +
                "from \"Employees\" as emp " +
                "inner join \"BusinessPhones\" as bp on emp.\"PersonnelNumber\" = bp.\"PersonnelNumber\" " +
                "inner join \"WorkPhone\" as wp on emp.\"PersonnelNumber\" = wp.\"PersonnelNumber\" " +
                "inner join \"History\" as his on emp.\"PersonnelNumber\" = his.\"PersonnelNumber\"" +
                "Where emp.\"PersonnelNumber\" = @PersonnelNumber;";

            employee = (Employee)_connection.Query<Employee>(
                query,
                new { PersonnelNumber = personnelNumber });

            if (employee != null)
            {
                operationsResults = OperationsResults.Success;
            }
            else
            {
                operationsResults = OperationsResults.DidtGet;
            }

            return operationsResults;
        }

        /// <summary>
        /// Получение сотрудника по номеру телефона.
        /// </summary>
        /// <param name="employee">
        /// Сласс сотрудника для маппинга.
        /// </param>
        /// <param name="phoneTypes">
        /// Тип телефона, рабочий/бизнес.
        /// </param>
        /// <param name="phoneNumber">
        /// номер телефона.
        /// </param>
        /// <returns>
        /// Резульат операции.
        /// </returns>
        public OperationsResults GetEmployee(
            out Employee employee,
            PhoneTypes phoneTypes,
            string phoneNumber)
        {
            OperationsResults operationsResults = OperationsResults.None;
            string query = string.Empty;
            switch (phoneTypes)
            {
                case PhoneTypes.Buisbusiness:

                    query =
                        "Select emp.\"PersonnelNumber\"," +
                        "emp.\"FirstName\"," +
                        "emp.\"MiddleName\"," +
                        "emp.\"isWorked\"," +
                        "emp.\"identificationCode\"," +
                        "wp.\"WorkPhone\"," +
                        "bp.\"PhoneNumber\" " +
                        "from \"Employees\" as emp " +
                        "inner join \"BusinessPhones\" as bp on emp.\"PersonnelNumber\" = bp.\"PersonnelNumber\" " +
                        "inner join \"WorkPhone\" as wp on emp.\"PersonnelNumber\" = wp.\"PersonnelNumber\" " +
                        "inner join \"History\" as his on emp.\"PersonnelNumber\" = his.\"PersonnelNumber\"" +
                        "Where bp.\"PhoneNumber\" ='@PhoneNumber';";

                    break;
                case PhoneTypes.Work:

                    query =
                        "Select emp.\"PersonnelNumber\"," +
                        "emp.\"FirstName\"," +
                        "emp.\"MiddleName\"," +
                        "emp.\"isWorked\"," +
                        "emp.\"identificationCode\"," +
                        "wp.\"WorkPhone\"," +
                        "bp.\"PhoneNumber\" " +
                        "from \"Employees\" as emp " +
                        "inner join \"BusinessPhones\" as bp on emp.\"PersonnelNumber\" = bp.\"PersonnelNumber\" " +
                        "inner join \"WorkPhone\" as wp on emp.\"PersonnelNumber\" = wp.\"PersonnelNumber\" " +
                        "inner join \"History\" as his on emp.\"PersonnelNumber\" = his.\"PersonnelNumber\"" +
                        "Where wp.\"WorkPhone\" ='@PhoneNumber';";

                    break;
            }

            employee = (Employee)_connection.Query<Employee>(
                query,
                new { PhoneNumber = phoneNumber });

            if (employee != null)
            {
                operationsResults = OperationsResults.Success;
            }
            else
            {
                operationsResults = OperationsResults.DidtGet;
            }

            return operationsResults;
        }
        #endregion IDBprocessing

        /// <summary>
        /// Получени ерандомной должности.
        /// </summary>
        /// <param name="random">
        /// random.
        /// </param>
        /// <returns>Название должности.</returns>
        private string GetRandomTitle(Random random)
        {
            System.Threading.Thread.Sleep(15);
            Titles titleEnum = (Titles)Enum.ToObject(
                      typeof(Titles),
                      random.Next(1, 16));

            var title = titleEnum.GetType().GetField(titleEnum.ToString());
            var titleDescr =
                (DescriptionAttribute)Attribute.GetCustomAttribute(
                    title,
                    typeof(DescriptionAttribute));
            return titleDescr.Description;
        }
    }
}
