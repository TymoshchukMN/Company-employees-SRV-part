// Author: Tymoshchuk Maksym
// Created On : 14/02/2024
// Last Modified On :
// Description: Перечисления типов телефона.
// Project: SRVpart

using System.ComponentModel;

namespace SRVpart.Enums
{
    internal enum PhoneTypes
    {
        [Description("Значение по-молчанию")]
        None,

        [Description("Бизнес-телефон")]
        Buisbusiness,

        [Description("Рабочий телефон")]
        Work,
    }
}
