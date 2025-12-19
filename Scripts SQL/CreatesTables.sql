USE Tienda_Ropa

CREATE TABLE Categoria (
    id_categoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Detalles NVARCHAR(MAX)
);
CREATE TABLE Subcategoria (
    id_subcategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Detalles NVARCHAR(MAX),
    id_categoria INT NOT NULL,
    FOREIGN KEY (id_categoria) REFERENCES Categoria(id_categoria)
);

CREATE TABLE Cliente (
    id_cliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) UNIQUE,
    Telefono NVARCHAR(20)
	FechaRegistro DATETIME DEFAULT GETDATE()
);

CREATE TABLE Producto (
    id_producto INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX),
    Precio_base DECIMAL(10,2) NOT NULL, --Precio al que la tienda consiguió el producto
    Precio_venta DECIMAL(10,2) NOT NULL, --Precio en el que se vende el producto
    CHECK (Precio_base >= 0 AND Precio_venta >= 0),
    Color NVARCHAR(50) NOT NULL,
    Talla NVARCHAR(20) NOT NULL,  
    id_subcategoria INT NOT NULL,
    FOREIGN KEY (id_subcategoria) REFERENCES Subcategoria(id_subcategoria)
);

CREATE TABLE Inventario (
    id_inventario INT IDENTITY(1,1) PRIMARY KEY,
    Cantidad INT NOT NULL CHECK (Cantidad >= 0),
    id_producto INT NOT NULL,
    FOREIGN KEY (id_producto) REFERENCES Producto(id_producto)
);

CREATE TABLE Pedido(
	id_pedido INT PRIMARY KEY IDENTITY(1,1),
	fecha DATETIME DEFAULT GETDATE(),--datatime por que quiero la hora
	estado NVARCHAR(1) DEFAULT 'P', --P: Pendiente de Pago. C: Confirmado / Pagado. E: Entregado
	total DECIMAL(10,2) DEFAULT 0,
	id_cliente INT NOT NULL,
	FOREIGN KEY (id_cliente) REFERENCES Cliente(id_cliente)
);

CREATE TABLE Promocion(
	id_promocion INT IDENTITY(1,1) PRIMARY KEY,
	nombre NVARCHAR(40) NOT NULL,
	descripcion NVARCHAR(100),
	porcentaje DECIMAL(3, 2) CHECK (porcentaje > 0 AND porcentaje <= 1),
	monto DECIMAL(8, 2) CHECK (monto > 0),
	fecha_inicio DATE NOT NULL,
	fecha_fin DATE NOT NULL,
	CHECK (fecha_fin >= fecha_inicio),
	CHECK (
        (porcentaje IS NOT NULL AND monto IS NULL) 
        OR (porcentaje IS NULL AND monto IS NOT NULL))
);

CREATE TABLE Aplicacion_Promo(
	id_aplicacion INT IDENTITY(1,1) PRIMARY KEY,
	monto_min DECIMAL(8,2) DEFAULT 0 CHECK(monto_min >= 0),
	id_promocion INT NOT NULL,
	id_producto INT,
	id_categoria INT,
	id_subcategoria INT,
	FOREIGN KEY (id_promocion) REFERENCES Promocion(id_promocion),
	FOREIGN KEY (id_producto) REFERENCES Producto(id_producto),
	FOREIGN KEY (id_categoria) REFERENCES Categoria(id_categoria),
	FOREIGN KEY (id_subcategoria) REFERENCES Subcategoria(id_subcategoria),
	CHECK (--La promo solo es aplicable a una cosa a la vez
        (id_producto IS NOT NULL AND id_categoria IS NULL AND id_subcategoria IS NULL)
        OR
        (id_producto IS NULL AND id_categoria IS NOT NULL AND id_subcategoria IS NULL)
        OR
        (id_producto IS NULL AND id_categoria IS NULL AND id_subcategoria IS NOT NULL))
);

CREATE TABLE Detalle_Pedido(
	id_detalle INT IDENTITY(1,1) PRIMARY KEY,
	cantidad INT DEFAULT 1 CHECK(cantidad > 0),
	precio_unitario_final DECIMAL(8, 2) NOT NULL, -- Precio ya con descuento
    descuento_aplicado DECIMAL(8, 2) DEFAULT 0,
	id_pedido INT NOT NULL,
	id_producto INT NOT NULL,
	id_aplicacion INT,--Puede ser nulo dado que no todos tienen descuento
	FOREIGN KEY (id_pedido) REFERENCES Pedido(id_pedido),
	FOREIGN KEY (id_producto) REFERENCES Producto(id_producto),
	FOREIGN KEY (id_aplicacion) REFERENCES Aplicacion_Promo(id_aplicacion)
);
