CREATE DATABASE ApiBD;
GO

USE ApiBD;
GO

CREATE TABLE Usuarios_Sistema (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Senha NVARCHAR(200) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    CPF VARCHAR(14) NULL,
    DataNascimento DATE NULL,
    GestorId INT NULL,
    CONSTRAINT FK_Usuarios_Gestor 
    FOREIGN KEY (GestorId) REFERENCES Usuarios_Sistema(Id)
);
GO

CREATE TABLE Departamento (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Nome VARCHAR(100) NOT NULL,
  Descricao VARCHAR(100) NULL,
  GestorId INT NULL
);

CREATE TABLE DepartamentoUsuario (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Usuarios_SistemaId INT NOT NULL,
  DepartamentoId INT NOT NULL,

  FOREIGN KEY (Usuarios_SistemaId) REFERENCES Usuarios_Sistema(Id),
  FOREIGN KEY (DepartamentoId) REFERENCES Departamento(Id)
);

CREATE TABLE Curso (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo VARCHAR(40) NOT NULL,
    Descricao VARCHAR(MAX) NULL,
    Usuarios_SistemaId INT NOT NULL,

    FOREIGN KEY (Usuarios_SistemaId) REFERENCES Usuarios_Sistema(Id)
);

CREATE TABLE Modulo (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo VARCHAR(30) NOT NULL,
    Ordem INT NOT NULL,
    CursoId INT NOT NULL,

    FOREIGN KEY (CursoId) REFERENCES Curso(Id)
);

CREATE TABLE Aula (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo VARCHAR(200) NOT NULL,
    Conteudo VARCHAR(MAX) NULL,
    ChaveVideo VARCHAR(2000) NULL,
    Ordem INT NOT NULL,
    ModuloId INT NOT NULL,

    FOREIGN KEY (ModuloId) REFERENCES Modulo(Id)
);

CREATE TABLE Inscricao (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Usuarios_SistemaId INT NOT NULL,
    CursoId INT NOT NULL,

    FOREIGN KEY (Usuarios_SistemaId) REFERENCES Usuarios_Sistema(Id),
    FOREIGN KEY (CursoId) REFERENCES Curso(Id)
);

CREATE TABLE ProgressoCurso (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Usuarios_SistemaId INT NOT NULL,
    CursoId INT NOT NULL,
    ModuloId INT NOT NULL,
    AulaId INT NOT NULL,
    Status VARCHAR(20) NOT NULL,

    FOREIGN KEY (Usuarios_SistemaId) REFERENCES Usuarios_Sistema(Id),
    FOREIGN KEY (CursoId) REFERENCES Curso(Id),
    FOREIGN KEY (ModuloId) REFERENCES Modulo(Id),
    FOREIGN KEY (AulaId) REFERENCES Aula(Id)
);

CREATE TABLE AtribuicaoCurso (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GestorId INT NOT NULL,
    CursoId INT NOT NULL,
    DepartamentoId INT NULL,
    FOREIGN KEY (GestorId) REFERENCES Usuarios_Sistema(Id),
    FOREIGN KEY (CursoId) REFERENCES Curso(Id),
    
    CONSTRAINT FK_AtribuicaoCurso_Departamento FOREIGN KEY (DepartamentoId) REFERENCES Departamento(Id)
);
