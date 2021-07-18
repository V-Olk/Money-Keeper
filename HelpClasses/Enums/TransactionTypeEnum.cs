using System.ComponentModel;

namespace VOlkin.HelpClasses.Enums
{
    public enum TransactionTypeEnum
    {
        [Description("Расход")]
        Expense,
        [Description("Доход")]
        Income,
        [Description("Перевод")]
        Transfer
    }
}
