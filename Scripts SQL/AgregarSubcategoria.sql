
CREATE PROCEDURE AgregarSubcategoria
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 10/12/25
 Procedimiento : AgregarSubcategoria
 Descripción   : Inserta una subcategoría validando existencia de categoría padre y duplicados.
========================================================================
*/
    @Nombre NVARCHAR(100),
    @Detalles NVARCHAR(MAX) = NULL,
    @id_categoria INT
AS
BEGIN
    SET NOCOUNT ON;

    --Que la categoría padre exista
    IF NOT EXISTS (SELECT 1 FROM Categoria WHERE id_categoria = @id_categoria)
    BEGIN
        RAISERROR('El ID de categoría indicado no existe.', 16, 1);
        RETURN;
    END

    --Evitar duplicados dentro de la misma categoría
    IF EXISTS (SELECT 1 FROM Subcategoria WHERE Nombre = @Nombre AND id_categoria = @id_categoria)
    BEGIN
        RAISERROR('La subcategoría ya existe en esta categoría.', 16, 1);
        RETURN;
    END

    INSERT INTO Subcategoria (Nombre, Detalles, id_categoria)
    VALUES (@Nombre, @Detalles, @id_categoria);
END
