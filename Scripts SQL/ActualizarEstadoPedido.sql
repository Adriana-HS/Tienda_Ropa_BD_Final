

CREATE PROCEDURE ActualizarEstadoPedido
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 10/12/25
 Procedimiento : ActualizarEstadoPedido
 Descripción   : Actualiza el estado de un pedido (P=Pendiente, C=Confirmado, E=Enviado).
========================================================================
*/
    @id_pedido INT,
    @nuevo_estado CHAR(1) -- P, C, E
AS
BEGIN
    SET NOCOUNT ON;

    IF @nuevo_estado NOT IN ('P', 'C', 'E')
    BEGIN
        RAISERROR('Estado inválido. Use P, C o E', 16, 1);
        RETURN;
    END

    UPDATE Pedido
    SET estado = @nuevo_estado
    WHERE id_pedido = @id_pedido;
END