USE [Honse];

-- Create Orders table
CREATE TABLE [dbo].[Orders]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [RestaurantId] UNIQUEIDENTIFIER NOT NULL,
    [CustomerEmail] VARCHAR(255) NOT NULL,
    [CustomerName] VARCHAR(255) NOT NULL,
    [CustomerPhone] VARCHAR(50) NOT NULL,
    [DeliveryAddress] VARCHAR(MAX) NOT NULL, -- JSON format like Restaurant.Address
    [OrderStatus] INT NOT NULL DEFAULT 0, -- 0 = Unconfirmed
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [ConfirmationToken] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_Orders_Restaurant FOREIGN KEY ([RestaurantId]) 
        REFERENCES [dbo].[Restaurant]([Id]) ON DELETE CASCADE
);

-- Create OrderItems table
CREATE TABLE [dbo].[OrderItems]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [ProductName] VARCHAR(255) NOT NULL, -- Snapshot for historical record
    [Quantity] INT NOT NULL CHECK ([Quantity] > 0),
    [UnitPrice] DECIMAL(18,2) NOT NULL, -- Snapshot for price verification
    [Subtotal] DECIMAL(18,2) NOT NULL,
    
    CONSTRAINT FK_OrderItems_Order FOREIGN KEY ([OrderId]) 
        REFERENCES [dbo].[Orders]([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Product FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Product]([Id]) ON DELETE NO ACTION
);

-- Create indexes for better query performance
CREATE INDEX IX_Orders_RestaurantId ON [dbo].[Orders]([RestaurantId]);
CREATE INDEX IX_Orders_OrderStatus ON [dbo].[Orders]([OrderStatus]);
CREATE INDEX IX_Orders_ConfirmationToken ON [dbo].[Orders]([ConfirmationToken]);
CREATE INDEX IX_Orders_CreatedAt ON [dbo].[Orders]([CreatedAt]);
CREATE INDEX IX_OrderItems_OrderId ON [dbo].[OrderItems]([OrderId]);
CREATE INDEX IX_OrderItems_ProductId ON [dbo].[OrderItems]([ProductId]);
