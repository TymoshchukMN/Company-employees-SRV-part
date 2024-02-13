using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRVpart.FakeDataGen
{
    internal class PhoneNumerGenerator
    {
        private static PhoneNumerGenerator _instance;
        private HashSet<string> _numbers = new HashSet<string>();

        private PhoneNumerGenerator()
        {
            FillHashSet();
        }

        public HashSet<string> Numbers
        {
            get
            {
                return _numbers;
            }
        }

        public static PhoneNumerGenerator GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PhoneNumerGenerator();
                return _instance;
            }
            else
            {
                return _instance;
            }
        }

        /// <summary>
        /// Получить номер телефона.
        /// </summary>
        /// <returns>
        /// Телефон в виде строки.
        /// </returns>
        public string GetNumber()
        {
            string num = _numbers.First();
            RemovePhone(num);
            return num;
        }

        /// <summary>
        /// Удаление использованного телефона из коллекции.
        /// </summary>
        /// <param name="number">
        /// Номер для удаления.
        /// </param>
        private void RemovePhone(string number)
        {
            _numbers.Remove(number);
        }

        /// <summary>
        /// Сгенерировать номера телефонов.
        /// </summary>
        private void FillHashSet()
        {
            StringBuilder stringBuilder = new StringBuilder();
            const ushort CollectionSize = 500;
            Random random = new Random();
            
            const string Life = "38093";
            const string Kievstar = "38097";
            const string MTC = "38095";

            for (ushort i = 0; i < CollectionSize; ++i)
            {
                PhoneOperators oper = 
                    (PhoneOperators)Enum.ToObject(
                        typeof(PhoneOperators),
                        random.Next(1, 3));

                switch (oper)
                {
                    case PhoneOperators.Life:
                        stringBuilder.Append(Life);

                        break;
                    case PhoneOperators.Kievstar:
                        stringBuilder.Append(Kievstar);

                        break;
                    case PhoneOperators.MTC:
                        stringBuilder.Append(MTC);

                        break;
                }

                stringBuilder.Append(random.Next(1000000, 9999999));
                _numbers.Add(stringBuilder.ToString());
                stringBuilder.Clear();
            }
        }
    }
}
