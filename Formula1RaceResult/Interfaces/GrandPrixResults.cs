using System.ComponentModel.DataAnnotations.Schema;
using System.Data;



namespace Formula1RaceResult.Interfaces
{



    public class GrandPrixResults
    {

        public string GrandPrixResultsId { get; set; }

        public string Pos { get; set; }

        public string No { get; set; }

        public string Driver { get; set; }

        public string Car { get; set; }

        public string Laps { get; set; }

        [Column("Time & Retired")]
        public string TimeRetired { get; set; }

        public string Pts { get; set; }

        public GrandPrixResults(string grandPrixResultsId, DataRow dataRow)
        {

            Pos = (string)dataRow["Pos"];
            No = (string)dataRow["No"];
            Driver = (string)dataRow["Driver"];
            Car = (string)dataRow["Car"];
            Laps = (string)dataRow["Laps"];
            TimeRetired = (string)dataRow["Time & Retired"];
            Pts = (string)dataRow["Pts"];

            GrandPrixResultsId = $"{grandPrixResultsId}\\{Driver}";
        }

    }



    public class GrandPrix
    {

        public string GrandPrixId { get; set; }

        public string GrandPrixResultsTitle { get; set; }

    }



}
