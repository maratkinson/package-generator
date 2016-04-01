using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPC_Package_Generator
{
    class LoadDataStrings
    {
        public static string loadDataInitial_1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
 <!--
/**********************************************************************/
*                                                                    *
*               Symmetrix DataStage Package File                     *
*                 Copyright (c) StatPro                              *
*                                                                    *
* Change Control:                                                    *
* Date (yyyy-MM-dd) Who (Init) Comment                               *
*====================================================================*
*                                                                    *
/**********************************************************************/
-->
<?xml-stylesheet type=""text/xsl"" href=""packages/report.xslt""?>
   <configuration>

        <Package version=""1.0""
                 name=""LoadData""
                 description=""""
                 log-level=""All""> " + Environment.NewLine
                                       + Environment.NewLine;

        public static string variablesNodeBegin_2 = @"        <Variables>" 
                                                    + Environment.NewLine 
                                                    + Environment.NewLine;

        public static string variablesNodeEnd_4 = @"        </Variables>
        <Loggers>
        <!-- add loggers here -->
        </Loggers>
        <Connections>
        <!-- add connections here -->
        </Connections>" + Environment.NewLine 
                        + Environment.NewLine;

        public static string taskNodeBegin_5 = @"<Tasks>

       <!-- add tasks here -->
      
	        <Task name=""LoadSourceFiles"" 
                  description=""Load Infostore files."" 
                  type=""Symmetrix.DataStage.Tasks.CSharpScriptTask, Symmetrix.DataStage"" 
                  depends="""" 
                  enabled=""True"">
			<Property name=""Debug"" value=""True""/>
			<Property name=""SourceCode"">
            <![CDATA[

            using System;
            using System.Collections;
            using System.IO;
            using System.Data;

            using Symmetrix.DataStage;
            using Symmetrix.DataStage.Tasks;

            using Zeno.Data;


            namespace Symmetrix.DataStage
            {

            public class ETLDataImport: IScriptTask
            {

                public ETLDataImport()
                {
                }
    
                public TaskResult Execute(Package package)
                { "
                        + Environment.NewLine 
                        + Environment.NewLine;

        public static string taskNodeEnd_7 = @"
        return result;
    }
    
    public TaskResult runTask(Package package, string taskName, string searchPattern, string fileDescription)
    {
        string searchPath = package.Variables[""InfostoreDataPath""].Value;
      
        DirectoryInfo dir = new DirectoryInfo(searchPath);
      
        FileInfo[] files = dir.GetFiles(searchPattern);
      
        DelimitedFileImportTask2 task = (DelimitedFileImportTask2)package.Tasks[taskName];
      
        if(files.Length==1)
        {        
            task.FileName = files[0].FullName;
            task.DateTimeFormat = ""yyyy-MM-dd"";
        
            package.Logger.Info(""Found {0} - Processing..."", files[0].Name);
        
            return package.ExecuteTask(task);
        }
        else
        {
            throw new FileNotFoundException(String.Format(""Unable to process {0} - File not found or multiple files exist"", fileDescription));
        }    
            return TaskResult.Success;
        }
    }
}

       ]]></Property>
    </Task>	  

";


        public static string end_8 = @"         </Tasks>

     </Package>

 </configuration>";

      
    }
}
