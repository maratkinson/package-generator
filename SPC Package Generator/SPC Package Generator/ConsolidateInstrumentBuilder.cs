using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SPC_Package_Generator
{
    class ConsolidateInstrumentBuilder
    {

        public string ConstructInstrumentTask(DataTable columnsDT, string schema, string instrumentTable)
        {
             string finalTask = CreateInsertStatement(columnsDT, schema, instrumentTable);

            finalTask = finalTask + Environment.NewLine + Environment.NewLine + "<<NEW TASK>>";

            return finalTask;
            //Console.WriteLine(finalTask);

        }

        public string AddInstrumentTask(DataTable columnsDT, string schema, string instrumentTable)
        {
            string final = CreateInsertStatement(columnsDT, schema, instrumentTable);

            final = final + Environment.NewLine + Environment.NewLine + "<<NEW TASK>>";

            return final;
        }

        public string CreateTempInstTable(DataTable columnsDT)
        {

            SqlTableStrings sts = new SqlTableStrings();

            //Create temp table from columns in instrument file including datecreated field
            DataRow newRow = columnsDT.NewRow();
            newRow["Include"] = true;
            newRow["FileColumn"] = "DateCreated";
            newRow["ActualColumn"] = "DateCreated datetime default GetDate()";
            newRow["Nullable"] = null;
            newRow["TypeColumn"] = "";
            columnsDT.Rows.Add(newRow);

            string tempTable = sts.GenerateCreateTable("temp", "@newInstrument", columnsDT);

            string final = String.Concat(tempTable,
                                         Environment.NewLine);

            return final;
        }

        private string CreateInsertStatement(DataTable columnsDT ,string schema, string instrumentTable)
        {
            string insertBegin = @"INSERT INTO @newInstrument
(
";

            string columns = CreateColumnList(columnsDT);

            insertBegin += columns;

            string select = @")
SELECT
";

            string selectColumns = CreateSelectColumnList(columnsDT);

            string selectFROM = String.Format(@"
FROM {0}.{1} src
LEFT JOIN stage.Instrument i on i.Code = src.Code
LEFT JOIN newInstrument ni on ni.Code = src.Code
WHERE i.Code is null and ni.Code is null

print  'Found ' + convert(varchar, @@rowcount) + ' new Instrument records...'", schema, instrumentTable);

            insertBegin += select + selectColumns + selectFROM;

            Console.WriteLine(insertBegin);
            return insertBegin;
        }

        //loop through DataTable from SqlColumnMapping form and generate sql script columns.
        public string CreateSelectColumnList(DataTable dt)
        {
            string columns = "";

            foreach (DataRow dr in dt.Rows)
            {
                if (bool.Parse(dr["InstrumentImport"].ToString()))
                {
                    columns = String.Concat(
                                    columns,
                                    "   src.",
                                    dr["ActualColumn"], //add space in front for indentation formatting.
                                    " ",
                                    Environment.NewLine);
                }
            }
            return columns;
        }

         //loop through DataTable from SqlColumnMapping form and generate sql script columns.
        public string CreateColumnList(DataTable dt)
        {
            string columns = "";

            foreach (DataRow dr in dt.Rows)
            {
                if (bool.Parse(dr["InstrumentImport"].ToString()))
                {
                    columns = String.Concat(
                                    columns,
                                    "   " + dr["ActualColumn"], //add space in front for indentation formatting.
                                    ",",
                                    Environment.NewLine);
                }
            }
            return columns;
        }
    }
}
