CREATE PROCEDURE Obtener_Productos
/*
=======================================================================
 Nombre         : Karen Landivar
 Fecha         :10/12/25
 Procedimiento : Obtener_Productos
 Descripción   : Devuelve el catálogo con Stock, Subcategoría y Categoría.
========================================================================
*/
AS
BEGIN
    SET NOCOUNT ON;
    SELECT P.*, I.Cantidad AS Stock, S.Nombre AS Subcategoria, C.Nombre AS Categoria
    FROM Producto P
    LEFT JOIN Inventario I ON P.id_producto = I.id_producto
    INNER JOIN Subcategoria S ON P.id_subcategoria = S.id_subcategoria
    INNER JOIN Categoria C ON S.id_categoria = C.id_categoria;
END