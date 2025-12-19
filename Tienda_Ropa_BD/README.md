# Sistema POS - Tienda de Ropa

Aplicación de escritorio WPF para gestión de punto de venta (POS) conectada a SQL Server.

## 🚀 Características

### 1. Módulo de Clientes
- Búsqueda de clientes por nombre/apellido
- Búsqueda detallada por ID o Email
- Crear nuevo cliente
- Editar cliente existente
- Validación de email duplicado

### 2. Módulo de Inventario
- Visualización de catálogo completo con stock actual
- Edición de precios (base y venta)
- Ajuste de stock (entrada/salida)
- Ver detalle completo del producto

### 3. Módulo de Ventas (POS)
- Selección de cliente
- Creación de pedido
- Agregar productos al carrito
- Cálculo automático de descuentos y totales (desde SQL)
- Validación de stock en tiempo real
- Eliminar items del carrito
- Finalizar venta y generar factura
- Reimpresión de facturas

### 4. Módulo de Historial
- Selección de cliente
- Visualización de historial de compras
- Estados de pedido traducidos (Pendiente, Confirmado, Enviado)
- Ver/Reimprimir facturas históricas

### 5. Módulo de Promociones
- Crear promociones (Porcentaje o Monto)
- Asignar promociones a productos, categorías o subcategorías
- Desactivar promociones

## 📋 Requisitos

- .NET 8.0 SDK
- SQL Server (local o remoto)
- Visual Studio 2022 o superior (recomendado)

## ⚙️ Configuración

### Base de Datos

La aplicación está configurada para conectarse a:
- **Servidor**: localhost
- **Base de Datos**: Tienda_Ropa
- **Usuario**: Cajera
- **Contraseña**: 123456

Para cambiar la configuración, edita el archivo `Config/ConnectionConfig.cs`.

### Stored Procedures Requeridos

La aplicación utiliza los siguientes Stored Procedures (deben existir en la base de datos):

**Clientes:**
- `Obtener_Clientes` (@Filtro)
- `Obtener_Cliente_Detalle` (@IdCliente)
- `Insertar_Cliente` (@Nombre, @Apellido, @Email, @Telefono, @IdCliente OUTPUT)
- `Editar_Cliente` (@IdCliente, @Nombre, @Apellido, @Email, @Telefono)

**Inventario:**
- `Obtener_Productos`
- `Obtener_Producto_Info` (@IdProducto)
- `Editar_Precio_Producto` (@IdProducto, @PrecioBase, @PrecioVenta)
- `Actualizar_Stock` (@IdProducto, @Cantidad, @TipoOperacion)

**Ventas:**
- `CrearPedido` (@IdCliente, @IdPedido OUTPUT)
- `Agregar_Producto` (@IdPedido, @IdProducto, @Cantidad)
- `sp_Obtener_Factura` (@IdPedido) - Devuelve 2 Result Sets
- `Eliminar_Producto_Pedido` (@IdItem)
- `ActualizarEstadoPedido` (@IdPedido, @Estado)

**Historial:**
- `ObtenerHistorialCliente` (@IdCliente) - Devuelve columna "Estado Pedido" traducida

**Promociones:**
- `CrearPromocion` (@Nombre, @Descripcion, @TipoDescuento, @ValorPorcentaje, @ValorMonto, @FechaInicio, @FechaFin, @IdPromocion OUTPUT)
- `AsignarPromocion` (@IdPromocion, @IdProducto, @IdCategoria, @IdSubcategoria)
- `Desactivar_Promocion` (@IdPromocion)

## 🏗️ Estructura del Proyecto

```
TiendaRopaPOS/
├── Config/
│   └── ConnectionConfig.cs          # Configuración de conexión
├── Data/
│   └── DatabaseAccess.cs            # Clase de acceso a datos
├── Models/
│   ├── Cliente.cs
│   ├── Producto.cs
│   ├── Pedido.cs
│   └── Promocion.cs
├── Services/
│   ├── ClienteService.cs
│   ├── InventarioService.cs
│   ├── VentaService.cs
│   ├── HistorialService.cs
│   └── PromocionService.cs
├── Views/
│   ├── ClientesView.xaml
│   ├── InventarioView.xaml
│   ├── VentasView.xaml
│   ├── HistorialView.xaml
│   ├── PromocionesView.xaml
│   └── [Diálogos varios]
├── MainWindow.xaml                  # Ventana principal
└── Styles.xaml                      # Estilos globales
```

## 🎯 Uso

1. **Compilar el proyecto:**
   ```bash
   dotnet build
   ```

2. **Ejecutar la aplicación:**
   ```bash
   dotnet run
   ```
   O desde Visual Studio: F5

3. **Navegación:**
   - Usa los botones del menú lateral para cambiar entre módulos
   - Cada módulo tiene su propia funcionalidad independiente

## ⚠️ Notas Importantes

- **Toda la lógica de negocio está en SQL**: Stock, precios, descuentos se calculan en los Stored Procedures
- **Manejo de errores**: Los errores de SQL (RAISERROR) se capturan y muestran al usuario
- **Sin autenticación**: Aplicación de uso local sin sistema de login
- **Validaciones**: El frontend valida datos básicos, pero SQL tiene la última palabra

## 🔧 Solución de Problemas

### Error de conexión
- Verifica que SQL Server esté ejecutándose
- Confirma las credenciales en `ConnectionConfig.cs`
- Asegúrate de que la base de datos existe

### Stored Procedure no encontrado
- Verifica que todos los SPs estén creados en la base de datos
- Revisa los nombres exactos (case-sensitive en algunos casos)

### Error "Stock insuficiente"
- Este error viene directamente de SQL
- El SP `Agregar_Producto` valida el stock antes de agregar

## 📝 Licencia

Este proyecto es de uso interno.

