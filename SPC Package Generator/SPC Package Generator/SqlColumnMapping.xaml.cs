﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace SPC_Package_Generator
{
    /// <summary>
    /// Interaction logic for SqlColumnMapping.xaml
    /// </summary>
    public partial class SqlColumnMapping : Window
    {
        string suggestionCol;
        string actualCol;
        string typeCol;
        DataTable dt = new DataTable();
        
        string columnsList;
        string namePattern, fileName, schema;

        string taskString;

        List<string> filePaths = new List<string>();
        List<string> namePatterns = new List<string>();
        List<string> fileNames = new List<string>();

        List<string> columns = new List<string>();

        public SqlColumnMapping(string _schema, List<string> _filePaths, List<string> _namePatterns, List<string> _fileNames)
        {
            filePaths = _filePaths;
            namePatterns = _namePatterns;
            fileNames = _fileNames;

            namePattern = _namePatterns[0];
            fileName = _fileNames[0];
            string filePath = _filePaths[0];
            schema = _schema;

            InitializeComponent();

            dt.Columns.Add("Include");
            dt.Columns.Add("FileColumn");
            dt.Columns.Add("ActualColumn");
            dt.Columns.Add("TypeColumn");
            dt.Columns.Add("Nullable");
            dt.Columns.Add("InstrumentImport");
            dt.Columns.Add("InstrumentRefresh");
            dt.Columns.Add("PortfolioImport");
            dt.Columns.Add("PortfolioRefresh");
            dt.Columns.Add("IssuerImport");
            dt.Columns.Add("IssuerRefresh");
            dt.Columns.Add("Benchmark_ConstituentImport");
            dt.Columns.Add("Benchmark_ConstituentRefresh");
            dt.Columns.Add("CurrencyImport");
            dt.Columns.Add("CurrencyRefresh");
            dt.Columns.Add("CountryImport");
            dt.Columns.Add("CountryRefresh");
            dt.Columns.Add("ExchangeImport");
            dt.Columns.Add("ExchangeRefresh");
            dt.Columns.Add("Portfolio_HoldingImport");
            dt.Columns.Add("Portfolio_HoldingRefresh");
            dt.Columns.Add("BenchmarkImport");
            dt.Columns.Add("BenchmarkRefresh");

            //get column headers
            getColumnMappings(filePath);

            populate_misc_table();

            populateGridTable();

            PopulateEntityComboBox();

        }

        //Populates the SqlTable and filename_Label fields.
        //SqlTable fields derives what SqlTable should be called from Filename.
        private void populate_misc_table()
        {
            sqlTable_TextBox.Text = fileName.Replace("\"", "").Replace(" ", "").Replace(".csv", "").Replace("SPC", "");

            filename_Label.Text = fileName.Replace("\"", "");
        }

        //Fills datatable in form and derives sql type from column name values.
        private void populateGridTable() 
        {
            dt.Clear();

            List<string> type_list = new List<string>();
            type_list.Add("datetime");
            type_list.Add("money");
            type_list.Add("bit");
            type_list.Add("varchar(50)");
            type_list.Add("varchar(250)");

            foreach (string column in columns)
            {
                suggestionCol = column.Replace("\"", "").Replace(" ", "");
                actualCol = column.Replace("\"", "").Replace(" ", "");
                
                String type;

                if (column.ToLower().Contains("Date"))
                {
                    type = "datetime";
                }
                else if (column.ToLower().Contains("marketcap"))
                {
                    type = "money";
                }
                else if (column.ToLower().Contains("traded") || column.ToLower().Contains("confirmed"))
                {
                    type = "money";
                }
                else if (column.ToLower().Contains("settled") || column.ToLower().Contains("ordered"))
                {
                    type = "money";
                }
                else if (column.ToLower().Contains("indicator"))
                {
                    type = "bit";
                }
                else if (column.ToLower().Contains("code"))
                {
                    type = "varchar(50)";
                }
                else {
                    type = "varchar(250)";
                }

                typeCol = type;
                
                //Check if column is code/description to determine nullable/not-nullable
                bool nullable = true;
                if(column.ToLower().Contains("code") || column.ToLower().Contains("description"))
                {
                    nullable = false;
                }

                Console.WriteLine("NAME PATTERN : " + namePattern);
                
                //Insert rows with correct allocation of import boolean values
                if (namePattern.ToLower().Contains("instrument"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, true, true, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false);
                }
                else if (namePattern.ToLower().Contains("portfolioholding"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, false, false, true, true, false, false, false, false, false, false, false, false, false, false, true, true, false, false);
                }
                else if (namePattern.ToLower().Contains("portfolio") && !namePattern.ToLower().Contains("holding"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, false, false, true, true, false, false, false, false, false, false, false, false, false, false, true, true, false, false);
                }
                else if (namePattern.ToLower().Contains("issuer"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, false, false, false, false, true, true, false, false, false, false, false, false, false, false, true, true, false, false);
                }
                else if (namePattern.ToLower().Contains("benchmark"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, false, false, false, false, false, false, true, true, false, false, false, false, false, false, true, true, false, false);
                }
                else if (namePattern.ToLower().Contains("benchmarkconstituent"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, false, false, false, false, false, false, true, true, false, false, false, false, false, false, true, true, false, false);
                }
                else if (namePattern.ToLower().Contains("country"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, false, false, false, false, false, false, false, false, false, false, true, true, false, false, true, true, false, false);
                }
                else if (namePattern.ToLower().Contains("currency"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, false, false, false, false, false, false, false, false, true, true, false, false, false, false, true, true, false, false);
                }
                else if (namePattern.ToLower().Contains("exchange"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, false, false);
                }
                else if (namePattern.ToLower().Contains("benchmark") && !namePattern.ToLower().Contains("constituent"))
                {
                    dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true);
                }
            }

            mainGridView.ItemsSource = dt.AsDataView(); 
            TypeColumnComboBox.ItemsSource = type_list;  
        }

        //##### Create XML Tasks Lists with column mappings ####################
        private void addXMLTask()
        {
            taskString = @"
                <Task name=""{0}""
                       description=""""
                       type=""Symmetrix.DataStage.Tasks.DelimitedFileImportTask2, Symmetrix.DataStage""
                       depends=""""
                       hidden=""True""
                       enabled=""False"">

                       <Property name=""FileName"" value=""{1}"" />
                       <Property name=""FirstRowHeaders"" value=""True"" />
                       <Property name=""TargetConnectionName"" value=""Stage"" />
                       <Property name=""TargetTableName"" value=""{2}"" />
                       <Property name=""TruncateTable"" value=""True"" />
                       <Property name=""DecimalSymbol"" value=""."" />  
                       <Property name=""WhereClause"" value="""" />
                       <Property name=""IgnoreDataErrors"" value=""False"" />
                       <ColumnMappings>
                              {3}
                       </ColumnMappings>
                </Task>" + Environment.NewLine
                         + Environment.NewLine +
                        "<<<TASK LIST>>>";

            string taskName = "Load" + namePattern.Replace("Pattern", "");
            string sqlTable = schema + "." + sqlTable_TextBox.Text;
            
            string finalTaskString = String.Format(taskString, taskName, namePattern, sqlTable, generateListOfColumnMappings());

            string xmlTaskList = Environment.NewLine + finalTaskString;

            string final = System.IO.File.ReadAllText(@"C:\Test\load-data.dspkg");

            final = final.Replace("<<<TASK LIST>>>", finalTaskString);

            //Remove string interpolation from TaskList
            if (filePaths.Count == 1)
            {
                //Recursion is done
                final = final.Replace("<<<TASK LIST>>>", "");

                System.IO.File.WriteAllText(@"C:\Test\load-data.dspkg", final);

                this.Close();
            }
            else
            {
                //Continue to recurse through list.
                System.IO.File.WriteAllText(@"C:\Test\load-data.dspkg", final);

                foreach (string x in namePatterns)
                {
                    Console.WriteLine(x);
                }

                //Remove processed item and move on to next item.
                filePaths.RemoveAt(0);
                namePatterns.RemoveAt(0);
                fileNames.RemoveAt(0);

                SqlColumnMapping sql = new SqlColumnMapping(schema, filePaths, namePatterns, fileNames);
                sql.Show();

                this.Close();
            }
        }

        //Generates list of XML column mappings.
        private string generateListOfColumnMappings()
        {
            string columnCode = @"                          <ColumnMapping source=""{0}"" target=""{1}""/>";
            columnsList = "";

            foreach (DataRow dr in dt.Rows)
            {
                if(bool.Parse(dr["Include"].ToString()))
                {
                    string final = String.Format(columnCode, dr["FileColumn"], dr["ActualColumn"]);
                    columnsList = columnsList + Environment.NewLine + final ;
                }
            }

            return columnsList;
        }

        //Retrieves column headers from the file.
        private void getColumnMappings(string filePath)
        {
            columns.Clear();
            StreamReader sr = new StreamReader(filePath);

            string[] headers = sr.ReadLine().Split(',');

            foreach (string header in headers)
            {
                string head = header.Replace("\"", "");
                columns.Add(header); // I've added the column headers here.
            }
        }

        //Create SQL Scripts for creating tables.
        private void GenerateSqlScripts()
        {
            SqlTableStrings sts = new SqlTableStrings();

            string sql = sts.GenerateCreateTable(schema, sqlTable_TextBox.Text, dt);

            System.IO.File.WriteAllText(String.Format(@"C:\Test\{0}.sql", sqlTable_TextBox.Text), sql);
            System.IO.File.WriteAllText(String.Format(@"C:\Test\SCEMA.sql"), "CREATE SCHEMA " + schema);
        }

        private void GenerateConsolidateInstrumentScript()
        {
            string package = System.IO.File.ReadAllText(@"C:\Test\consolidate-instruments.dspkg");

            ConsolidateInstrumentBuilder cib = new ConsolidateInstrumentBuilder();

            //if it is main instrument file create temp tables
            if (entity_ComboBox.SelectedIndex == 0)
            {
                package = package.Replace("<<TEMP TABLE>>", cib.CreateTempInstTable(dt));
            }

            //Add new SQL import portion in instrument task
            string newImport = cib.AddInstrumentTask(dt, schema, sqlTable_TextBox.Text);
            package = package.Replace("<<NEW TASK>>", newImport);

            //Add new SQL refresh portion in instrument task
            string newRefresh = cib.CreateRefreshStatement(dt, sqlTable_TextBox.Text, schema);
            package = package.Replace("<<NEW REFRESH>>", newRefresh);

            if (filePaths.Count == 1)
            {
                package = package.Replace("<<END>>", ConsolidateInstrumentStrings.consolInsPackageInstrumentTaskEnd);
                package = package.Replace("<<NEW REFRESH>>", ""); //REMOVE REFRESH TAG
                package = package.Replace("<<NEW TASK>>", "");    //REMOVE TASK TAG
            }

            System.IO.File.WriteAllText(@"C:\Test\consolidate-instruments.dspkg", package);
        }

        private void GenerateConsolidateHoldingScript()
        {
            string packageHol = System.IO.File.ReadAllText(@"C:\Test\consolidate-holdings.dspkg");
            Console.WriteLine(packageHol);

            ConsolidateHoldingsBuilder chb = new ConsolidateHoldingsBuilder();
            string insert = chb.CreateHoldingInsertStatement(dt, schema, sqlTable_TextBox.Text);
            string fixHoldings = chb.CreateFixHoldingStatement();

            Console.WriteLine("INSERT HOLDINGS : " + insert);
            Console.WriteLine("FIX FIX HOLDINGS : " + fixHoldings);

            string task = String.Concat(ConsolidateHoldingsStrings.consolHoldingsTaskStart
                                        ,Environment.NewLine
                                        ,Environment.NewLine
                                        ,insert
                                        ,Environment.NewLine);

            packageHol = packageHol.Replace("<<NEW IMPORT>>", task);
            packageHol = packageHol.Replace("<<FIX HOLDINGS>>", fixHoldings);
            packageHol = packageHol.Replace("<<TASK END>>", ConsolidateHoldingsStrings.consolHoldingsTaskEnd);

            System.IO.File.WriteAllText(@"C:\Test\consolidate-holdings.dspkg", packageHol);
            Console.WriteLine("FINISHED PACKAGE : ");
            Console.WriteLine(packageHol);
        }

        private void GenerateConsolidateStaticScripts()
        {
            string entity = entity_ComboBox.SelectedValue.ToString();
            if (entity_ComboBox.SelectedIndex != 0 || entity_ComboBox.SelectedIndex != 1)
            {
                string packageStatic = System.IO.File.ReadAllText(@"C:\Test\consolidate-static.dspkg");
                string staticTask = ConsolidateStaticStrings.consolStaticTask;

                ConsolidateStaticBuilder csb = new ConsolidateStaticBuilder();


                string insert = csb.AddEntityImport(RemoveFalseRows(dt, entity + "Import"), entity, schema);
                string update = csb.AddEntityRefresh(RemoveFalseRows(dt, entity + "Refresh"), entity, schema);

                staticTask = staticTask.Replace("<<ENTITY>>", entity);
                staticTask = staticTask.Replace("<<NEW IMPORT>>", insert);
                staticTask = staticTask.Replace("<<NEW REFRESH>>", update);

                //add new task to package
                packageStatic = packageStatic.Replace("<<NEW TASK>>", staticTask);

                System.IO.File.WriteAllText(@"C:\Test\consolidate-static.dspkg", packageStatic);
            }
        }

        private void Generate_Scripts_Click(object sender, RoutedEventArgs e)
        {
            //check if its the holding file and offer to create the holding package.
            if (fileName.ToLower().Contains("holding"))
            {
                var result = MessageBox.Show("Do you want to create the Consolidate-Holdings package from this file?",
                                              "Consolidate Holdings",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    GenerateConsolidateHoldingScript();
                } 
            }

            //Create scripts
            
            GenerateConsolidateStaticScripts();

            GenerateConsolidateInstrumentScript();
            GenerateSqlScripts();
            addXMLTask();
        }

        private void PopulateEntityComboBox()
        {
            entity_ComboBox.Items.Add("Instrument");
            entity_ComboBox.Items.Add("Portfolio_Holding");
            entity_ComboBox.Items.Add("Portfolio");
            entity_ComboBox.Items.Add("Issuer");
            entity_ComboBox.Items.Add("Benchmark");
            entity_ComboBox.Items.Add("Benchmark_Constituent");
            entity_ComboBox.Items.Add("Country");
            entity_ComboBox.Items.Add("Currency");
            entity_ComboBox.Items.Add("Exchange");

            if (namePattern.ToLower().Contains("instrument"))
            {
                entity_ComboBox.SelectedIndex = 0;
            }
            else if (namePattern.ToLower().Contains("portfolio_holding"))
            {
                entity_ComboBox.SelectedIndex = 1;
            }
            else if (namePattern.ToLower().Contains("portfolio") && !namePattern.ToLower().Contains("holding"))
            {
                entity_ComboBox.SelectedIndex = 2;
            }
            else if (namePattern.ToLower().Contains("issuer"))
            {
                entity_ComboBox.SelectedIndex = 3;
            }
            else if (namePattern.ToLower().Contains("benchmark"))
            {
                entity_ComboBox.SelectedIndex = 4;
            }
            else if (namePattern.ToLower().Contains("benchmark_constituent"))
            {
                entity_ComboBox.SelectedIndex = 5;
            }
            else if (namePattern.ToLower().Contains("country"))
            {
                entity_ComboBox.SelectedIndex = 6;
            }
            else if (namePattern.ToLower().Contains("currency"))
            {
                entity_ComboBox.SelectedIndex = 7;
            }
            else if (namePattern.ToLower().Contains("exchange"))
            {
                entity_ComboBox.SelectedIndex = 8;
            }
        }

        private DataTable RemoveFalseRows(DataTable dt, string check)
        {
            DataTable newDT = dt;
            for(int i = newDT.Rows.Count-1; i >= 0; i--)
            {
                DataRow dr = newDT.Rows[i];
                if (!bool.Parse(dr[check].ToString())) //check if row has not been selected for import/refresh
                {
                    dr.Delete();
                }
            }

            return newDT;
        }
    }
}
