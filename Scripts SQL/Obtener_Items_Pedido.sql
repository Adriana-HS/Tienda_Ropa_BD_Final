
CREATE PROCEDURE Obtener_Items_Pedido
/*
======================================================================
 Nombre        : Karen Landivar
 Fecha         : 16/12/25
 Procedimiento : Obtener_Items_Pedido
 Descripción   : Devuelve todo el detalle del producto, necesario para mostrar en el carrito.
========================================================================
*/
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
        P.Nombre, P.Color, P.Talla,
        (DP.cantidad * DP.precio_unitario_final) AS subtotal
    FROM Detalle_Pedido DP
    INNER JOIN Producto P ON DP.id_producto = P.id_producto
    WHERE DP.id_pedido = @id_pedido;
END