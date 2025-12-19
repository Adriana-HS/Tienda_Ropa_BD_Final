CREATE PROCEDURE Editar_Precio_Producto
/*
=======================================================================
 Nombre        : Karen Landivar
 Fecha         : 11/12/25
 Procedimiento : Editar_Precio_Producto
 Descripción   : Modifica los precios de un producto existente.
========================================================================
*/
    @id_producto  INT,
    @Precio_base  DECIMAL(10,2),
    @Precio_venta DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON
    IF NOT EXISTS (SELECT 1 FROM Producto WHERE id_producto = @id_producto)
    BEGIN
        RAISERROR('El producto no existe.', 16, 1);
        RETURN;
    END

    IF @Precio_base < 0 OR @Precio_venta < 0
    BEGIN
        RAISERROR('Los precios no pueden ser negativos.', 16, 1);
        RETURN;
    END

    UPDATE Producto
    SET Precio_base  = @Precio_base,
        Precio_venta = @Precio_venta
    WHERE id_producto = @id_producto;
END;
