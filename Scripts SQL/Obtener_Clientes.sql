CREATE PROCEDURE Obtener_Clientes
/*
======================================================================
 Nombre        : Karen Landivar
 Fecha         : 10/12/25
 Procedimiento : Obtener_Clientes
 Descripción   : Devuelve todos los clientes o filtra por nombre/apellido.
========================================================================
*/
    @Filtro NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Eliminamos espacios accidentales
    SET @Filtro = TRIM(@Filtro);

    --Si el filtro está vacío, mostramos todos de inmediato
    IF @Filtro IS NULL OR @Filtro = ''
    BEGIN
        SELECT * FROM Cliente;
    END
    ELSE
    BEGIN
        --Buscamos coincidencias rápidas
        SELECT * FROM Cliente
        WHERE Nombre LIKE '%' + @Filtro + '%'
           OR Apellido LIKE '%' + @Filtro + '%'
    END
END
