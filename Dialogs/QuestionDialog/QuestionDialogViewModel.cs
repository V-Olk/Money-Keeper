using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOlkin.ViewModels;

namespace VOlkin.Dialogs.QuestionDialog
{
    public class QuestionDialogViewModel : ViewModelBase
    {
        public string Question { get; private set; }

        public QuestionDialogViewModel(string question)
        {
            Question = question;
        }
    }
}
