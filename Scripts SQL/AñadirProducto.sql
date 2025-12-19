
CREATE PROCEDURE AgregarProducto
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 10/12/25
 Procedimiento : AgregarProducto
 Descripción   : Agrega un producto al pedido gestionando transacción, stock y descuentos.
===========================================================================
*/
	@id_pedido INT,
	@id_producto INT,
	@cantidad INT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @precio_unitario_final DECIMAL(8,2),
	@descuento_aplicado DECIMAL(8,2),
	@precio_base DECIMAL(8,2),
	@stock INT,
    @id_aplicacion INT;

	--Todo o Nada
    BEGIN TRANSACTION;
    BEGIN TRY
        SELECT @stock = Cantidad 
        FROM Inventario 
        WHERE id_producto = @id_producto;

        IF @stock IS NULL OR @stock < @cantidad
        BEGIN
            RAISERROR('No hay suficiente stock para este producto.', 16, 1);
        END

		--Obtenemos el precio final con descuento
        SET @precio_unitario_final = dbo.CalcularPrecioFinalProduct(@id_producto);
        
        SELECT @precio_base = Precio_venta 
        FROM Producto 
        WHERE id_producto = @id_producto;
        
        --Calculamos el descuento monetario (Base - Final)
        SET @descuento_aplicado = @precio_base - @precio_unitario_final;

        IF @descuento_aplicado > 0
        BEGIN
             SELECT TOP 1 @id_aplicacion = A.id_aplicacion
             FROM Aplicacion_Promo A
             INNER JOIN Promocion P 
             ON A.id_promocion = P.id_promocion
             INNER JOIN Producto Prod 
             ON Prod.id_producto = @id_producto
             INNER JOIN Subcategoria Sub 
             ON Prod.id_subcategoria = Sub.id_subcategoria
             WHERE GETDATE() BETWEEN P.fecha_inicio AND P.fecha_fin
             AND (
                (A.id_producto = @id_producto) OR 
                (A.id_subcategoria = Sub.id_subcategoria) OR 
                (A.id_categoria = Sub.id_categoria))
             ORDER BY 
                CASE 
                    WHEN A.id_producto IS NOT NULL THEN 1
                    WHEN A.id_subcategoria IS NOT NULL THEN 2
                    WHEN A.id_categoria IS NOT NULL THEN 3
                END ASC;
        END;

        INSERT INTO Detalle_Pedido(
            id_pedido, id_producto, cantidad, 
            precio_unitario_final, descuento_aplicado, id_aplicacion)
        VALUES (
            @id_pedido, @id_producto, @cantidad, 
            @precio_unitario_final, @descuento_aplicado, @id_aplicacion);

        --Restar inventario
        UPDATE Inventario
        SET Cantidad = Cantidad - @cantidad
        WHERE id_producto = @id_producto;

        --Sumar al total gral
        UPDATE Pedido
        SET total = total + (@precio_unitario_final * @cantidad)
        WHERE id_pedido = @id_pedido;

        -- Si llegamos aquí, todo salió bien. So confirmar cambios, para eso de 'TRANSACTION'
        COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
    --Si hace ROLLBACK deshace todo; lo deja como antes, en caso de que falle algo
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;--2@ porque es varible global del sistema

        --Reenviamos el error a la aplicación
        DECLARE @MensajeError NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@MensajeError, 16, 1);
	END CATCH
END