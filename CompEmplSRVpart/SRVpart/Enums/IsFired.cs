// Author: Tymoshchuk Maksym
// Created On : 15/02/2024
// Last Modified On :
// Description: перечисления уволен/работает
// Project: SRVpart

using System.ComponentModel;

namespace SRVpart.Enums
{
    internal enum IsFired : byte
    {
        [Description("Сотрудник работает")]
        Works,

        [Description("Уволенный сотрудник")]
        Fired,
    }
}
