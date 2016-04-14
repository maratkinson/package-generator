using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SPC_Package_Generator
{
    class ConsolidateHoldingsBuilder
    {
        SqlTableStrings sts = new SqlTableStrings();

        public string CreateHoldingInsertStatement(DataTable columnsDT, string schema, string instrumentTable)
        {
            string test = String.Empty;
            string insertBegin = @"INSERT INTO stage.PortfolioHolding
(
";  

            string columns = sts.CreateInsertColumnList(columnsDT, "Portfolio_HoldingImport");

            insertBegin += columns;

            string select = @")
SELECT
";

            string selectColumns = sts.CreateSelectColumnList(columnsDT, "Portfolio_HoldingImport");

            string selectFROM = String.Format(@"
FROM {0}.{1} src
GROUP BY PortfolioCode,
InstrumentCode

print  'Found ' + convert(varchar, @@rowcount) + ' new Holding records...'", schema, instrumentTable);

            insertBegin += select + selectColumns + selectFROM;

            Console.WriteLine(insertBegin);
            return insertBegin;
        }

        public string CreateFixHoldingStatement()
        { 
            string fixholdings = @"
update stage.PortfolioHolding set TradedHolding = 0 where TradedHolding is null
update stage.PortfolioHolding set TradedBookValue = 0 where TradedBookValue is null
update stage.PortfolioHolding set TradedMarketValue = 0 where TradedMarketValue is null
update stage.PortfolioHolding set TradedEffectiveExposure = 0 where TradedEffectiveExposure is null
update stage.PortfolioHolding set OrderedHolding = 0 where OrderedHolding is null
update stage.PortfolioHolding set OrderedBookValue = 0 where OrderedBookValue is null
update stage.PortfolioHolding set OrderedMarketValue = 0 where OrderedMarketValue is null
update stage.PortfolioHolding set OrderedEffectiveExposure = 0 where OrderedEffectiveExposure is null
update stage.PortfolioHolding set ConfirmedHolding = 0 where ConfirmedHolding is null
update stage.PortfolioHolding set ConfirmedMarketValue = 0 where ConfirmedMarketValue is null
update stage.PortfolioHolding set ConfirmedBookValue = 0 where ConfirmedBookValue is null
update stage.PortfolioHolding set ConfirmedEffectiveExposure = 0 where ConfirmedEffectiveExposure is null

";


            return fixholdings;
        }

    }
}
