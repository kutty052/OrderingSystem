using Microsoft.Data.SqlClient;
using OrderingSystem.Models;
using OrderingSystem.Data;

public class CustomerRepository
{
    private readonly DbHelper _db;
    public CustomerRepository(DbHelper db) => _db = db;

    public (List<Customer>, int) GetCustomers(string search, int page, int pageSize)
    {
        var customers = new List<Customer>();
        int totalCount = 0;

        using var conn = _db.GetOpenConnection();
        string sql = @"
                    -- total count
                    SELECT COUNT(*) 
                    FROM Customers
                    WHERE (@search IS NULL OR Name LIKE '%' + @search + '%');

                    -- paged data
                    SELECT Id, Name, Email, Phone, CreatedAt FROM Customers
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
                customers.Add(new Customer
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Phone = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)
                });
            }
        }

        return (customers, totalCount);
    }

    public List<Customer> GetAll()
    {
        var customers = new List<Customer>();
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Email, Phone, CreatedAt FROM Customers ORDER BY Name";

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        while (reader.Read())
        {
            customers.Add(new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.GetString(3),
                CreatedAt = reader.GetDateTime(4)
            });
        }
        return customers;
    }

    public Customer? GetById(int id)
    {
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Email, Phone, CreatedAt FROM Customers WHERE Id = @id";
        var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = id; cmd.Parameters.Add(p);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new Customer
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2),
            Phone = reader.GetString(3),
            CreatedAt = reader.GetDateTime(4)
        };
    }


    public int Create(Customer model)
    {
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO Customers (Name, Email, Phone)
        VALUES (@name, @email, @phone); SELECT SCOPE_IDENTITY();";
        cmd.Parameters.Add(new SqlParameter("@name", model.Name));
        cmd.Parameters.Add(new SqlParameter("@email", model.Email));
        cmd.Parameters.Add(new SqlParameter("@phone", model.Phone));
        var id = cmd.ExecuteScalar();
        return Convert.ToInt32(id);
    }


    public void Update(Customer model)
    {
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Customers SET Name=@name,Email=@email, Phone=@phone WHERE Id=@id";
        cmd.Parameters.Add(new SqlParameter("@name", model.Name));
        cmd.Parameters.Add(new SqlParameter("@email", model.Email));
        cmd.Parameters.Add(new SqlParameter("@phone", model.Phone));
        cmd.Parameters.Add(new SqlParameter("@id", model.Id));
        cmd.ExecuteNonQuery();
    }


    public void Delete(int id)
    {
        using var conn = _db.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Customers WHERE Id = @id";
        cmd.Parameters.Add(new SqlParameter("@id", id));
        cmd.ExecuteNonQuery();
    }
}
