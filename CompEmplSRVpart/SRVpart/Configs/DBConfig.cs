// Author: Tymoshchuk Maksym
// Created On : 13/02/2024
// Last Modified On :
// Description: Класс для хранениея
//              конфигурации на подключение к БД
// Project: SRVpart

namespace SRVpart.Configs
{
    internal class DBConfig
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public string DBname { get; set; }

        public string UserName { get; set; }
    }
}
