using Microsoft.Data.SqlClient;
using OrderingSystem.Models;
using OrderingSystem.Data;

public class ProductRepository
{
    private readonly DbHelper _db;
    public ProductRepository(DbHelper db) => _db = db;

    public (List<Product>, int) GetProducts(string search, int page, int pageSize)
    {
        var products = new List<Product>();
        int totalCount = 0;

        using var conn = _db.GetOpenConnection();
        string sql = @"
                    -- total count
                    SELECT COUNT(*) 
                    FROM Products
                    WHERE (@search IS NULL OR Name LIKE '%' + @search + '%');

                    -- paged data
                    SELECT Id, Name, Price, StockQty
                    FROM Products
                    WHERE (@search IS NULL OR Name LIKE '%' + @search + '%')
                    ORDER BY Id
                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                ";

        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add(new SqlParameter("@search", string.IsNullOrWhiteSpace(search) ? DBNull.Value : search));
            cmd.Parameters.Add(new SqlParameter("@offset", (page - 1) * pageSize));
            cmd.Parameters.Add(new SqlParameter("@pageSize", pageSize));

            using var reader = cmd.ExecuteReader();
 
            if (reader.Read()) totalCount = reader.GetInt32(0);

            reader.NextResult();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    StockQty = reader.GetInt32(3)
                });
            }
        }

        return (products, totalCount);
    }

    public List<Product> GetAll()
    {
        var products = new List<Product>();
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Price, StockQty, CreatedAt FROM Products ORDER BY Name";
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        while (reader.Read())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                StockQty = reader.GetInt32(3),
                CreatedAt = reader.GetDateTime(4)
            });
        }
        return products;
    }

    public Product? GetById(int id)
    {
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Price, StockQty, CreatedAt FROM Products WHERE Id = @id";
        var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = id; cmd.Parameters.Add(p);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new Product
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Price = reader.GetDecimal(2),
            StockQty = reader.GetInt32(3),
            CreatedAt = reader.GetDateTime(4)
        };
    }


    public int Create(Product model)
    {
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO Products (Name, Price, StockQty)
        VALUES (@name, @price, @stock); SELECT SCOPE_IDENTITY();";
        cmd.Parameters.Add(new SqlParameter("@name", model.Name));
        cmd.Parameters.Add(new SqlParameter("@price", model.Price));
        cmd.Parameters.Add(new SqlParameter("@stock", model.StockQty));
        var id = cmd.ExecuteScalar();
        return Convert.ToInt32(id);
    }


    public void Update(Product model)
    {
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Products SET Name=@name,Price=@price, StockQty=@stock WHERE Id=@id";
        cmd.Parameters.Add(new SqlParameter("@name", model.Name));
        cmd.Parameters.Add(new SqlParameter("@price", model.Price));
        cmd.Parameters.Add(new SqlParameter("@stock", model.StockQty));
        cmd.Parameters.Add(new SqlParameter("@id", model.Id));
        cmd.ExecuteNonQuery();
    }


    public void Delete(int id)
    {
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Products WHERE Id = @id";
        cmd.Parameters.Add(new SqlParameter("@id", id));
        cmd.ExecuteNonQuery();
    }
}
