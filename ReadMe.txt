Database script:
USE [UsersDB]
GO

/****** Object: Table [dbo].[Users] Script Date: 07/04/2025 02:15:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Users];


GO
CREATE TABLE [dbo].[Users] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [FirstName] NVARCHAR (100)   NOT NULL,
    [LastName]  NVARCHAR (100)   NOT NULL,
    [Email]     NVARCHAR (255)   NOT NULL,
    [CreatedAt] DATETIME2 (7)    NOT NULL
);


