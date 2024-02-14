using System.ComponentModel;

namespace SRVpart.Enums
{
    internal enum OperationsResults : byte
    {
        None,

        [Description("Операция успешна.")]
        Success,

        [Description("Сотрудник уже работает")]
        AlreadyWorking,

        [Description("Изменения не внесены")]
        NotChanged,

        [Description("Сотрудника несуществует")]
        DoestExist,

        [Description("Данные не получены")]
        DidtGet,
    }
}
