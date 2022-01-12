using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.IN;

namespace LongOperations
{
    [Serializable]
    [PXCacheName("LPOrderLine")]
    public class LPOrderLine : IBqlTable
    {
        #region Keys

        public class PK: PrimaryKeyOf<LPOrderLine>.By<id>
        {
            public LPOrderLine Find(PXGraph graph, int id) => FindBy(graph, id);
        }    

        public static class FK
        {
            public class Order : LPOrder.PK.ForeignKeyOf<LPOrderLine>.By<orderID> { }
        }    

        #endregion

        #region Id
        [PXDBIdentity(IsKey = true)]
        public virtual int? Id { get; set; }
        public abstract class id : PX.Data.BQL.BqlInt.Field<id> { }
        #endregion

        #region OrderID
        [PXDBInt()]
        [PXParent(typeof(FK.Order))]
        [PXDBDefault(typeof(LPOrder.id), DefaultForUpdate = false)]
        [PXUIField(DisplayName = "Order ID")]
        public virtual int? OrderID { get; set; }
        public abstract class orderID : PX.Data.BQL.BqlInt.Field<orderID> { }
        #endregion

        #region InventoryID
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        [StockItem]
        [PXForeignReference(typeof(Field<inventoryID>.IsRelatedTo<InventoryItem.inventoryID>))]
        [PXUIField(DisplayName = "Item ID")]
        public virtual int? InventoryID
        {
            get;
            set;
        }
        #endregion

        #region OrderQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Order Qty")]
        public virtual Decimal? OrderQty { get; set; }
        public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
        #endregion

        #region OrderAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Order Amt")]
        public virtual Decimal? OrderAmt { get; set; }
        public abstract class orderAmt : PX.Data.BQL.BqlDecimal.Field<orderAmt> { }
        #endregion
    }
}