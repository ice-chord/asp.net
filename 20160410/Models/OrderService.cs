using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;


namespace _20160410.Models
{
    public class OrderService
    {
        /// <summary>
		/// 取得DB連線字串
		/// </summary>
		/// <returns></returns>
		private string GetDBConnectionString()
		{
			return
				System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
		}

		/// <summary>
		/// 新增訂單
		/// </summary>
		/// <param name="order"></param>
		/// <returns>訂單編號</returns>
		public string InsertOrder(Models.Order order)
		{
            string sql = @" 
	                       Insert INTO Sales.Orders
						 (
							CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipperID
                            ,Freight,ShipCountry,ShipCity,ShipRegion,ShipPostalCode,ShipAddress,ShipName
						)
						VALUES
						(
							@CustomerID ,@EmployeeID,@Orderdate,@RequireDdate,@ShippedDate,
							 @ShipperID ,@Freight,
							@ShipCountry,@ShipCity,@ShipRegion,@ShipPostalCode,@ShipAddress,@ShipName
						);
						Select SCOPE_IDENTITY()
						";
            string orderId;
			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", order.CustomerID));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", order.employeeid));
                cmd.Parameters.Add(new SqlParameter("@Orderdate", order.Orderdate));
                cmd.Parameters.Add(new SqlParameter("@RequireDdate", order.RequireDdate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", order.ShippedDate));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", order.ShipperId));
                cmd.Parameters.Add(new SqlParameter("@Freight", order.Freight));
                cmd.Parameters.Add(new SqlParameter("@ShipName", order.ShipName));
                cmd.Parameters.Add(new SqlParameter("@ShipAddress", order.ShipAddress));
                cmd.Parameters.Add(new SqlParameter("@ShipCity", order.ShipCity));
                cmd.Parameters.Add(new SqlParameter("@ShipRegion", order.ShipRegion));
                cmd.Parameters.Add(new SqlParameter("@ShipPostalCode", order.ShipPostalCode));
                cmd.Parameters.Add(new SqlParameter("@ShipCountry", order.ShipCountry));

