using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOlkin.Dialogs.QuestionDialog
{
    public class QuestionDialogViewModel
    {
        public QuestionDialogViewModel(string question)
        {
            Question = question;
        }
        public string Question { get; set; }
    }
}
