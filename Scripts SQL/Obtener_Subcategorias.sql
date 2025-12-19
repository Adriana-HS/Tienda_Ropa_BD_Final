
CREATE PROCEDURE Obtener_Subcategorias
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 15/12/25
 Procedimiento : Obtener_Subcategorias
 Descripción   : Devuelve la lista de subcategorías con sus detalles y ID padre.
===========================================================================
*/
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id_subcategoria,
        Nombre,
        Detalles,
        id_categoria
    FROM Subcategoria
    ORDER BY Nombre ASC;
END