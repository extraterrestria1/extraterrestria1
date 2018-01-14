using System.Data;



namespace Formula1RaceResult.Interfaces
{



    public interface IFormula1RaceResult
    {

        string CurrentRaceResultsArchiveId { get; set; }

        DataTable GrandPrixResults { get; set; }

    }



    public interface IFormula1RaceResultHtmlParser
    {

        string ParseRaceResultDataTableFromUrl(string url, DataTable dataTable);

    }



    public interface IFormula1RaceResultDbManipulalor
    {

        string ConnectionString { get; set; }

        bool IsRaceResultsExistsInDbByIdCheck(string raceResultsArchiveId);

        string ErrorMessageSavingDataToDb(string raceResultsArchiveId, string raceResultsArchiveTitle, DataTable grandPrixResults);

    }



}
