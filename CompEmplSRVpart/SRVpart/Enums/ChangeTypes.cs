// Author: Tymoshchuk Maksym
// Created On : 14/02/2024
// Last Modified On :
// Description: перечисления
//              определения полей
//              для изменения
// Project: SRVpart

using System.ComponentModel;

namespace SRVpart.Enums
{
    internal enum ChangeTypes
    {
        [Description("Изменение фамилии")]
        LastName,

        [Description("Изменение должности")]
        Title,

        [Description("Изменение Бизнес-телефона")]
        PhoneNumber,

        [Description("Изменение Рабочего телефона")]
        WorkPhone,
    }
}
