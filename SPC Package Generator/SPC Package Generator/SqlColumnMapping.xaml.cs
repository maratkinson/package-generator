using System;
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
            //get column headers
            getColumnMappings(filePath);

            populate_misc_table();

            populateGridTable();
        }

        //Populates the SqlTable and filename_Label fields.
        //SqlTable fields derives what SqlTable should be called from Filename.
        private void populate_misc_table()
        {
            sqlTable_TextBox.Text = fileName.Replace("\"", "").Replace(" ", "").Replace(".csv", "").Replace("SPC", "");

            filename_Label.Content = fileName.Replace("\"", "");
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
                
                dt.Rows.Add(true, actualCol, suggestionCol, typeCol, nullable);
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
            string sqlTable = sqlTable_TextBox.Text;

            //using String interpolation tags in the 20's for XML task properties such as Sql Table and column mapping.
            string finalTaskString = String.Format(taskString, taskName, namePattern, sqlTable_TextBox.Text, generateListOfColumnMappings());

            string xmlTaskList = Environment.NewLine + finalTaskString;

            string final = System.IO.File.ReadAllText(@"C:\Test\load-data.dspkg");

            final = final.Replace("<<<TASK LIST>>>", finalTaskString);

            //Remove string interpolation from TaskList
            if (filePaths.Count == 1)
            {
                //Recursion is done
                final = final.Replace("<<<TASK LIST>>>", "");

                System.IO.File.WriteAllText(@"C:\Test\load-data.dspkg", final);

                MessageBox.Show("Completed!");

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
            sts.GenerateCreateTable(schema, sqlTable_TextBox.Text, dt);
        }

        private void Generate_Scripts_Click(object sender, RoutedEventArgs e)
        {
            addXMLTask();
            GenerateSqlScripts();
        }
    }
}