				orderId= cmd.ExecuteScalar().ToString();
				conn.Close();
			}
			return orderId;

		}
		/// <summary>
		/// 依照Id 取得訂單資料
		/// </summary>
		/// <returns></returns>
		public Models.Order GetOrderById(string orderId)
		{
			DataTable dt = new DataTable();
			string sql = @"SELECT 
					A.OrderId,A.CustomerID,B.Companyname As CustomerName,
					A.employeeid,C.lastname+ C.firstname As EmployeeName,
					A.Orderdate,A.RequireDdate,A.ShippedDate,
					A.ShipperId,D.companyname As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.shipperid=D.shipperid
					Where  A.OrderId=@OrderId";


			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.Add(new SqlParameter("@OrderId", orderId));
				
				SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
				sqlAdapter.Fill(dt);
				conn.Close();
			}
			return this.MapOrderDataToList(dt).FirstOrDefault();
		}
		/// <summary>
		/// 依照條件取得訂單資料
		/// </summary>
		/// <returns></returns>
		public List<Models.Order> GetOrderByCondtioin(Models.OrderSearchArg arg)
		{

			DataTable dt = new DataTable();
            string sql = @"SELECT 
					A.OrderId,A.CustomerID,B.Companyname As CustomerName,
					A.employeeid,C.lastname+ C.firstname As EmployeeName,
					A.Orderdate,A.RequireDdate,A.ShippedDate,
					A.ShipperId,D.companyname As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.shipperid=D.shipperid
					Where (B.Companyname Like @CustomerName Or @CustomerName='') And 
						  (A.Orderdate=@Orderdate Or @Orderdate='') 
						   And (A.OrderId=@OrderId Or @OrderId='') 
                           and (D.ShipperID=@ShipperId )
                           and (C.EmployeeID = @EmployeeId)
                           and (A.RequiredDate=@RequiredDate Or @RequiredDate='')
                           and (A.ShippedDate=@ShippedDate Or @ShippedDate='')
						    ";
            ///	                          
					   
             

			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.Add(new SqlParameter("@CustomerName", arg.CustomerName == null ? string.Empty : arg.CustomerName));
				cmd.Parameters.Add(new SqlParameter("@Orderdate", arg.OrderDate==null?string.Empty:arg.OrderDate));
                cmd.Parameters.Add(new SqlParameter("@OrderId", arg.OrderId == null ? int.MinValue:arg.OrderId));
                cmd.Parameters.Add(new SqlParameter("@ShipperId", arg.ShipperID == null ? string.Empty : arg.ShipperID));
                cmd.Parameters.Add(new SqlParameter("@ShipperDate", arg.ShippedDate == null ? string.Empty : arg.ShippedDate));
                cmd.Parameters.Add(new SqlParameter("@EmployeeId", arg.EmployeeId == null ? string.Empty : arg.EmployeeId));
                cmd.Parameters.Add(new SqlParameter("@RequiredDate", arg.RequireDdate == null ? string.Empty :arg.RequireDdate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", arg.ShippedDate == null ? string.Empty : arg.ShippedDate));
				SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
				sqlAdapter.Fill(dt);
				conn.Close();
			}
			
			
			return this.MapOrderDataToList(dt);
		}
		/// <summary>
		/// 刪除訂單
		/// </summary>
		public void DeleteOrderById(string orderId)
		{
			try
			{
                string sql = "Delete a FROM Sales.Orders a join Sales.OrderDetails b on a.orderid=b.orderid  Where a.orderid=@orderid";
				using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand(sql, conn);
					cmd.Parameters.Add(new SqlParameter("@orderid", orderId));
					cmd.ExecuteNonQuery();
					conn.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}


		}

		/// <summary>
		/// 更新訂單
		/// </summary>
		/// <param name="order"></param>
		public void UpdateOrder(Models.Order order)
		{
			string sql = @"Update 
							Sales.Orders SET 
							CustomerID=@CustomerID,employeeid=@employeeid,
							orderdate=@orderdate,requireddate=@requireddate,
							shippeddate=@shippeddate,shipperid=@shipperid,
							freight=@freight,shipname=@shipname,
							shipaddress=@shipaddress,shipcity=@shipcity,
							shipregion=@shipregion,shippostalcode=@shippostalcode,
							shipcountry=@shipcountry
							WHERE orderid=@orderid";

			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.Add(new SqlParameter("@customerid", order.CustomerID));
				cmd.Parameters.Add(new SqlParameter("@employeeid", order.employeeid));
				cmd.Parameters.Add(new SqlParameter("@orderdate", order.Orderdate));
				cmd.Parameters.Add(new SqlParameter("@requireddate", order.RequireDdate));
				cmd.Parameters.Add(new SqlParameter("@shippeddate", order.ShippedDate));
				cmd.Parameters.Add(new SqlParameter("@shipperid", order.ShipperId));
				cmd.Parameters.Add(new SqlParameter("@freight", order.Freight));
				cmd.Parameters.Add(new SqlParameter("@shipname", order.ShipperName));
				cmd.Parameters.Add(new SqlParameter("@shipaddress", order.ShipAddress));
				cmd.Parameters.Add(new SqlParameter("@shipcity", order.ShipCity));
				cmd.Parameters.Add(new SqlParameter("@shipregion", order.ShipRegion));
				cmd.Parameters.Add(new SqlParameter("@shippostalcode", order.ShipPostalCode));
				cmd.Parameters.Add(new SqlParameter("@shipcountry", order.ShipCountry));
				cmd.Parameters.Add(new SqlParameter("@orderid", order.OrderId));
				cmd.ExecuteNonQuery();
				conn.Close();
			}

		}

        private List<Models.Order> MapOrderDataToList(DataTable orderData)
        {
            List<Models.Order> result = new List<Order>();


            foreach (DataRow row in orderData.Rows)
            {
                result.Add(new Order()
                {
                    CustomerID = row["CustomerId"].ToString(),
                    CustomerName = row["CustomerName"].ToString(),
                    employeeid = (int)row["EmployeeId"],
                    EmployeeName = row["EmployeeName"].ToString(),
                    Freight = (decimal)row["Freight"],
                    Orderdate = row["Orderdate"] == DBNull.Value ? (DateTime?)null : (DateTime)row["Orderdate"],
                    OrderId = (int)row["OrderId"],
                    RequireDdate = row["RequireDdate"] == DBNull.Value ? (DateTime?)null : (DateTime)row["RequireDdate"],
                    ShipAddress = row["ShipAddress"].ToString(),
                    ShipCity = row["ShipCity"].ToString(),
                    ShipCountry = row["ShipCountry"].ToString(),
                    ShipName = row["ShipName"].ToString(),
                    ShippedDate = row["ShippedDate"] == DBNull.Value ? (DateTime?)null : (DateTime)row["ShippedDate"],
                    ShipperId = (int)row["ShipperId"],
                    ShipperName = row["ShipperName"].ToString(),
                    ShipPostalCode = row["ShipPostalCode"].ToString(),
                    ShipRegion = row["ShipRegion"].ToString()
                });
            }
            return result;
        }
    }
}