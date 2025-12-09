USE [Honse];

CREATE TABLE [Order](
    [Id] UNIQUEIDENTIFIER PRIMARY KEY NONCLUSTERED DEFAULT NEWID(),
    
    -- Link to User Account (nullable for guest orders)
    [UserId] UNIQUEIDENTIFIER NULL,
    
    -- CLIENT SNAPSHOT
    [ClientName] NVARCHAR(255) NOT NULL,
    [ClientEmail] NVARCHAR(255) NOT NULL,
    
    -- JSON ADDRESS
    [DeliveryAddress] NVARCHAR(MAX) NOT NULL, 

    -- ORDER DETAILS
    [RestaurantId] UNIQUEIDENTIFIER NOT NULL REFERENCES [Restaurant]([Id]),
    [OrderNo] VARCHAR(50) NOT NULL, 
    [Timestamp] DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [Total] DECIMAL(19, 4) NOT NULL,
    
    -- CURRENT ORDER STATUS (enum as string, NOT JSON)
    [OrderStatus] VARCHAR(50) NOT NULL,
    
    -- JSON STATUS HISTORY (array of status changes)
    [StatusHistory] NVARCHAR(MAX) NOT NULL,
    
    -- JSON PRODUCTS (array of order items)
    [Products] NVARCHAR(MAX) NOT NULL,
    
    -- Timings
    [PreparationTime] DATETIME2 NULL,
    [DeliveryTime] DATETIME2 NULL,

    -- VALIDATION: Ensure JSON fields contain valid JSON
    CONSTRAINT [CK_Order_StatusHistory_JSON] CHECK (ISJSON([StatusHistory]) = 1),
    CONSTRAINT [CK_Order_Products_JSON] CHECK (ISJSON([Products]) = 1),
    CONSTRAINT [CK_Order_Address_JSON] CHECK (ISJSON([DeliveryAddress]) = 1),
    
    -- Foreign key constraint (supports NULL for guest orders)
    CONSTRAINT [FK_Order_User] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION
)

-- Indexes
CREATE INDEX [IX_Order_UserId] ON [Order]([UserId])
CREATE INDEX [IX_Order_RestaurantId] ON [Order]([RestaurantId])
