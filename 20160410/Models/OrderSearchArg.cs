using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20160410.Models
{
    public class OrderSearchArg
    {
        public string CustomerName { get; set; }
        public string OrderDate { get; set; }
        public string EmployeeId { get; set; }
        public string DeleteOrderId { get; set; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 出貨公司名稱
        /// </summary>
        /// 
        public string ShipperID { get; set; }
        public string ShipperName { get; set; }
        public string RequireDdate { get; set; }
        public string ShippedDate { get; set; }
    }
}