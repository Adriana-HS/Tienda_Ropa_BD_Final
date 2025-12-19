

CREATE PROCEDURE EliminarPedido
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 10/12/25
 Procedimiento : EliminarPedido
 Descripción   : Elimina un pedido pendiente, devolviendo el stock de los productos.
==========================================================================
*/
    @id_pedido INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Pedido WHERE id_pedido = @id_pedido AND estado = 'P')
    BEGIN
        RAISERROR('El pedido no existe o ya no se puede eliminar (ya fue pagado o enviado).', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY

        UPDATE I
        SET I.Cantidad = I.Cantidad + D.cantidad
        FROM Inventario I
        INNER JOIN Detalle_Pedido D 
        ON I.id_producto = D.id_producto
        WHERE D.id_pedido = @id_pedido;

        DELETE FROM Detalle_Pedido 
        WHERE id_pedido = @id_pedido;

        DELETE FROM Pedido 
        WHERE id_pedido = @id_pedido;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMsg, 16, 1);
    END CATCH
END
