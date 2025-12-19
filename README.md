# Sistema POS - Tienda de Ropa

Sistema de Punto de Venta completo para gestión de tienda de ropa, desarrollado en C# con Windows Forms y SQL Server. Incluye gestión de inventario, clientes, ventas y sistema de promociones automáticas.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Uso](#-uso)
- [Base de Datos](#-base-de-datos)
- [Solución de Problemas](#-solución-de-problemas)
- [Autores](#-autores)

## ✨ Características

### Módulo de Clientes
- Registro de clientes con validación de email único
- Búsqueda y filtrado por nombre/apellido
- Edición de información de contacto
- Historial de compras por cliente

### Módulo de Inventario
- Catálogo completo de productos con atributos (color, talla)
- Gestión de stock con control de entradas/salidas
- Categorización jerárquica (Categoría > Subcategoría)
- Edición de precios (base y venta)
- Control de stock en tiempo real

### Módulo de Ventas (POS)
- Interfaz intuitiva de punto de venta
- Carrito de compras con gestión de items
- Aplicación automática de descuentos y promociones
- Cálculo automático de totales
- Gestión de estados de pedido (Pendiente/Confirmado/Enviado)
- Generación e impresión de facturas

### Sistema de Promociones
- Descuentos por porcentaje o monto fijo
- Aplicación a productos específicos, categorías o subcategorías
- Sistema de prioridad (Producto > Subcategoría > Categoría)
- Configuración de fechas de vigencia
- Montos mínimos de compra
- Activación/desactivación de promociones

### Reportes
- Historial de ventas por cliente
- Facturas detalladas con descuentos aplicados
- Consulta de inventario actual

## 🔧 Requisitos Previos

### Software Necesario

- **Sistema Operativo:** Windows 10 o superior
- **.NET SDK:** 6.0 o superior
  - [Descargar .NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- **Base de Datos:** SQL Server 2019 o superior / Azure SQL Database
  - [Descargar SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) (gratuito)
- **IDE (Opcional pero recomendado):**
  - Visual Studio 2022 Community (gratuito)
  - Visual Studio Code

### Herramientas de Base de Datos (Opcional)

- SQL Server Management Studio (SSMS)
- Azure Data Studio

## 🚀 Instalación

### 1. Clonar o Descargar el Proyecto

**Opción A - Con Git:**
```bash
git clone https://github.com/TU_USUARIO/TiendaRopaPOS.git
cd TiendaRopaPOS
```

**Opción B - Descarga Directa:**
- Descargar el archivo .zip
- Extraer en la ubicación deseada

### 2. Configurar la Base de Datos

#### 2.1 Crear la Base de Datos

1. Abrir SQL Server Management Studio (SSMS) o Azure Data Studio
2. Conectarse al servidor SQL Server
3. Ejecutar el script de creación:

```bash
Database/
├── 01_CreateDatabase.sql          # Crear BD y tablas
├── 02_CreateStoredProcedures.sql  # Todos los SPs
└── 03_InsertSampleData.sql        # Datos de prueba (opcional)
```

Ejecutar en orden:
```sql
-- 1. Crear base de datos y tablas
-- Archivo: Database/01_CreateDatabase.sql

-- 2. Crear stored procedures
-- Archivo: Database/02_CreateStoredProcedures.sql

-- 3. (Opcional) Insertar datos de prueba
-- Archivo: Database/03_InsertSampleData.sql
```

#### 2.2 Verificar la Instalación

Ejecutar esta consulta para verificar:
```sql
USE Tienda_Ropa;
GO

-- Verificar tablas
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- Verificar stored procedures
SELECT ROUTINE_NAME 
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_TYPE = 'PROCEDURE';
```

### 3. Configurar la Aplicación

Abrir el archivo `Config/ConnectionConfig.cs` y modificar los parámetros de conexión:

```csharp
public static class ConnectionConfig
{
    // Configuración para servidor local
    public static string Server = "localhost";
    // O para instancia nombrada: "localhost\\SQLEXPRESS"
    
    public static string DatabaseName = "Tienda_Ropa";
    public static string UserId = "tu_usuario";
    public static string Password = "tu_contraseña";
    public static bool TrustServerCertificate = true;
    
    // ... resto del código
}
```

**Ejemplos de configuración:**

**SQL Server local (autenticación SQL):**
```csharp
public static string Server = "localhost";
public static string UserId = "sa";
public static string Password = "TuPassword123";
```

**SQL Server Express:**
```csharp
public static string Server = "localhost\\SQLEXPRESS";
public static string UserId = "sa";
public static string Password = "TuPassword123";
```

**Azure SQL Database:**
```csharp
public static string Server = "tu-servidor.database.windows.net";
public static string UserId = "tu-usuario";
public static string Password = "tu-contraseña";
```

### 4. Compilar y Ejecutar

#### Opción A - Con Visual Studio

1. Abrir `Tienda_Ropa3.sln`
2. Restaurar paquetes NuGet (automático)
3. Presionar `F5` o hacer clic en "Iniciar"

#### Opción B - Desde la Terminal

```bash
# Navegar a la carpeta del proyecto
cd "ruta/al/proyecto/TiendaRopaPOS"

# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run
```

## ⚙️ Configuración

### Configuración de Conexión Avanzada

Si necesitas configurar parámetros adicionales, edita el método `GetConnectionString()` en `ConnectionConfig.cs`:

```csharp
public static string GetConnectionString()
{
    return $"Server={Server};" +
           $"Database={DatabaseName};" +
           $"User Id={UserId};" +
           $"Password={Password};" +
           $"TrustServerCertificate={TrustServerCertificate};" +
           $"Encrypt=True;" +           // Añadir si es necesario
           $"Connection Timeout=30;";   // Ajustar timeout
}
```

### Permisos de Base de Datos

El usuario de SQL Server debe tener los siguientes permisos:

```sql
USE Tienda_Ropa;
GO

-- Permisos mínimos necesarios
GRANT EXECUTE TO tu_usuario;
GRANT SELECT, INSERT, UPDATE, DELETE TO tu_usuario;
```

## 📁 Estructura del Proyecto

```
TiendaRopaPOS/
│
├── Config/                      # Configuración
│   └── ConnectionConfig.cs      # Cadena de conexión a BD
│
├── Data/                        # Capa de acceso a datos
│   └── DatabaseAccess.cs        # Ejecutor de stored procedures
│
├── Models/                      # Modelos de dominio
│   ├── Cliente.cs
│   ├── Producto.cs
│   ├── Categoria.cs
│   ├── Subcategoria.cs
│   ├── Pedido.cs
│   ├── ItemPedido.cs
│   └── Factura.cs
│
├── Services/                    # Lógica de negocio
│   ├── ClienteService.cs        # Gestión de clientes
│   ├── InventarioService.cs     # Gestión de productos
│   ├── VentaService.cs          # Proceso de ventas
│   ├── HistorialService.cs      # Consulta de historial
│   ├── PromocionService.cs      # Sistema de promociones
│   └── CategoriaService.cs      # Categorización
│
├── Views/                       # Interfaces de usuario
│   ├── (22 archivos .cs y .Designer.cs)
│   └── (Formularios Windows Forms)
│
├── Database/                    # Scripts SQL
│   ├── 01_CreateDatabase.sql
│   ├── 02_CreateStoredProcedures.sql
│   └── 03_InsertSampleData.sql
│
├── TiendaRopaPOS.csproj        # Archivo del proyecto
├── Tienda_Ropa3.sln            # Solución de Visual Studio
└── README.md                    # Este archivo
```

## 📖 Uso

### Primera Ejecución

1. **Crear Categorías:**
   - Módulo Inventario → Gestionar Categorías
   - Ejemplos: "Ropa de Mujer", "Ropa de Hombre", "Accesorios"

2. **Crear Subcategorías:**
   - Dentro de cada categoría
   - Ejemplos: "Camisas", "Pantalones", "Vestidos"

3. **Agregar Productos:**
   - Módulo Inventario → Nuevo Producto
   - Especificar: Nombre, Color, Talla, Precios, Subcategoría
   - Agregar stock inicial

4. **Registrar Clientes:**
   - Módulo Clientes → Nuevo Cliente
   - Datos: Nombre, Apellido, Email (opcional), Teléfono (opcional)

5. **(Opcional) Configurar Promociones:**
   - Módulo Promociones → Nueva Promoción
   - Definir descuento, fechas y alcance
   - Asignar a productos/categorías

### Flujo de Venta Típico

1. **Iniciar Venta:**
   - Módulo Ventas (POS)
   - Seleccionar o buscar cliente

2. **Agregar Productos:**
   - Buscar producto en el catálogo
   - Especificar cantidad
   - Los descuentos se aplican automáticamente

3. **Revisar Carrito:**
   - Verificar items, cantidades y precios
   - Eliminar productos si es necesario
   - Ver subtotal, descuentos y total

4. **Finalizar Venta:**
   - Confirmar pedido
   - Generar factura
   - Imprimir comprobante

5. **Consultar Historial:**
   - Módulo Historial
   - Buscar por cliente
   - Ver/reimprimir facturas anteriores

## 🗄️ Base de Datos

### Arquitectura

El sistema utiliza una arquitectura basada en **Stored Procedures** para:
- Mayor seguridad (prevención de SQL Injection)
- Mejor rendimiento (planes de ejecución pre-compilados)
- Lógica centralizada en la base de datos
- Mantenimiento simplificado

### Tablas Principales

- **Cliente:** Información de clientes
- **Categoria/Subcategoria:** Jerarquía de productos
- **Producto:** Catálogo con atributos
- **Inventario:** Control de stock
- **Pedido:** Encabezado de ventas
- **Detalle_Pedido:** Items de cada venta
- **Promocion:** Descuentos configurables
- **Aplicacion_Promo:** Asignación de promociones

### Stored Procedures Principales

**Clientes:** `Insertar_Cliente`, `Obtener_Clientes`, `Obtener_Cliente_Detalle`, `Editar_Cliente`

**Inventario:** `Insertar_Producto`, `Obtener_Productos`, `Obtener_Producto_Info`, `Actualizar_Stock`, `Editar_Precio_Producto`

**Ventas:** `CrearPedido`, `AgregarProducto`, `Obtener_Items_Pedido`, `EliminarProductoPedido`, `EliminarPedido`, `ActualizarEstadoPedido`

**Promociones:** `CrearPromocion`, `AsignarPromocion`, `Desactivar_Promocion`, `Obtener_Promociones_Activas`

**Reportes:** `ObtenerHistorialCliente`, `ObtenerFactura`

**Función:** `CalcularPrecioFinalProduct` - Calcula precio con descuento automático

### Diagrama de Flujo de Datos

```
Cliente → Pedido (1:N)
Pedido → Detalle_Pedido (1:N)
Detalle_Pedido → Producto (N:1)
Producto → Subcategoria (N:1)
Subcategoria → Categoria (N:1)
Producto → Inventario (1:1)
Promocion → Aplicacion_Promo (1:N)
Aplicacion_Promo → Producto/Categoria/Subcategoria (N:1)
```

## 🐛 Solución de Problemas

### Error: "No se puede conectar a la base de datos"

**Causas posibles:**
- SQL Server no está ejecutándose
- Credenciales incorrectas
- Servidor/instancia incorrecta

**Solución:**
```bash
# 1. Verificar que SQL Server esté corriendo
# Windows: Services → SQL Server (MSSQLSERVER o tu instancia)

# 2. Probar conexión con SSMS usando las mismas credenciales

# 3. Verificar el nombre del servidor:
SELECT @@SERVERNAME;

# 4. Verificar ConnectionConfig.cs tiene los datos correctos
```

### Error: "Could not find stored procedure"

**Causa:** Los stored procedures no se ejecutaron correctamente

**Solución:**
```sql
-- Verificar SPs instalados
USE Tienda_Ropa;
SELECT ROUTINE_NAME 
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_TYPE = 'PROCEDURE';

-- Re-ejecutar: Database/02_CreateStoredProcedures.sql
```

### Error: "El stock no puede quedar negativo"

**Causa:** Intentando vender más unidades de las disponibles

**Solución:**
- Verificar stock disponible antes de agregar al carrito
- Actualizar inventario si es necesario

### Error de compilación: "No se puede restaurar paquetes NuGet"

**Solución:**
```bash
dotnet clean
dotnet restore
dotnet build
```

### La aplicación se cierra inmediatamente

**Solución:**
```bash
# Ejecutar desde terminal para ver errores
dotnet run

# Revisar los mensajes de error
# Usualmente es problema de conexión a BD
```

### Archivo de depuración `C:\temp\debug_factura.txt`

**Descripción:** Archivo temporal de diagnóstico

**Solución:**
- Se puede eliminar manualmente
- Para desactivar: remover código de debug en `VentaService.cs` líneas 271-280

## 📝 Notas Importantes

### Seguridad

⚠️ **IMPORTANTE:** Este proyecto almacena credenciales de base de datos en el código fuente. 

**Para producción:**
- Usar variables de entorno
- Implementar Azure Key Vault
- Configurar autenticación Windows
- Nunca subir credenciales a repositorios públicos

### Datos de Prueba

El archivo `Database/03_InsertSampleData.sql` incluye datos de ejemplo para testing:
- Clientes de prueba
- Productos de catálogo básico
- Categorías y subcategorías
- Promociones de ejemplo

### Respaldo de Datos

**Recomendación:** Realizar respaldos periódicos

```sql
-- Backup manual
BACKUP DATABASE Tienda_Ropa 
TO DISK = 'C:\Backups\Tienda_Ropa.bak'
WITH FORMAT, INIT, NAME = 'Full Backup';

-- Restore
RESTORE DATABASE Tienda_Ropa 
FROM DISK = 'C:\Backups\Tienda_Ropa.bak'
WITH REPLACE;
```

## 👥 Autores

- **Karen Landivar** - Desarrollo de módulos de gestión
- **Adriana Hernández** - Sistema de promociones y pedidos

## 📄 Licencia

Este proyecto fue desarrollado como proyecto académico.

## 🤝 Contribuciones

Para reportar bugs o sugerir mejoras:
1. Crear un Issue en GitHub
2. Describir el problema o mejora
3. Incluir pasos para reproducir (si es bug)

---

**Última actualización:** Diciembre 2025

**Versión:** 1.0.0

**Tecnologías:** C# .NET 6.0, Windows Forms, SQL Server 2019+
