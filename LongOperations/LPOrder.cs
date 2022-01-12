using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;

namespace LongOperations
{
    [Serializable]
    [PXCacheName("LPOrder")]
    public class LPOrder : IBqlTable
    {

        #region Keys

        public class PK : PrimaryKeyOf<LPOrder>.By<id> 
        {
            public static LPOrder Find(PXGraph graph, int orderID) => FindBy(graph, orderID);
        }

        #endregion


        #region Id
        [PXDBIdentity(IsKey = true)]
        public virtual int? Id { get; set; }
        public abstract class id : PX.Data.BQL.BqlInt.Field<id> { }
        #endregion

        #region OrderNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Nbr")]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region OrderDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Order Date")]
        public virtual DateTime? OrderDate { get; set; }
        public abstract class orderDate : PX.Data.BQL.BqlDateTime.Field<orderDate> { }
        #endregion
    }
}