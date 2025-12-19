CREATE OR ALTER PROCEDURE Obtener_Items_Pedido
    @id_pedido INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        DP.id_detalle,
        DP.id_producto,
        DP.cantidad,
        DP.precio_unitario_final,
        DP.descuento_aplicado,
        P.Nombre,
        (DP.cantidad * DP.precio_unitario_final) AS subtotal
    FROM Detalle_Pedido DP
    INNER JOIN Producto P ON DP.id_producto = P.id_producto
    WHERE DP.id_pedido = @id_pedido;
END
GO

CREATE OR ALTER PROCEDURE Obtener_Subcategorias
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id_subcategoria,
        Nombre,
        Detalles,
        id_categoria
    FROM Subcategoria
    ORDER BY Nombre ASC;
END
GO

CREATE OR ALTER PROCEDURE Obtener_Promociones_Activas
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
GO
