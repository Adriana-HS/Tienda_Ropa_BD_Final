CREATE PROCEDURE Editar_Cliente
/*
=======================================================================
 Nombre        : Karen Landivar
 Fecha         : 10/12/25
 Procedimiento : Editar_Cliente
 Descripción   : Actualiza uno o varios campos del cliente.
 Parámetros    :
    @id_cliente -> ID del cliente a modificar (obligatorio)
    @Nombre     -> Nuevo nombre (opcional)
    @Apellido   -> Nuevo apellido (opcional)
    @Email      -> Nuevo email (opcional)
    @Telefono   -> Nuevo teléfono (opcional)
========================================================================*/
    @id_cliente INT,
    @Nombre     NVARCHAR(100) = NULL,
    @Apellido   NVARCHAR(100) = NULL,
    @Email      NVARCHAR(150) = NULL,
    @Telefono   NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON
    IF NOT EXISTS (SELECT 1 FROM Cliente WHERE id_cliente = @id_cliente)
    BEGIN
        RAISERROR('El cliente no existe.', 16, 1);
        RETURN;
    END
    UPDATE Cliente
    SET 
        Nombre   = COALESCE(@Nombre, Nombre),
        Apellido = COALESCE(@Apellido, Apellido),
        Email    = COALESCE(@Email, Email),
        Telefono = COALESCE(@Telefono, Telefono)
    WHERE id_cliente = @id_cliente;
END;
