

CREATE PROCEDURE AsignarPromocion
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 9/12/25
 Procedimiento : AsignarPromocion
 Descripción   : Asigna una promoción existente a un Producto, Categoría o Subcategoría.
========================================================================
*/
	@id_producto INT = NULL,
    @id_categoria INT = NULL,
    @id_subcategoria INT = NULL,
	@id_promo INT,
    @monto_min_compra DECIMAL(8,2)
AS
BEGIN
    SET NOCOUNT ON;
    -- Aunque el CHECK de la tabla lo protege, uno nunca sabe
    DECLARE @contador INT = 0;
    IF @id_producto IS NOT NULL SET @contador = @contador + 1; --como es una linea lo pongo todo junto
    IF @id_categoria IS NOT NULL SET @contador = @contador + 1;
    IF @id_subcategoria IS NOT NULL SET @contador = @contador + 1;

    IF @contador <> 1
    BEGIN
        RAISERROR('Debes poner SOLO UNO: Producto, Categoría o Subcategoría.', 16, 1);
        RETURN;
    END

    INSERT INTO Aplicacion_Promo (
        id_promocion, monto_min, id_producto, 
        id_categoria, id_subcategoria)
    VALUES (
        @id_promo, @monto_min_compra, @id_producto, 
        @id_categoria, @id_subcategoria);
END