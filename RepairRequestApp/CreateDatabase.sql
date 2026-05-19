IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RepairRequestsDB')
BEGIN
    CREATE DATABASE RepairRequestsDB;
    PRINT 'База данных RepairRequestsDB создана';
END
GO

USE RepairRequestsDB;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RepairRequests]') AND type = N'U')
BEGIN
    CREATE TABLE RepairRequests (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Equipment NVARCHAR(200) NOT NULL,
        FaultType NVARCHAR(200) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        Client NVARCHAR(150) NOT NULL,
        Description NVARCHAR(500) NULL,
        CreatedDate DATETIME NOT NULL
    );
    PRINT 'Таблица RepairRequests создана';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RepairRequests_Status' AND object_id = OBJECT_ID('RepairRequests'))
BEGIN
    CREATE INDEX IX_RepairRequests_Status ON RepairRequests(Status);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RepairRequests_CreatedDate' AND object_id = OBJECT_ID('RepairRequests'))
BEGIN
    CREATE INDEX IX_RepairRequests_CreatedDate ON RepairRequests(CreatedDate);
END
GO

IF NOT EXISTS (SELECT TOP 1 * FROM RepairRequests)
BEGIN
    INSERT INTO RepairRequests (Equipment, FaultType, Status, Client, Description, CreatedDate)
    VALUES 
    ('Ноутбук Dell', 'Не включается', 'Новая', 'Иванов И.И.', 'Ноутбук не реагирует на кнопку включения', DATEADD(day, -5, GETDATE())),
    ('Смартфон iPhone', 'Разбит экран', 'В работе', 'Петров П.П.', 'Трещины на экране, требуется замена', DATEADD(day, -3, GETDATE())),
    ('Холодильник Samsung', 'Не морозит', 'Завершена', 'Сидорова А.А.', 'Холодильник работает, но не охлаждает', DATEADD(day, -7, GETDATE())),
    ('Стиральная машина LG', 'Не сливает воду', 'Новая', 'Козлов Д.Д.', 'Вода не уходит после стирки', DATEADD(day, -2, GETDATE())),
    ('Телевизор Sony', 'Нет изображения', 'В работе', 'Михайлова Е.Е.', 'Звук есть, изображения нет', DATEADD(day, -1, GETDATE()));
    PRINT 'Тестовые данные добавлены';
END
GO

SELECT * FROM RepairRequests;
GO