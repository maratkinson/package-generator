using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace SPC_Package_Generator
{
    class LoadDataBuilder
    {
        //Strings for package
        string variableXMLList;
        string cSharpTaskList;
        string schema;

        List<string> variablesList_3 = new List<string>();
        List<string> namePatterns = new List<string>();
        List<string> filepaths = new List<string>();

        //##### BUILD PACKAGE ####################
        public void Build_Package(List<string> filepaths, List<string> filenames, string _schema)
        {
            //set schema
            schema = _schema;

            int count = 0;

            foreach (string filename in filenames)
            {
                string filepath = filepaths[count];

                File.filename = filename;

                string namePattern = filename.Substring(0, (filename.Length - 4));
                namePattern = namePattern.Replace(" ", "");

                //Add to list for later processing, passing on to SqlColumnMapping Form.
                namePatterns.Add(namePattern);
                
                createXMLVars(namePattern, filename);
                addCSharpTask(namePattern);
                
                count += 1;

            }

            string final = LoadDataStrings.loadDataInitial_1 +
                           LoadDataStrings.variablesNodeBegin_2 +
                           variableXMLList + Environment.NewLine + Environment.NewLine +
                           LoadDataStrings.variablesNodeEnd_4 +
                           LoadDataStrings.taskNodeBegin_5 +
                           cSharpTaskList + Environment.NewLine + Environment.NewLine +
                           LoadDataStrings.taskNodeEnd_7 +
                           "<<<TASK LIST>>>" + Environment.NewLine + Environment.NewLine + //PLACEHOLDER FOR TASKLIST, Only populated after the SQLColumnMapping forms.
                           LoadDataStrings.end_8;

            System.IO.File.WriteAllText(@"C:\Test\load-data.dspkg", final);

            CreateConsolInstPackage();

            //start wizard
            SqlColumnMapping sqlCol = new SqlColumnMapping(schema, filepaths, namePatterns, filenames);
            sqlCol.Show();

        }

        //##### Create Variable XML Lists ####################
        public void createXMLVars(string namePattern, String filename)
        {
            string beginVar = @"        <Variable name=""";
            string midVar = @""" value=""";
            string endVar = @""" />";

            string var = String.Format("{0}{1}{2}{3}{4}", beginVar, namePattern, midVar, filename, endVar);

            variableXMLList = variableXMLList + Environment.NewLine + var;
            
        }

        //##### Create C# Task XML Lists ####################
        private void addCSharpTask(string namePattern)
        {
            string taskString = @"
            TaskResult result = TaskResult.Success;" + Environment.NewLine +
            @"            result = runTask(package, ""{0}"", package.Variables[""{1}""].Value, ""{0}"");
            if (result != TaskResult.Success)
            return result;";

            string taskName = "Load" + namePattern.Replace("Pattern", "");

            string finalTaskString = String.Format(taskString, taskName, namePattern);

            cSharpTaskList = cSharpTaskList + finalTaskString + "\n";
        }

        //##### Create Consolidate Instruments Package #######
        private void CreateConsolInstPackage()
        {
            string consolInstpackage = ConsolidateInstrumentStrings.consolInsPackageStart
                        + ConsolidateInstrumentStrings.consolInsPackageInstrumentTaskStart + @"<<TEMP TABLE>>

    <<NEW TASK>>

  /**************************************************************************
  *                                                                         *
  *                         Refresh Instrument Fields                       *
  *                                                                         *
  ***************************************************************************/

    <<NEW REFRESH>>

    <<END>>";


            System.IO.File.WriteAllText(@"C:\Test\consolidate-instruments.dspkg", consolInstpackage);
        }
    }
}
