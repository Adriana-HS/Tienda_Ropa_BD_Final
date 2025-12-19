
CREATE PROCEDURE ObtenerHistorialCliente
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 10/12/25
 Procedimiento : ObtenerHistorialCliente
 Descripción   : Obtiene el historial de pedidos de un cliente ordenado por fecha.
==========================================================================
*/
    @id_cliente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        id_pedido,
        fecha,
        CASE estado
            WHEN 'P' THEN 'Pendiente'
            WHEN 'C' THEN 'Confirmado/Pagado'
            WHEN 'E' THEN 'Enviado'
            ELSE estado
        END EstadoPedido,
        total
    FROM Pedido
    WHERE id_cliente = @id_cliente
    ORDER BY fecha DESC;
END
