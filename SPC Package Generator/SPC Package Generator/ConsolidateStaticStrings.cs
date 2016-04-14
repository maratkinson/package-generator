using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPC_Package_Generator
{
    class ConsolidateStaticStrings
    {
        public static string consolStaticStart = @"<?xml version=""1.0"" encoding=""utf-8""?>
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
           name=""ConsolidateStatic""
           description=""""
           log-level=""All"">

    <Variables />

    <Loggers />

    <Connections />

    <Tasks>

      <!-- add tasks here -->



    <<NEW TASK>>


  </Tasks>

 </Package>

 </configuration>

" + Environment.NewLine
               + Environment.NewLine;


        public static string consolStaticTask = @"  <Task   name=""<<ENTITY>>""
          description=""""
          type=""Symmetrix.DataStage.Tasks.ExecuteSQLTask, Symmetrix.DataStage""
          depends=""""
          enabled=""True"">

        <Property name=""ConnectionName"" value=""Stage""/>
        <Property name=""SQLStatement"">
<![CDATA[

<<TEMP TABLE>>

<<NEW IMPORT>>


  	/************************************
	*									
	*	REFRESH Attributes	
	*									
	*************************************/ 


<<NEW REFRESH>>
  
]]>
    </Property>
    <Parameters />
  </Task>

    <<NEW TASK>>

  ";
    }
}
