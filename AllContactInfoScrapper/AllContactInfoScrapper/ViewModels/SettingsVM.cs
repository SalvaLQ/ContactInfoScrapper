using AllContactInfoScrapper.Domain.Google;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AllContactInfoScrapper.ViewModels
{
    public class SettingsVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public List<Language> Languages { get; set; }
        public List<Country> Countries { get; set; }

        private Country _SelectedCountry;
        public Country SelectedCountry
        {
            get => _SelectedCountry;
            set => SetField(ref _SelectedCountry, value);
        }
        private Language _SelectedLanguage;
        public Language SelectedLanguage
        {
            get => _SelectedLanguage;
            set => SetField(ref _SelectedLanguage, value);
        }
        private int _MaxPages;
        public int MaxPages
        {
            get => _MaxPages;
            set => SetField(ref _MaxPages, value);
        }
        private int _ResultPerPage;
        public int ResultPerPage
        {
            get => _ResultPerPage;
            set => SetField(ref _ResultPerPage, value);
        }
        private int _delayMin;
        public int delayMin
        {
            get => _delayMin;
            set => SetField(ref _delayMin, value);
        }
        private int _delayMax;
        public int delayMax
        {
            get => _delayMax;
            set => SetField(ref _delayMax, value);

        }       
        public SettingsVM()
        {
            Languages = new List<Language>();
            Countries = new List<Country>();
        }
    }
}
