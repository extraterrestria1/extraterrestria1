using System;
using System.Data;
using System.Linq;
using System.Windows;

using CsQuery;
using CsQuery.ExtensionMethods.Internal;

using Formula1RaceResult.Interfaces;



namespace Formula1RaceResult.HtmlParser
{



    public class Formula1RaceResultHtmlParser : IFormula1RaceResultHtmlParser
    {

        private readonly string _raceResultHtmlTableClassName;

        public Formula1RaceResultHtmlParser(string raceResultHtmlTableClassName)
        {

            _raceResultHtmlTableClassName = raceResultHtmlTableClassName;

        }

        public string ParseRaceResultDataTableFromUrl(string url, DataTable dataTable)
        {

            dataTable.Columns.Clear();
            dataTable.Clear();

            if (url.IsNullOrEmpty())
                return string.Empty;

            CQ dom = null;

            try
            {
                dom = CQ.CreateFromUrl(url);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            if (dom == null)
                return string.Empty;

            var resultsArchiveDateNode = dom[".date"];

            if (resultsArchiveDateNode.Length == 0)
            {
                MessageBox.Show(Properties.Resources.messageInvalidPageResult);
                return string.Empty;
            }

            var htmlTableColumns = dom.Find($".{_raceResultHtmlTableClassName}").Find("thead").Find("tr").Find("th");

            if (!htmlTableColumns.Any())
            {
                MessageBox.Show(Properties.Resources.messagePageNotSupported);
                return string.Empty;
            }

            var resultsArchiveId = string.Empty;

            foreach (var domObject in resultsArchiveDateNode.Get(0).ChildElements)
            {
                resultsArchiveId += resultsArchiveId.IsNullOrEmpty() ? domObject.InnerText : $"_{domObject.InnerText}";
            }

            dataTable.TableName = dom[".ResultsArchiveTitle"].
                Get(0).
                FirstChild.NodeValue.Replace("/n", "").
                Trim().
                Replace("- RACE RESULT", "").
                Trim();

            createDataColumns(htmlTableColumns, dataTable);

            fillDataRows(dom, dataTable);

            //erase limiter columns

            for (var columnIterator = 0; columnIterator < dataTable.Columns.Count; columnIterator++)
            {
                var dataColumn = dataTable.Columns[columnIterator];

                if (dataColumn.ColumnName.StartsWith("limiter"))
                    dataTable.Columns.Remove(dataColumn);

                if (dataColumn.ColumnName.Contains("/"))
                    dataColumn.ColumnName = dataColumn.ColumnName.Replace("/", " & ");
            }

            return resultsArchiveId;

        }

        private static void createDataColumns(CQ htmlTableColumns, DataTable dataTable)
        {

            for (var columnIterator = 0; columnIterator < htmlTableColumns.Count(); columnIterator++)
            {
                var domObject = htmlTableColumns.Get(columnIterator);
                string columnName;
                string caption;

                if (domObject.InnerText.IsNullOrEmpty())
                {
                    columnName = domObject.HasChildren
                        ? domObject.ChildNodes.Item(0).InnerText
                        : $"limiter{columnIterator}";

                    if (domObject.HasChildren && domObject.ChildNodes.Item(0).NodeName == "ABBR")
                    {
                        caption = domObject.ChildNodes.Item(0).OuterHTML;
                        caption = caption.Replace("<abbr title=", "").Replace($">{columnName}</abbr>", "");
                        caption = caption.Substring(1, caption.Length - 2);
                    }
                    else
                    {
                        caption = columnName;
                    }
                }
                else
                {
                    columnName = domObject.InnerText;
                    caption = columnName;
                }

                dataTable.Columns.Add(new DataColumn { ColumnName = columnName, Caption = caption });
            }

        }

        private void fillDataRows(CQ dom, DataTable dataTable)
        {

            foreach (var domObject in dom.Find($".{_raceResultHtmlTableClassName}").Find("tbody").Find("tr"))
            {
                var dataRow = dataTable.NewRow();
                var columnIterator = 0;

                foreach (var childDomObject in domObject.ChildElements)
                {
                    var dataColumn = dataTable.Columns[columnIterator];
                    columnIterator++;

                    if (dataColumn.ColumnName.StartsWith("limiter"))
                        continue;

                    var value = string.Empty;

                    foreach (var childNode in childDomObject.ChildNodes)
                    {
                        if (!string.IsNullOrWhiteSpace(childNode.NodeValue))
                            value += childNode.NodeValue;
                        else if (childNode.HasChildren && !string.IsNullOrWhiteSpace(childNode.LastChild.NodeValue)
                            && !childNode.OuterHTML.Contains("hide-for-desktop"))
                            value += value.IsNullOrEmpty()
                                ? childNode.LastChild.NodeValue
                                : $" {childNode.LastChild.NodeValue}";
                    }

                    dataRow[dataColumn] = value;
                }

                dataTable.Rows.Add(dataRow);

            }
        }

    }



}
