USE master;
GO

DROP TABLE IF EXISTS dbo.customers_SE
GO

DROP TABLE IF EXISTS dbo.customers_FI
GO

create table dbo.customers_SE
(
    id       int identity,
    name     varchar(255),
    phone    varchar(255),
    uploaded bit
)
GO

create table dbo.customers_FI
(
    id       int identity,
    name     varchar(255),
    phone    varchar(255),
    uploaded bit
)
GO