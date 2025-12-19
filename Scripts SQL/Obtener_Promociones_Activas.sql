
CREATE PROCEDURE Obtener_Promociones_Activas
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 15/12/25
 Procedimiento : Obtener_Promociones_Activas
 Descripción   : Devuelve la lista de promociones vigentes a la fecha actual.
========================================================================
*/
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id_promocion,
        nombre
    FROM Promocion
    WHERE GETDATE() BETWEEN fecha_inicio AND fecha_fin
    ORDER BY nombre ASC;
END