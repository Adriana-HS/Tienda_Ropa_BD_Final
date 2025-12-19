
CREATE PROCEDURE Insertar_Categoria
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 15/12/25
 Procedimiento : Insertar_Categoria
 Descripción   : Inserta una nueva categoría validando que no existan duplicados.
===========================================================================
*/
    @Nombre NVARCHAR(100),
    @Detalles NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Categoria WHERE Nombre = @Nombre)
    BEGIN
        RAISERROR('La categoría "%s" ya existe.', 16, 1, @Nombre);
        RETURN;
    END

    INSERT INTO Categoria (Nombre, Detalles)
    VALUES (@Nombre, @Detalles);
END