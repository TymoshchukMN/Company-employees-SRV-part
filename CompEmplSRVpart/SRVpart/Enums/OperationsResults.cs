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
    }
}
