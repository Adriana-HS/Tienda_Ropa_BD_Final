
CREATE FUNCTION CalcularPrecioFinalProduct(@id_producto INT)
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 10/12/25
 Procedimiento : CalcularPrecioFinalProduct
 Descripción   : Función que calcula el precio final aplicando la mejor promoción disponible.
========================================================================
*/
RETURNS DECIMAL(8,2)
AS
BEGIN
	DECLARE @PrecioBase DECIMAL(8,2); 
	DECLARE @PrecioFinal DECIMAL(8,2);
	DECLARE @Porcentaje DECIMAL(3,2);
	DECLARE @Monto DECIMAL(8,2);
	DECLARE @id_categoria INT;
	DECLARE @id_subcategoria INT
	
	--Conseguir valores
	SELECT 
	@PrecioBase = Precio_venta,
	@id_subcategoria = id_subcategoria
	FROM Producto PD
	WHERE PD.id_producto = @id_producto;

	SELECT
	@id_categoria = id_categoria
	FROM Subcategoria S
	WHERE S.id_subcategoria = @id_subcategoria;

	--Conseguimos los descuentos, con TOP 1 porque por ahi hay más de una
	SELECT TOP 1
	@Porcentaje = porcentaje,
	@Monto = monto
	FROM Promocion P
	INNER JOIN Aplicacion_Promo A
	ON A.id_promocion = P.id_promocion
	WHERE GETDATE() BETWEEN fecha_inicio AND fecha_fin
	AND (
		(A.id_producto = @id_producto) OR --1
		(A.id_subcategoria = @id_subcategoria) OR --2
		(A.id_categoria = @id_categoria))-- 3 en eleccion
	ORDER BY --Para la prioridad
		CASE WHEN A.id_producto IS NOT NULL THEN 1
		WHEN A.id_subcategoria IS NOT NULL THEN 2
		WHEN A.id_categoria IS NOT NULL THEN 3
		END ASC;
	--Asignamos el precio final
	SET @PrecioFinal = @PrecioBase;--Por si no hay descuento

	--Por si hay descuento
	IF @Porcentaje IS NOT NULL 
	BEGIN
		SET @PrecioFinal = @PrecioBase - (@PrecioBase * @Porcentaje);
	END
	ELSE IF @Monto IS NOT NULL
	BEGIN
		SET @PrecioFinal = @PrecioBase - @Monto;
	END
	
	--Por si acaso sale negativo T_T
	IF @PrecioFinal < 0 
	BEGIN
		SET @PrecioFinal = 0;
	END

	RETURN @PrecioFinal
END