using System.Collections;
using System.Threading;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;

namespace LongOperations
{
    public class AdditionalGraph : PXGraph
    {
        public PXSelect<LPOrderLine> OrderLine;
        public PXAction<LPOrderLine> additionalAction;

        [PXButton]
        public IEnumerable AdditionalAction(PXAdapter adapter)
        {
            var row = OrderLine.Insert();
            InventoryItem inventory = SelectFrom<InventoryItem>.Where<InventoryItem.inventoryCD.IsEqual<@P.AsString>>
               .View.Select(this, "AALEGO500");
            row.InventoryID = inventory?.InventoryID;
            row.OrderQty = 10M;
            row.OrderAmt = 100M;
            OrderLine.Insert(row);
            //OrderLine.Current = row;
            Thread.Sleep(2000);
            return adapter.Get();
        }
    }
}