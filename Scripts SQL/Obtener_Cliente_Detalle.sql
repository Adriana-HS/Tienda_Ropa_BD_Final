CREATE PROCEDURE Obtener_Cliente_Detalle
/*
=======================================================================
 Nombre        : Karen Landivar
 Fecha         : 10/12/25
 Procedimiento : Obtener_Cliente_Detalle
 Descripción   : Devuelve un cliente específico buscado por ID o Email.
========================================================================
*/
    @id_cliente INT = NULL,
    @Email      NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    --Debe enviar al menos uno
    IF @id_cliente IS NULL AND @Email IS NULL
    BEGIN
        RAISERROR('Debe proporcionar ID o Email.', 16, 1);
        RETURN;
    END
    --Si envía ambos, prioriza ID
    IF @id_cliente IS NOT NULL
    BEGIN
        SELECT *
        FROM Cliente
        WHERE id_cliente = @id_cliente;
        RETURN;
    END

    --Buscar por Email (se ejecuta sólo si no se ingresó ID)
    SELECT *
    FROM Cliente
    WHERE Email = @Email;
END;
