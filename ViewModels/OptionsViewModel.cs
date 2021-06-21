using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VOlkin.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private bool _isDarkThemeOn = Properties.Settings.Default.IsDarkThemeOn;//new PaletteHelper().GetTheme().GetBaseTheme() == BaseTheme.Dark;
        public bool IsDarkThemeOn
        {
            get { return _isDarkThemeOn; }
            set
            {
                _isDarkThemeOn = value;

                Properties.Settings.Default.IsDarkThemeOn = value;
                Properties.Settings.Default.Save();

                ThemeModifier.ModifyTheme(value);
            }
        }
    }
}
