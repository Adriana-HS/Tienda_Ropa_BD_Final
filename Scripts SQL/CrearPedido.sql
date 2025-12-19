
CREATE PROCEDURE CrearPedido
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 10/12/25
 Procedimiento : CrearPedido
 Descripción   : Inicializa un nuevo pedido para un cliente con estado Pendiente.
==========================================================================
*/
    @id_cliente INT,
	@id_pedido_necesario INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO Pedido ( fecha, estado, total, id_cliente)
    VALUES (GETDATE(), 'P', 0, @id_cliente);

    SET @id_pedido_necesario = SCOPE_IDENTITY();
END