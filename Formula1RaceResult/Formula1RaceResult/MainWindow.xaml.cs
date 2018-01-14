using System.Windows;
using System.Data;

using Formula1RaceResult.Interfaces;
using Formula1RaceResult.HtmlParser;
using Formula1RaceResult.DbManipulalor;



namespace Formula1RaceResult
{



    public partial class MainWindow : IFormula1RaceResult
    {

        private readonly string _defaultWindowTitle = Properties.Resources.windowTitle;

        private readonly IFormula1RaceResultHtmlParser _htmlParser;

        private readonly IFormula1RaceResultDbManipulalor _dbManipulalor;

        public string CurrentRaceResultsArchiveId { get; set; }

        public DataTable GrandPrixResults { get; set; }

        public MainWindow()
        {

            InitializeComponent();

            Title = _defaultWindowTitle;

            GrandPrixResults = new DataTable();

            _htmlParser = new Formula1RaceResultHtmlParser(Properties.Settings.Default.raceResultHtmlTableClassName);
            _dbManipulalor = new Formula1RaceResultDbManipulalor();

            refreshData();

        }

        private void buttonRefresh_OnClick(object sender, RoutedEventArgs e)
        {

            refreshData();

        }

        private void buttonSave_OnClick(object sender, RoutedEventArgs e)
        {

            if (!checkDataCanBeSavedInDb())
                return;

            var errorMessageSavingDataToDb = _dbManipulalor.ErrorMessageSavingDataToDb
                (CurrentRaceResultsArchiveId, GrandPrixResults.TableName, GrandPrixResults);

            MessageBox.Show
                (string.IsNullOrEmpty
                    (errorMessageSavingDataToDb)
                    ? Properties.Resources.messageDataIsSuccessfullySavedInDb
                    : errorMessageSavingDataToDb);

        }

        private void refreshData()
        {

            if (string.IsNullOrEmpty(TextBoxUrl.Text))
                return;

            fillGrandPrixResultsFromUrl(TextBoxUrl.Text);
            visualizeData();

            Title = string.IsNullOrEmpty(GrandPrixResults.TableName) ? _defaultWindowTitle : GrandPrixResults.TableName;

        }

        private void fillGrandPrixResultsFromUrl(string url)
        {

            var parsedGrandPrixResults = GrandPrixResults;

            CurrentRaceResultsArchiveId = _htmlParser.ParseRaceResultDataTableFromUrl(url, parsedGrandPrixResults);

            GrandPrixResults = parsedGrandPrixResults;
        }

        private void visualizeData()
        {

            Title = GrandPrixResults.TableName;
            DataGrid.DataContext = this;

        }

        private bool checkDataCanBeSavedInDb()
        {

            if (string.IsNullOrEmpty(CurrentRaceResultsArchiveId) || GrandPrixResults.Rows.Count == 0)
                return false;

            if (!_dbManipulalor.IsRaceResultsExistsInDbByIdCheck(CurrentRaceResultsArchiveId))
                return true;

            MessageBox.Show(Properties.Resources.messageDataIsAlreadyExistsInDb);

            return false;

        }

    }



}
