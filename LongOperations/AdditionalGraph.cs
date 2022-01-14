using System.Collections;
using System.Collections.Generic;
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
            PXLongOperation.StartOperation(this, () =>
            {
                Thread.Sleep(2000);
            });
            return adapter.Get();
        }
    }
}