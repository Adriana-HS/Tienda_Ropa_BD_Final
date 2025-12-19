
CREATE PROCEDURE ObtenerFactura
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 10/12/25
 Procedimiento : ObtenerFactura
 Descripción   : Obtiene los datos de cabecera y lista de productos para la factura.
==========================================================================
*/
    @id_pedido INT
AS
BEGIN
    SET NOCOUNT ON;

    --MAIN
    SELECT 
        P.id_pedido,
        P.fecha,
        P.total,
        C.Nombre + ' ' + C.Apellido AS Cliente_Nombre,
        C.Telefono,
        C.Email,
        C.id_cliente
    FROM Pedido P
    INNER JOIN Cliente C ON P.id_cliente = C.id_cliente
    WHERE P.id_pedido = @id_pedido;

    --Lista de Productos
    SELECT 
        Prod.Nombre AS Producto,
        Prod.Color, 
        Prod.Talla, 
        DP.cantidad,
        DP.precio_unitario_final AS Precio_Unitario,
        (DP.cantidad * DP.precio_unitario_final) AS Subtotal,
        DP.descuento_aplicado AS Ahorro_Por_Unidad,
        DP.id_detalle, --Útil para el mapeo en C#
        DP.id_producto
    FROM Detalle_Pedido DP
    INNER JOIN Producto Prod ON DP.id_producto = Prod.id_producto
    WHERE DP.id_pedido = @id_pedido;
END
