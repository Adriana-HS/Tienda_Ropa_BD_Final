
CREATE PROCEDURE EliminarProductoPedido
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 10/12/25
 Procedimiento : EliminarProductoPedido
 Descripción   : Elimina un ítem de un pedido, restaurando stock y actualizando el total.
==========================================================================
*/
    @id_detalle INT --Solo el ID dado que borramos solo esa FILA
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @id_pedido INT;
    DECLARE @id_producto INT;
    DECLARE @cantidad_devolver INT;
    DECLARE @monto_restar DECIMAL(10,2);
    DECLARE @estado_pedido CHAR(1);

    --Necesitamos saber cuánto costaba y cuántos eran para devolverlos
    SELECT 
        @id_pedido = id_pedido,
        @id_producto = id_producto,
        @cantidad_devolver = cantidad,
        @monto_restar = (cantidad * precio_unitario_final) -- Calculamos cuánto valía esa línea
    FROM Detalle_Pedido
    WHERE id_detalle = @id_detalle;

    IF @id_pedido IS NULL
    BEGIN
        RAISERROR('El item que intentas borrar no existe.', 16, 1);
        RETURN;
    END

    SELECT @estado_pedido = estado 
    FROM Pedido 
    WHERE id_pedido = @id_pedido;
    
    IF @estado_pedido <> 'P'
    BEGIN
        RAISERROR('No se puede eliminar items de un pedido que ya fue Pagado o Enviado.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        
        --Devolver stock
        UPDATE Inventario
        SET Cantidad = Cantidad + @cantidad_devolver
        WHERE id_producto = @id_producto;

        --Restar monto del MAIN pedido
        UPDATE Pedido
        SET total = total - @monto_restar
        WHERE id_pedido = @id_pedido;

        --Raro, pero nunca imposible, para que no se disparate
        UPDATE Pedido 
        SET total = 0 
        WHERE id_pedido = @id_pedido AND total < 0;

        --NOW borrar detalle
        DELETE FROM Detalle_Pedido
        WHERE id_detalle = @id_detalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMsg, 16, 1);
    END CATCH
END
