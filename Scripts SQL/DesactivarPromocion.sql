
CREATE PROCEDURE Desactivar_Promocion
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 9/12/25
 Procedimiento : Desactivar_Promocion
 Descripción   : Desactiva una promoción cambiando su fecha fin al día anterior.
========================================================================
*/
    @id_promo INT
AS
BEGIN
    UPDATE Promocion
    SET fecha_fin = DATEADD(DAY, -1, GETDATE()) 
    WHERE id_promocion = @id_promo;
END
