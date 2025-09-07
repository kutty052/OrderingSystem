-- Seed data
INSERT INTO Products (Name, Price, StockQty) VALUES
('Widget A', 10.00, 100),
('Widget B', 20.00, 50),
('Gadget C', 30.00, 30),
('Doohickey D', 40.50, 200),
('Thingamajig E', 50.00, 80);

INSERT INTO Customers (Name, Email, Phone) VALUES
('Alice Johnson', 'alice@example.com', '555-0100'),
('Bob Smith', 'bob@example.com', '555-0110'),
('Carol Lee', 'carol@example.com', '555-0120'),
('Delta Co', 'sales@deltaco.com', '555-0130'),
('Eve Adams', 'eve@example.com', '555-0140');

-- Create a few orders
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (1, DATEADD(day,-2,GETDATE()), 30.00);
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (2, DATEADD(day,-1,GETDATE()), 30.00);
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (3, DATEADD(day,-3,GETDATE()), 20.00);
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (4, DATEADD(day,-2,GETDATE()), 51.99);
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (5, DATEADD(day,-1,GETDATE()), 59.97);
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (1, DATEADD(day,-3,GETDATE()), 29.99);
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (2, DATEADD(day,-2,GETDATE()), 66.97);
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (3, DATEADD(day,-1,GETDATE()), 51.99);
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (4, DATEADD(day,-3,GETDATE()), 29.99);
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) VALUES (5, DATEADD(day,-2,GETDATE()), 66.97);

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (1, 1, 3, 10.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (2, 3, 1, 30.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (3, 2, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (4, 1, 3, 10.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (5, 3, 1, 30.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (6, 1, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (6, 2, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (6, 3, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (6, 4, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (6, 5, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (7, 1, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (7, 2, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (7, 3, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (7, 4, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (7, 5, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (8, 1, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (8, 2, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (9, 3, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (9, 4, 1, 20.00);
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (10, 5, 1, 20.00);

-- Create default user: username 'admin', password 'Password123'
DECLARE @pw NVARCHAR(256) = CONVERT(NVARCHAR(256), 'Password123');
-- Hash using SHA256 base64 (done in app during login)
INSERT INTO Users (Username, PasswordHash) VALUES ('admin', CONVERT(NVARCHAR(256), 
    CAST(0x00 AS VARBINARY(MAX))
));

Update Users set passwordHash='n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=' where username='admin';
-- NOTE: Replace the password hash manually after running or use SQL to compute hash.
