using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPC_Package_Generator
{
    class ConsolidateInstrumentStrings
    {
        public static string consolInsPackageStart = @"<?xml version=""1.0"" encoding=""utf-8""?>
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
           name=""ConsolidateInstrument""
           description=""""
           log-level=""All"">

    <Variables />

    <Loggers />

    <Connections />

    <Tasks>

      <!-- add tasks here -->" + Environment.NewLine
                       + Environment.NewLine;



        public static string consolInsPackageInstrumentTaskStart = @"      <Task name=""Instrument""
            description=""""
            type=""Symmetrix.DataStage.Tasks.ExecuteSQLTask, Symmetrix.DataStage""
            depends=""""
            enabled=""True"">

        <Property name=""ConnectionName"" value=""Stage""/>
        <Property name=""SQLStatement"">
<![CDATA[
 
 set nocount on

  ";






    }
}
