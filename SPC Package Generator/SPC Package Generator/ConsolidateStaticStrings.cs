using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPC_Package_Generator
{
    class ConsolidateStaticStrings
    {

        public static string consolStaticTaskStart = @"      <Task name=""{0}""
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
