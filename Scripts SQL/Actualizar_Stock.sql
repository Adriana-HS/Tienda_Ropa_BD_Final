CREATE PROCEDURE Actualizar_Stock
/*
=======================================================================
 Nombre        : Karen Landivar
 Fecha         : 11/12/25
 Procedimiento : Actualizar_Stock
 Descripción   : Suma o descuenta stock del inventario.
========================================================================
*/
    @id_producto INT,
    @CantidadAjuste INT  --Puede ser positivo o negativo
AS
BEGIN
     SET NOCOUNT ON
    -- Validar que el producto exista
    IF NOT EXISTS (SELECT 1 FROM Producto WHERE id_producto = @id_producto)
    BEGIN
        RAISERROR('El producto no existe.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Inventario WHERE id_producto = @id_producto) 
    /* Condición para saber si se tiene que crear o actualizar. 
    Si el select muestra un resultado, se actualiza (porque ya hay datos). 
    Sino, se crea un nuevo registro con el producto
    */
        BEGIN
        DECLARE @StockActual INT;
        DECLARE @NuevoStock INT;

        SELECT @StockActual = Cantidad 
        FROM Inventario 
        WHERE id_producto = @id_producto;

        SET @NuevoStock = @StockActual + @CantidadAjuste;

        -- Verificar que no quede negativo
        IF @NuevoStock < 0
        BEGIN
            RAISERROR('El stock no puede quedar negativo.', 16, 1);
            RETURN;
        END

        UPDATE Inventario
        SET Cantidad = @NuevoStock
        WHERE id_producto = @id_producto;
    END

    ELSE
    BEGIN
        -- Si no hay inventario previo, solo se permite crear stock ≥ 0
        IF @CantidadAjuste < 0
        BEGIN
            RAISERROR('No se puede crear un inventario con stock negativo.', 16, 1);
            RETURN;
        END

        INSERT INTO Inventario (Cantidad, id_producto)
        VALUES (@CantidadAjuste, @id_producto);
    END
END;
