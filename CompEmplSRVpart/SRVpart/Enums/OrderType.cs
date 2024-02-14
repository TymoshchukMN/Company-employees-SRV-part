// Author: Tymoshchuk Maksym
// Created On : 14/02/2024
// Last Modified On :
// Description: перечисления для
//              определения условий
//              поиска сотрудников
// Project: SRVpart

using System.ComponentModel;

namespace SRVpart.Enums
{
    /// <summary>
    /// Критерий поиска сотрудников по дате.
    /// Больше указанной даты, или меньше.
    /// </summary>
    internal enum SearchCreteria
    {
        [Description("Меньше даты")]
        Less,

        [Description("Больше даты")]
        More,
    }
}
