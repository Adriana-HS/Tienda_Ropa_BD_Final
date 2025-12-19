CREATE PROCEDURE Insertar_Cliente
/*
=======================================================================
 Nombre        : Karen Landivar
 Fecha         : 10/12/25
 Procedimiento : Insertar_Cliente
 Descripción   : Registra un nuevo cliente en el sistema.
========================================================================
*/
    @Nombre   NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Email    NVARCHAR(150) = NULL,
    @Telefono NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON
    --Verificación para no poner Emails repetidos
    IF @Email IS NOT NULL AND EXISTS(SELECT 1 FROM Cliente WHERE Email = @Email)
    BEGIN
        RAISERROR('Ya existe un cliente con este Email.', 16, 1);
        RETURN;
    END
    INSERT INTO Cliente (Nombre, Apellido, Email, Telefono)
    VALUES (@Nombre, @Apellido, @Email, @Telefono);
END;
