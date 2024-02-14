// Author: Tymoshchuk Maksym
// Created On : 14/02/2024
// Last Modified On :
// Description: Логирование
// Project: SRVpart

using System.Collections.Generic;

namespace SRVpart
{
    internal class Logger
    {
        private static Logger _instance;
        private List<string> _logs;

        private Logger()
        {
        }
        
        public List<string> Logs
        {
            get { return _logs; }
        }

        public static Logger GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Logger();
                return _instance;
            }
            else
            {
                return _instance;
            }
        }
        
        /// <summary>
        /// Добавление логов в коллектор.
        /// </summary>
        /// <param name="log">
        /// строка логов.
        /// </param>
        public void AddLog(string log)
        {
            _logs.Add(log);
        }
    }
}
