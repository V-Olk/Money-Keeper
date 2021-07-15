using System.ComponentModel;
using System.Windows.Markup;

namespace VOlkin.HelpClasses.Enums
{
    public enum CategoryTypeEnum
    {
        [Description("Расход")]
        Expense,
        [Description("Доход")]
        Income
    }
}
