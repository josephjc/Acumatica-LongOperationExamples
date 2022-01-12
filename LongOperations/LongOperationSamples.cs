using System;
using System.Collections;
using System.Threading;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;

namespace LongOperations
{
    public class LongOperationSamples : PXGraph<LongOperationSamples>
    {
        #region Views and Actions
        public PXSave<LPOrder> Save;
        public PXCancel<LPOrder> Cancel;
        public PXPrevious<LPOrder> Previous;
        public PXNext<LPOrder> Next;

        public PXSelect<LPOrder> MasterView;
        public PXSelect<LPOrderLine, Where<LPOrderLine.orderID, Equal<Current<LPOrder.id>>>> DetailsView;

        public PXAction<LPOrder> test1Success;
        public PXAction<LPOrder> test2Error;
        public PXAction<LPOrder> test3StaticMethod;
        public PXAction<LPOrder> test4RefreshUI;
        public PXAction<LPOrder> test5NestedAction;
        #endregion

        #region Data View Delegates

        public virtual IEnumerable detailsView()
        {
            //Execute Query
            var result = new PXView(this, false, DetailsView.View.BqlSelect).SelectMulti();
            
            //Get order line created from long operation. Returns null if does not exist
            var addedOrderLine = PXLongOperation.GetCustomInfoPersistent(this.UID) as LPOrderLine;
            
            //Clear long operation CustomInfo so that order line is not added a second time
            PXLongOperation.RemoveCustomInfoPersistent(this.UID);

            //Add line 
            if (addedOrderLine?.InventoryID != null)
            {
                result.Add(addedOrderLine);
                DetailsView.Insert(addedOrderLine);
            }
            return result;
        }

        #endregion

        #region Action Impl

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Test1-Success")]
        public IEnumerable Test1Success(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, () =>
            {
                //Simulate long operation
                Thread.Sleep(2000);
            });
            return adapter.Get();
        }


        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Test2-Error")]
        public IEnumerable Test2Error(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, () =>
            {
                //Simulate error in long opeartion
                Thread.Sleep(2000);
                throw new PXException(LPMessages.SAMPLE_ERROR);
            });
            return adapter.Get();
        }


        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Test3-StaticMethod")]
        public IEnumerable Test3StaticMethod(PXAdapter adapter)
        {
            var graphCopy = this;
            PXLongOperation.StartOperation(graphCopy, () =>
            {
                //execute logic in seperate static method to ensure independent execution
                DoTest3StaticMethod(graphCopy);
            });
            return adapter.Get();
        }

        private static void DoTest3StaticMethod(LongOperationSamples graphCopy)
        {
            //Can access and use graph safely, still ensuring independent execution
            var uid = graphCopy.UID;
            Thread.Sleep(2000);
        }

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Test4-RefreshUI")]
        public IEnumerable Test4RefreshUI(PXAdapter adapter)
        {
            var graphCopy = this;
            PXLongOperation.StartOperation(graphCopy, () =>
            {                
                DoTest4RefreshUI(graphCopy);
            });
            return adapter.Get();
        }

        private static void DoTest4RefreshUI(LongOperationSamples graphCopy)
        {
            //Add new Orderline 
            var row = graphCopy.DetailsView.Insert();
            InventoryItem inventory = SelectFrom<InventoryItem>.Where<InventoryItem.inventoryCD.IsEqual<@P.AsString>>
                .View.Select(graphCopy, "AACOMPUT01");
            row.InventoryID = inventory?.InventoryID;
            row.OrderQty = 10M;
            row.OrderAmt = 100M;
            graphCopy.DetailsView.Update(row);

            //Put Orderline in Custom Info Persistent
            PXLongOperation.SetCustomInfoPersistent(row);

            Thread.Sleep(2000);
        }

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Test5-NestedAction")]
        public IEnumerable Test5NestedAction(PXAdapter adapter)
        {
            var graphCopy = this;
            PXLongOperation.StartOperation(graphCopy, () =>
            {
                DoTest5NestedAction(graphCopy);
            });
            return adapter.Get();
        }

        private static void DoTest5NestedAction(LongOperationSamples graphCopy)
        {
            //Simulate long operation
            Thread.Sleep(2000);
            
            //Prepare data in custom info persistent (to be populated at a later stage)
            PXLongOperation.SetCustomInfoPersistent(new LPOrderLine());

            //Register Custom Info which would call another action on another graph
            PXLongOperation.SetCustomInfo(new NestedActionCustomInfo());
        }       

        #endregion        
    }

    internal class NestedActionCustomInfo : IPXCustomInfo
    {
        public void Complete(PXLongRunStatus status, PXGraph graph)
        {
            if (status == PXLongRunStatus.Completed && graph is LongOperationSamples longOperationSamplesGraph)
            { 
                //Call an action of another graph
                var additionalGraph = PXGraph.CreateInstance<AdditionalGraph>();
                additionalGraph.additionalAction.Press();
                //Since this action, uses a long operation, we will wait for it to be completed before proceeding
                PXLongOperation.WaitCompletion(additionalGraph.UID);

                //Update custom info based of the action (which would have been updated in the Current row)
                var line = PXLongOperation.GetCustomInfoPersistent(graph.UID) as LPOrderLine;
                line.InventoryID = additionalGraph.OrderLine.Current.InventoryID;
                line.OrderQty    = additionalGraph.OrderLine.Current.OrderQty;
                line.OrderAmt    = additionalGraph.OrderLine.Current.OrderAmt;
            }
        }
    }    
}