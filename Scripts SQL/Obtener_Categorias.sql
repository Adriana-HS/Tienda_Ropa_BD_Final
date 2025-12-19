
CREATE PROCEDURE Obtener_Categorias
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 15/12/25
 Procedimiento : Obtener_Categorias
 Descripción   : Devuelve la lista de categorías ordenadas alfabéticamente.
========================================================================
*/
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT id_categoria, Nombre, Detalles
    FROM Categoria
    ORDER BY Nombre ASC;
END
