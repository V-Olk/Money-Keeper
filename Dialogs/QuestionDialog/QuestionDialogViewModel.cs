using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOlkin.HelpClasses;
using VOlkin.ViewModels;

namespace VOlkin.Dialogs.QuestionDialog
{
    public class QuestionDialogViewModel : NotifyPropertyChanged
    {
        public QuestionDialogViewModel(string question)
        {
            Question = question;
        }

        public string Question { get; private set; }

    }
}
