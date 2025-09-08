using OrderingSystem.Controllers;
using Microsoft.Data.SqlClient;
using OrderingSystem.Models;

namespace OrderingSystem.Data
{
    public class OrderRepository
    {
        private readonly DbHelper _db;
        public OrderRepository(DbHelper db)
        {
            _db = db;
        }

        public int Create(OrderCreateDto dto)
        {
            using var conn = _db.GetOpenConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                var orderSql = "INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (@cust, GETDATE(), @total); SELECT CAST(SCOPE_IDENTITY() as int)";
                using var cmdOrder = new SqlCommand(orderSql, conn, tran);
                cmdOrder.Parameters.Add(new SqlParameter("@cust", dto.CustomerId));
                cmdOrder.Parameters.Add(new SqlParameter("@total", dto.Items.Sum(i => i.UnitPrice * i.Quantity)));
                var orderId = (int)cmdOrder.ExecuteScalar();

                var itemSql = "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (@order, @prod, @qty, @price)";
                foreach (var it in dto.Items)
                {
                    using var cmdItem = new SqlCommand(itemSql, conn, tran);
                    cmdItem.Parameters.AddWithValue("@order", orderId);
                    cmdItem.Parameters.AddWithValue("@prod", it.ProductId);
                    cmdItem.Parameters.AddWithValue("@qty", it.Quantity);
                    cmdItem.Parameters.AddWithValue("@price", it.UnitPrice);
                    cmdItem.ExecuteNonQuery();
                }

                tran.Commit();
                return orderId;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return 0;
            }
        }

        public Order GetById(int id)
        {
            var order = new Order();
            var sql = @"SELECT o.Id, o.OrderDate, o.TotalAmount, c.Name as CustomerName
                        FROM Orders o
                        JOIN Customers c on c.Id = o.CustomerId
                        WHERE o.Id = @id";
            var dt = _db.ExecuteDataTable(sql, new SqlParameter("@id", id));
            if (dt.Rows.Count > 0)
            {
                order = new Order()
                {
                    Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    OrderDate = Convert.ToDateTime(dt.Rows[0]["OrderDate"]),
                    TotalAmount = Convert.ToDecimal(dt.Rows[0]["TotalAmount"]),
                    CustomerName = Convert.ToString(dt.Rows[0]["CustomerName"]),
                    Items = new List<OrderItem>()
                };
            }
            var items = _db.ExecuteDataTable(@"SELECT oi.Quantity, oi.UnitPrice, p.Name FROM OrderItems oi JOIN Products p ON p.Id=oi.ProductId WHERE oi.OrderId=@id", new SqlParameter("@id", id));

            if (order.Items == null)
                order.Items = new List<OrderItem>();

            foreach (System.Data.DataRow item in items.Rows)
            {
                order.Items.Add(new OrderItem()
                {
                    Quantity = Convert.ToInt32(item["Quantity"]),
                    UnitPrice = Convert.ToDecimal(item["UnitPrice"]),
                    Name = Convert.ToString(item["Name"])
                });
            }
            return order;
        }

        public object GetSalesStats(DateTime from, DateTime to)
        {
            var salesByDay = new List<object>();
            int totalOrders = 0;
            decimal totalSales = 0, avgOrderValue = 0;
            var topProducts = new List<object>();

            using var conn = _db.GetOpenConnection();
            {
                string sql = @"
                SELECT COUNT(*) AS TotalOrders, SUM(TotalAmount) AS TotalSales, 
                       AVG(TotalAmount) AS AvgOrderValue
                FROM Orders
                WHERE OrderDate BETWEEN @from AND @to;

                SELECT CONVERT(varchar, OrderDate, 23) as OrderDay, SUM(TotalAmount) as Sales
                FROM Orders
                WHERE OrderDate BETWEEN @from AND @to
                GROUP BY CONVERT(varchar, OrderDate, 23)
                ORDER BY OrderDay
                            
                SELECT TOP 5 P.Name, SUM(OI.Quantity * OI.UnitPrice) AS TotalSales
                FROM OrderItems OI
                JOIN Orders O ON O.Id = OI.OrderId
                JOIN Products P ON P.Id = OI.ProductId
                WHERE O.OrderDate BETWEEN @from AND @to
                GROUP BY P.Name
                ORDER BY TotalSales DESC;;
            ";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@from", from));
                cmd.Parameters.Add(new SqlParameter("@to", to));

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    totalOrders = reader["TotalOrders"] != DBNull.Value ? Convert.ToInt32(reader["TotalOrders"]) : 0;
                    totalSales = reader["TotalSales"] != DBNull.Value ? Convert.ToDecimal(reader["TotalSales"]) : 0;
                    avgOrderValue = reader["AvgOrderValue"] != DBNull.Value ? Convert.ToDecimal(reader["AvgOrderValue"]) : 0;
                }

                reader.NextResult();
                while (reader.Read())
                {
                    salesByDay.Add(new
                    {
                        Day = reader["OrderDay"].ToString(),
                        Sales = Convert.ToDecimal(reader["Sales"])
                    });
                }

                reader.NextResult();
                while (reader.Read())
                {
                    topProducts.Add(new
                    {
                        Product = reader["Name"].ToString(),
                        Sales = Convert.ToDecimal(reader["TotalSales"])
                    });
                }
            }
            return new
            {
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                AvgOrderValue = avgOrderValue,
                SalesByDay = salesByDay,
                TopProducts = topProducts
            };
        }
    }
}
