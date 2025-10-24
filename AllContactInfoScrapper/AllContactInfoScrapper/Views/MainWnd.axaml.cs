using AllContactInfoScrapper.Domain;
using AllContactInfoScrapper.Infraestructure.ContactInfoEngine.Provider;
using AllContactInfoScrapper.Infraestructure.GoogleScrapper;
using AllContactInfoScrapper.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Material.Dialog;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static AllContactInfoScrapper.LocalizationResources.MainRes;

namespace AllContactInfoScrapper.Views
{
    public partial class MainWnd : Window
    {
        private CancellationTokenSource tokenSource;
        private MainVM model;
        public MainWnd()
        {
            InitializeComponent();
            model = new MainVM();

            FillContactProviders();
            this.DataContext = model;
        }
        
        

        
        private async void btExport_Click(object sender, RoutedEventArgs e)
        {
            if (model.ContactsInfo == null || !model.ContactsInfo.Any())
                return;
            var dlg = new SaveFileDialog();
            string FileExt;

            dlg.DefaultExtension = csvExt;
            dlg.Filters.Add(new FileDialogFilter() { Name = csvFileName, Extensions = { csvExt } });
            FileExt = "." + csvExt;

            dlg.InitialFileName = Infraestructure.Files.FilesUtils.MakeValidFileName("Contacts " + DateTime.Now.ToShortDateString() + DateTime.Now.ToShortDateString() + FileExt);
            var result = await dlg.ShowAsync(this);
            if (result != null)
            {
                var destinationPath = result;
                Infraestructure.Files.FileExporter FileExp = new Infraestructure.Files.FileExporter(destinationPath);

                FileExp.ExpExportFileCSV(model.ContactsInfo.ToList());
                var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                {
                    ContentHeader = exportTitle,
                    SupportingText = exportedOk,
                    StartupLocation = WindowStartupLocation.CenterOwner,
                    Borderless = true,
                    DialogHeaderIcon = Material.Dialog.Icons.DialogIconKind.Info,
                });
                _ = await dialog.ShowDialog(this);
            }
        }
        private async void btExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (model.ContactsInfo == null || !model.ContactsInfo.Any())
                return;
            var dlg = new SaveFileDialog();
            string FileExt;

            dlg.DefaultExtension = ExcelExt;
            dlg.Filters.Add(new FileDialogFilter() { Name = ExcelFileName, Extensions = { ExcelExt } });
            FileExt = "." + ExcelExt;

