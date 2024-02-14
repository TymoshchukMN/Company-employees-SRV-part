// Author: Tymoshchuk Maksym
// Created On : 14/02/2024
// Last Modified On :
// Description: Генерация идентификационных номеров
//              для наполнения базы фейковыми данными
// Project: SRVpart

using System;
using System.Collections.Generic;
using System.Linq;

namespace SRVpart.FakeDataGen
{
    internal class IdentCodeGenerator
    {
        private static IdentCodeGenerator _instance;

        private HashSet<int> _codes = new HashSet<int>();
       
        private IdentCodeGenerator()
        {
            FillPool();
        }

        public HashSet<int> MyProperty
        {
            get { return _codes; }
            set { _codes = value; }
        }

        public static IdentCodeGenerator GetInstance()
        {
            if (_instance == null)
            {
                _instance = new IdentCodeGenerator();
                return _instance;
            }
            else
            {
                return _instance;
            }
        }

        public int GetCode()
        {
            int code = _codes.First();
            _codes.Remove(code);
            return code;
        }

        private void FillPool()
        {
            const int PoolSize = 300;
            const int StartRand = 10000000;
            const int EndRand = 100000000;

            Random random = new Random();

            do
            {
                _codes.Add(random.Next(StartRand, EndRand));
            }
            while (_codes.Count <= PoolSize);  
        }
    }
}
