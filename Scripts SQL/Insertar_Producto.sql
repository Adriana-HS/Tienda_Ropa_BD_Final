CREATE PROCEDURE Insertar_Producto
/*
=======================================================================
 Nombre        : Karen Landivar
 Fecha         : 10/12/25
 Procedimiento : Insertar_Producto
 Descripción   : Crea un nuevo producto en el catálogo.
========================================================================
*/
    @Nombre        NVARCHAR(150),
    @Descripcion   NVARCHAR(MAX) = NULL,
    @Precio_base   DECIMAL(10,2),
    @Precio_venta  DECIMAL(10,2),
    @Color         NVARCHAR(50),
    @Talla         NVARCHAR(20),
    @id_subcategoria INT
AS
BEGIN
    SET NOCOUNT ON
    INSERT INTO Producto (Nombre, Descripcion, Precio_base, Precio_venta,
                          Color, Talla, id_subcategoria)
    VALUES (@Nombre, @Descripcion, @Precio_base, @Precio_venta,
            @Color, @Talla, @id_subcategoria);
END;
