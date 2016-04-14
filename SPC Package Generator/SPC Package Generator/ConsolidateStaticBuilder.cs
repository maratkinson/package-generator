using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SPC_Package_Generator
{
    class ConsolidateStaticBuilder
    {
        SqlTableStrings sts = new SqlTableStrings();

        private string ConstructEntityImport(string entity)
        {
            string final = "";

            final = ConsolidateStaticStrings.consolStaticTask.Replace("<<ENTITY>>", entity);
            final = ConsolidateStaticStrings.consolStaticTask.Replace("<<NEW IMPORT>>", String.Concat("<<", entity, " NEW IMPORT>>"));
            final = ConsolidateStaticStrings.consolStaticTask.Replace("<<NEW REFRESH>>", String.Concat("<<", entity, " NEW REFRESH>>"));

            return final;
        }

        private string AddTempTable(DataTable dt, string entity, string schema)
        {
            string final = "";

            final = String.Concat("Declare @new"
                                  ,entity
                                  ,@" Table
                                  (
"
                                  ,sts.GenerateTempTable(schema, entity, dt)
                                  ,Environment.NewLine
                                  ,")"
                                  );

            return final;
        }

        public string AddEntityImport(DataTable dt, string entity, string schema)
        {
            string final = String.Empty;

            final = String.Concat(Environment.NewLine
                                  ,"INSERT INTO stage."
                                  ,entity
                                  ,Environment.NewLine
                                  ,"("
                                  ,Environment.NewLine
                                  , sts.CreateInsertColumnList(dt, entity + "Import")
                                  ,Environment.NewLine
                                  ,")"
                                  ,Environment.NewLine
                                  ,"Select"
                                  ,Environment.NewLine
                                  , sts.CreateSelectColumnList(dt, entity + "Import")
                                  ,Environment.NewLine
                                  ,"FROM " + schema + "." + entity + " ph"
                                  , Environment.NewLine
                                  ,"LEFT JOIN stage." + entity + " sp on sp.code = ph.code"
                                  ,Environment.NewLine
                                  ,"LEFt JOIN @new" + entity + " np on np.code = ph.code"
                                  , "WHERE sp.code is null and np.code is null"
                                  , Environment.NewLine
                                  , Environment.NewLine
                                  , "print 'Found ' + convert(varchar, @@rowcount) + ' new " + entity + " records...'"
                                  , Environment.NewLine
                                  , Environment.NewLine
                                  ,"<<NEW IMPORT>>"
                                  );

            return final;
        }

        public string AddEntityRefresh(DataTable dt, string entity, string schema)
        {
            string final = String.Empty;

            final = String.Concat(Environment.NewLine
                                  , "UPDATE stage"
                                  , Environment.NewLine
                                  , "SET"
                                  , Environment.NewLine
                                  , sts.CreateInsertColumnList(dt, entity+"Refresh")
                                  , Environment.NewLine
                                  , "FROM stage.Portfolio stage"
                                  , Environment.NewLine
                                  , "INNER JOIN " + entity + "." + entity + " src on src.Code = stage.Code"
                                  , Environment.NewLine
                                  , Environment.NewLine
                                  , "print 'Updated ' + convert(varchar, @@rowcount) + '" + entity + " records...'"
                                  , Environment.NewLine
                                  , Environment.NewLine
                                  , "<<NEW REFRESH>>"
                                  );

            return final;
        }
    }
}
