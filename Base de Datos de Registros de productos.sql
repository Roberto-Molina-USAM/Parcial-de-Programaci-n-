--========================================================================================
--Roberto Carlos Bonilla Molina, 260387, Tecnico en Ingenieria de Sistemas de Computación
--Emerson Isai Galeas Beltran , 260778 , Tecnico en Ingenieria de Sistemas de Computación
--Prueba Parcial: Programación I 
--Base de Datos: ParcialProgramacion1
--Sistema de Control de Productos
--========================================================================================

IF DB_ID('ParcialProgramacion1') IS NULL
BEGIN
    CREATE DATABASE ParcialProgramacion1;
END;
GO

USE ParcialProgramacion1;
GO

-- 2. Crear la tabla Productos
CREATE TABLE dbo.Productos
(
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Categoria NVARCHAR(50) NOT NULL,
    Precio DECIMAL (10,2) NOT NULL,
    Existencias INT NOT NULL
);
GO

-- Registrar 3 productos por categoría, Bebidas, Snacks y Lacteos
INSERT INTO Productos (Nombre, Categoria, Precio, Existencias) VALUES 

('Cafe Molido', 'Bebidas', 4.50, 25),
('Jugo de Naranja', 'Bebidas', 2.10, 40),

('Galletas Dulces', 'Snacks', 1.25, 60),
('Papas Fritas', 'Snacks', 1.50, 45),

('Queso Fresco', 'Lacteos', 3.50, 20),
('Yogurt Natural', 'Lacteos', 2.00, 40)

select * from Productos