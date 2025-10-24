using AllContactInfoScrapper.Infraestructure.GoogleScrapper.Globalization;
using AllContactInfoScrapper.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Material.Dialog;
using System;
using System.Linq;

namespace AllContactInfoScrapper.Views
{
    public partial class SettingsWnd : Window
    {
        private SettingsVM model ;
        public SettingsWnd()
        {
            InitializeComponent();
            model = new SettingsVM();
            LoadSettings();
            FillSearchEngineLanguages();
            FillSearchEngineCountries();
            DataContext = model;
           
        }
      
        private void LoadSettings()
        {
            model.delayMin = appsettings.Default.MinDelay;
            model.delayMax = appsettings.Default.MaxDelay;
            model.MaxPages = appsettings.Default.MaxPages;
            model.ResultPerPage = appsettings.Default.ResultPerPage;
        }
        private void FillSearchEngineLanguages()
        {
            var StreamProviders = AssetLoader.Open(new Uri("avares://AllContactInfoScrapper/Assets/GoogleLanguages.csv"));
            LanguageReader reader = new LanguageReader(StreamProviders);
            model.Languages.AddRange(reader.GetSupportedLanguages());
            if (!string.IsNullOrEmpty(appsettings.Default.SearchEngineDefaultLanguage))
            {
                var SelLang = model.Languages.Where(d => d.Code == appsettings.Default.SearchEngineDefaultLanguage).FirstOrDefault();
                if (SelLang != null)
                {
                    model.SelectedLanguage = SelLang;
                }
            }
        }
        private void FillSearchEngineCountries()
        {
            var StreamProviders = AssetLoader.Open(new Uri("avares://AllContactInfoScrapper/Assets/GoogleCountries.csv"));
            CountryReader reader = new CountryReader(StreamProviders);
            model.Countries.AddRange(reader.GetSupportedCountries());
            if (!string.IsNullOrEmpty(appsettings.Default.SearchEngineDefaultCountry))
            {
                var SelCountry = model.Countries.Where(d => d.Domain == appsettings.Default.SearchEngineDefaultCountry).FirstOrDefault();
                if (SelCountry != null)
                {
                    model.SelectedCountry = SelCountry;
                }
            }

        }
        private async void btAcept_Click(object sender, RoutedEventArgs e)
        {
            var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
            {
                ContentHeader = LocalizationResources.SettingsRes.saveTitle,
                SupportingText = LocalizationResources.SettingsRes.saveChanges,
                StartupLocation = WindowStartupLocation.CenterOwner,
                Borderless = true,
                DialogHeaderIcon = Material.Dialog.Icons.DialogIconKind.Warning,
                NegativeResult = new DialogResult("no"),
                DialogButtons = new DialogButton[]
                {
                    new DialogButton
                    {
                        Content =LocalizationResources.SettingsRes.Yes,
                        Result = "yes"
                    },
                    new DialogButton
                    {
                        Content = LocalizationResources.SettingsRes.No,
                        Result = "no"
                    },
                },
            });
            var msg = await dialog.ShowDialog(this);
            if (msg.GetResult == "yes")
            {
                appsettings.Default.MinDelay = model.delayMin;
                appsettings.Default.MaxDelay = model.delayMax;
                appsettings.Default.MaxPages = model.MaxPages;
                appsettings.Default.ResultPerPage = model.ResultPerPage;
                appsettings.Default.SearchEngineDefaultCountry = model.SelectedCountry.Domain;
                appsettings.Default.SearchEngineDefaultLanguage = model.SelectedLanguage.Code;
                appsettings.Default.Save();
            }
            Close();
        }
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }
    }
}
