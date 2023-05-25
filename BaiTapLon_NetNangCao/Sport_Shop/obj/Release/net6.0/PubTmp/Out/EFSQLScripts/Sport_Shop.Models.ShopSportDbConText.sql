IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE TABLE [Category] (
        [CategoryId] int NOT NULL IDENTITY,
        [TenDanhMuc] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY ([CategoryId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE TABLE [Customers] (
        [CustomerId] int NOT NULL IDENTITY,
        [HoTen] nvarchar(100) NULL,
        [GioiTinh] nvarchar(10) NULL,
        [NgaySinh] datetime2 NULL,
        [Phone] nvarchar(15) NULL,
        [Anh] Varchar(300) NULL,
        [DiaChi] nvarchar(200) NULL,
        [NgayTao] datetime2 NOT NULL,
        [TaiKhoan] nvarchar(max) NOT NULL,
        [MatKhau] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE TABLE [EmployeeAccount] (
        [EmployeeAccountId] int NOT NULL IDENTITY,
        [TaiKhoan] VARCHAR(40) NOT NULL,
        [MatKhau] VARCHAR(100) NOT NULL,
        CONSTRAINT [PK_EmployeeAccount] PRIMARY KEY ([EmployeeAccountId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE TABLE [Size] (
        [SizeId] int NOT NULL IDENTITY,
        [TenSize] NVARCHAR(20) NOT NULL,
        CONSTRAINT [PK_Size] PRIMARY KEY ([SizeId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE TABLE [Product] (
        [ProductId] int NOT NULL IDENTITY,
        [TenSanPham] nvarchar(max) NOT NULL,
        [MoTa] nvarchar(max) NOT NULL,
        [Anh] nvarchar(max) NULL,
        [Gia] int NOT NULL,
        [HoatDong] int NOT NULL,
        [GiamGia] int NOT NULL,
        [TrangThai] nvarchar(max) NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_Product] PRIMARY KEY ([ProductId]),
        CONSTRAINT [FK_Product_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([CategoryId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE TABLE [Order] (
        [OrderId] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [TotalPrice] int NOT NULL,
        [NguoiNhan] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [DiaChi] nvarchar(max) NOT NULL,
        [NgayDat] datetime2 NOT NULL,
        CONSTRAINT [PK_Order] PRIMARY KEY ([OrderId]),
        CONSTRAINT [FK_Order_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE TABLE [Employee] (
        [EmployeeId] int NOT NULL IDENTITY,
        [HoTen] NVARCHAR(60) NOT NULL,
        [GioiTinh] NVARCHAR(10) NOT NULL,
        [NgaySinh] datetime2 NOT NULL,
        [DiaChi] NVARCHAR(200) NOT NULL,
        [Phone] VARCHAR(15) NOT NULL,
        [NgayVao] datetime2 NOT NULL,
        [ChucVu] int NOT NULL,
        [Anh] VARCHAR(200) NOT NULL,
        [EmployeeAccountId] int NOT NULL,
        CONSTRAINT [PK_Employee] PRIMARY KEY ([EmployeeId]),
        CONSTRAINT [FK_Employee_EmployeeAccount_EmployeeAccountId] FOREIGN KEY ([EmployeeAccountId]) REFERENCES [EmployeeAccount] ([EmployeeAccountId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE TABLE [ProductSize] (
        [ProductSizeId] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [SizeId] int NOT NULL,
        [SoLuongTon] int NOT NULL,
        CONSTRAINT [PK_ProductSize] PRIMARY KEY ([ProductId], [SizeId], [ProductSizeId]),
        CONSTRAINT [FK_ProductSize_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([ProductId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductSize_Size_SizeId] FOREIGN KEY ([SizeId]) REFERENCES [Size] ([SizeId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE TABLE [OrderDetail] (
        [OrderDetailId] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [ProductId] int NOT NULL,
        [SizeId] int NOT NULL,
        [SoLuong] int NOT NULL,
        [Gia] int NOT NULL,
        CONSTRAINT [PK_OrderDetail] PRIMARY KEY ([OrderDetailId]),
        CONSTRAINT [FK_OrderDetail_Order_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Order] ([OrderId]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderDetail_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([ProductId]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderDetail_Size_SizeId] FOREIGN KEY ([SizeId]) REFERENCES [Size] ([SizeId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE UNIQUE INDEX [IX_Employee_EmployeeAccountId] ON [Employee] ([EmployeeAccountId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE INDEX [IX_Order_CustomerId] ON [Order] ([CustomerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE INDEX [IX_OrderDetail_OrderId] ON [OrderDetail] ([OrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE INDEX [IX_OrderDetail_ProductId] ON [OrderDetail] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE INDEX [IX_OrderDetail_SizeId] ON [OrderDetail] ([SizeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE INDEX [IX_Product_CategoryId] ON [Product] ([CategoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE INDEX [IX_ProductSize_SizeId] ON [ProductSize] ([SizeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    CREATE UNIQUE INDEX [IX_Size_TenSize] ON [Size] ([TenSize]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424173618_gs')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230424173618_gs', N'6.0.12');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424193644_sdg')
BEGIN
    ALTER TABLE [Order] ADD [TrangThai] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230424193644_sdg')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230424193644_sdg', N'6.0.12');
END;
GO

COMMIT;
GO

