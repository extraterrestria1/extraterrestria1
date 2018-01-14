using System;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Linq;

using Formula1RaceResult.Interfaces;



namespace Formula1RaceResult.DbManipulalor
{



    public class Formula1RaceResultDbManipulalor : IFormula1RaceResultDbManipulalor
    {
        public string ConnectionString { get; set; }

        public Formula1RaceResultDbManipulalor()
        {

            ConnectionString = Properties.Settings.Default.DbConnection;

        }

        public bool IsRaceResultsExistsInDbByIdCheck(string raceResultsArchiveId)
        {

            using (var context = new GrandPrixResultsContext(ConnectionString))
                return context.GrandPrix.Any(grandPrix => grandPrix.GrandPrixId == raceResultsArchiveId);

         }

        public string ErrorMessageSavingDataToDb(string raceResultsArchiveId, string raceResultsArchiveTitle, DataTable grandPrixResults)
        {

            var errorMessage = string.Empty;

            try
            {
                using (var context = new GrandPrixResultsContext(ConnectionString))
                {
                    context.GrandPrix.Add
                        (new GrandPrix
                        {
                            GrandPrixId = raceResultsArchiveId,
                            GrandPrixResultsTitle = raceResultsArchiveTitle
                        });

                    foreach (DataRow dataRow in grandPrixResults.Rows)
                        context.GrandPrixResults.Add(new GrandPrixResults(raceResultsArchiveId, dataRow));

                    context.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }

            return errorMessage;

        }

    }



}
