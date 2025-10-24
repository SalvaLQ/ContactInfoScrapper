using AllContactInfoScrapper.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AllContactInfoScrapper.ViewModels
{
    public class MainVM : INotifyPropertyChanged
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


        private bool _mnActivatateVisible;
        public bool mnActivatateVisible
        {
            get => _mnActivatateVisible;
            set => SetField(ref _mnActivatateVisible, value);
        }
        private bool _mnGetAllEnabled;
        public bool mnGetAllEnabled
        {
            get => _mnGetAllEnabled;
            set => SetField(ref _mnGetAllEnabled, value);
        }
        private int _TotalContacts;
        public int TotalContacts
        {
            get => _TotalContacts;
            set => SetField(ref _TotalContacts, value);
        }
        private int _TotalPhones;
        public int TotalPhones
        {
            get => _TotalPhones;
            set => SetField(ref _TotalPhones, value);
        }
        private int _TotalEmails;
        public int TotalEmails
        {
            get => _TotalEmails;
            set => SetField(ref _TotalEmails, value);
        }

        private ContactInfoProvider _SelectedConcatInfoProvider;
        public ContactInfoProvider SelectedConcatInfoProvider
        {
            get => _SelectedConcatInfoProvider;
            set => SetField(ref _SelectedConcatInfoProvider, value);
        }
        public bool _LaunchEnabled;
        public bool LaunchEnabled
        {
            get => _LaunchEnabled;
            set => SetField(ref _LaunchEnabled, value);
        }

        public bool _StopEnabled;
        public bool StopEnabled
        {
            get => _StopEnabled;
            set => SetField(ref _StopEnabled, value);
        }

        public string _Keywords;
        public string Keywords
        {
            get => _Keywords;
            set => SetField(ref _Keywords, value);
        }

        public string _Emails;
        public string Emails
        {
            get => _Emails;
            set => SetField(ref _Emails, value);
        }

        public bool _AnyWord;
        public bool AnyWord
        {
            get => _AnyWord;
            set => SetField(ref _AnyWord, value);
        }
        public bool _AllWord;
        public bool AllWord
        {
            get => _AllWord;
            set => SetField(ref _AllWord, value);
        }
        

        public List<ContactInfoProvider> ContactInfoProviders { get; set; }
        
        public ObservableCollection<ContactInfo> ContactsInfo { get; set; }

        public MainVM()
        {
            mnGetAllEnabled = true;
            _mnActivatateVisible = true;
            
            ContactInfoProviders = new List<ContactInfoProvider>();
            ContactsInfo = new ObservableCollection<ContactInfo>();
            StopEnabled = false;
            LaunchEnabled = true;            
            AllWord = true;
            AnyWord = false;
        }
    }
}
