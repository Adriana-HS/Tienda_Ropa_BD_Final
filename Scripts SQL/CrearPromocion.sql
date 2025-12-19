
CREATE PROCEDURE CrearPromocion
/* =======================================================================
 Nombre        : Adriana Hernández
 Fecha         : 9/12/25
 Procedimiento : CrearPromocion
 Descripción   : Registra una nueva promoción validando reglas de porcentaje o monto.
==========================================================================
*/
	@Nombre NVARCHAR(40), 
	@Descripcion NVARCHAR(100) = NULL, 
	@Porcentaje DECIMAL(3,2) = NULL, 
	@Monto DECIMAL(8,2) = NULL, 
	@FechaInicio DATE, 
	@FechaFin DATE,
	@id_promo_necesario INT OUTPUT--Esto para que devuelva algo, para llamarlo en Aplicaion_Promo
AS
BEGIN
	SET NOCOUNT ON;
	
    IF (@Porcentaje IS NOT NULL AND @Monto IS NOT NULL)
    BEGIN
        RAISERROR('No puedes definir Porcentaje y Monto al mismo tiempo.', 16, 1);
        RETURN;
    END
	--Al menos uno debe existir
    IF (@Porcentaje IS NULL AND @Monto IS NULL)
    BEGIN
        RAISERROR('Debes definir al menos un Porcentaje o un Monto de descuento.', 16, 1);
        RETURN;
    END

    INSERT INTO Promocion (nombre, descripcion, porcentaje, monto, fecha_inicio, fecha_fin)
    VALUES (@Nombre, @Descripcion, @Porcentaje, @Monto, @FechaInicio, @FechaFin);

    SET @id_promo_necesario = SCOPE_IDENTITY();--Este SCOPE me devuelve el ID que se genero en esta consulta
END