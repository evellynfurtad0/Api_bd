CREATE DATABASE ApiBD;
GO

USE ApiBD;
GO

CREATE TABLE Usuarios_Sistema (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE
);
GO
ALTER TABLE Usuarios_Sistema
ADD CONSTRAINT CK_Role CHECK (Role IN ('Admin', 'Funcionario', 'Gestor'));

ALTER TABLE Usuarios_Sistema
ALTER COLUMN Role INT;

ALTER TABLE Usuarios_Sistema
ADD CONSTRAINT CK_Role CHECK (Role IN (1, 2, 3));

UPDATE Usuarios_Sistema SET Role = 1 WHERE Role = 'Funcionario';
UPDATE Usuarios_Sistema SET Role = 2 WHERE Role = 'Gestor';
UPDATE Usuarios_Sistema SET Role = 3 WHERE Role = 'Admin';

-- ALTER TABLE Usuarios_Sistema
-- ADD Senha NVARCHAR(200) NOT NULL DEFAULT('');
