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

        public void ConstructInstrumentTask(DataTable columnsDT)
        {
            string finalTask = "";
        }

        private string CreateTempInstTable(DataTable columnsDT)
        {

            SqlTableStrings sts = new SqlTableStrings();
            string tempTable = sts.GenerateCreateTable("temp", "newInstrument", columnsDT);

            string insertStatement = CreateInsertStatement(columnsDT);

            string final = String.Concat(tempTable,
                                         Environment.NewLine,
                                         insertStatement

                                        );

            return final;
        }

        private string CreateInsertStatement(DataTable columnsDT)
        {

            return "";
        }
    }
}