            dlg.InitialFileName = Infraestructure.Files.FilesUtils.MakeValidFileName("Contacts " + DateTime.Now.ToShortDateString() + DateTime.Now.ToShortDateString() + FileExt);
            var result = await dlg.ShowAsync(this);
            if (result != null)
            {
                var destinationPath = result;
                Infraestructure.Files.FileExporter FileExp = new Infraestructure.Files.FileExporter(destinationPath);
                List<string> Fields = new List<string>() { HeadTitle, HeadEmail, HeadPhone, HeadDesc, HeadUrl };
                FileExp.ExpExportFileExcel(model.ContactsInfo.ToList(), Fields);
                var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                {
                    ContentHeader = exportTitle,
                    SupportingText = exportedOk,
                    StartupLocation = WindowStartupLocation.CenterOwner,
                    Borderless = true,
                    DialogHeaderIcon = Material.Dialog.Icons.DialogIconKind.Info,
                });
                _ = await dialog.ShowDialog(this);
            }
        }
        private void mnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWnd sett = new SettingsWnd();
            sett.Show(this);
        }


       
        private void mnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWnd about = new AboutWnd();
            about.Show(this);

        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            if (model.ContactsInfo != null)
            {
                model.ContactsInfo.Clear();
                model.TotalContacts = 0;
                model.TotalPhones = 0;
                model.TotalEmails = 0;
            }
        }
        private void mnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private async void btLaunch_Click(object sender, RoutedEventArgs e)
        {
            
            await Launch();
        }
        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            model.LaunchEnabled = true;
            model.StopEnabled = false;
        }
        private void FillContactProviders()
        {
            var StreamProviders = AssetLoader.Open(new Uri("avares://AllContactInfoScrapper/Assets/ContactInfoProviders.csv"));
            ProviderReader reader = new ProviderReader(StreamProviders);
            model.ContactInfoProviders.AddRange(reader.GetProviders());
            model.SelectedConcatInfoProvider = model.ContactInfoProviders.First();
        }
        private async Task Launch(bool Captcha = false)
        {
            tokenSource = new CancellationTokenSource();
            model.StopEnabled = true;
            model.LaunchEnabled = false;
            bool failed = false;
            Random rnd = new Random();
            string DomainCountry = appsettings.Default.SearchEngineDefaultCountry;
            string Language = appsettings.Default.SearchEngineDefaultLanguage;

            GoogleScrapperCore scrapper = new GoogleScrapperCore(
                DomainCountry,
                Language,
                appsettings.Default.MinDelay,
                appsettings.Default.MaxDelay,
                appsettings.Default.ResultPerPage,
                appsettings.Default.MaxPages);
           
            SearchGenerator search = new SearchGenerator(model.Keywords, model.Emails, model.AnyWord, model.SelectedConcatInfoProvider);
            string SearchTermp = search.GetSearchTerm();
            var DownloadDlg = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
            {
                ContentHeader = completedTitle,
                SupportingText = DownloadingWarning,
                StartupLocation = WindowStartupLocation.CenterOwner,
                Borderless = true,
                DialogButtons = new DialogButton[]
                {

                },
                DialogHeaderIcon = Material.Dialog.Icons.DialogIconKind.Info,
            });

            var progress = new Progress<ContactInfo>(info =>
            {
                if (info.Title == "##DOWNLOADING##")
                    DownloadDlg.ShowDialog(this);
                else if (info.Title == "##DOWNLOADED##")
                    DownloadDlg.GetWindow().Close();
                else
                {
                    model.ContactsInfo.Add(info);
                    if (!string.IsNullOrEmpty(info.Phone))
                        model.TotalPhones++;
                    if (!string.IsNullOrEmpty(info.Email))
                        model.TotalEmails++;
                    model.TotalContacts++;
                }
            });
            try
            {
                var ContactInfos = await scrapper.GetContactInfosAsync(SearchTermp, progress, tokenSource.Token, Captcha);
                model.TotalContacts = model.ContactsInfo.Count;

            }
            catch (Exception ex)
            {

                if (ex.Message == "Captcha")
                {
                    var dCaptcha = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                    {
                        ContentHeader = CaptchaTitle,
                        SupportingText = CaptchaInfo,
                        StartupLocation = WindowStartupLocation.CenterOwner,
                        Borderless = true,
                        DialogHeaderIcon = Material.Dialog.Icons.DialogIconKind.Error,
                    });
                    var resDialogCaptcha = await dCaptcha.ShowDialog(this);
                    Captcha = true;
                    await Launch(true);
                }
                else
                {
                    Log.Error(ex.Message + ex.Source + ex.StackTrace);
                    var er = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                    {
                        ContentHeader = LocalizationResources.CommonRes.errorTitle,
                        SupportingText = ErrorInfo,
                        StartupLocation = WindowStartupLocation.CenterOwner,
                        Borderless = true,
                        DialogHeaderIcon = Material.Dialog.Icons.DialogIconKind.Error,
                    });
                    var resEr = await er.ShowDialog(this);
                    failed = true;
                }
            }

            model.LaunchEnabled = true;
            model.StopEnabled = false;
            if (!failed)
            {
                var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                {
                    ContentHeader = completedTitle,
                    SupportingText = completedMsg,
                    StartupLocation = WindowStartupLocation.CenterOwner,
                    Borderless = true,
                    DialogHeaderIcon = Material.Dialog.Icons.DialogIconKind.Info,
                });
                var result = await dialog.ShowDialog(this);
            }
            Captcha = false;
        }
    }
}
