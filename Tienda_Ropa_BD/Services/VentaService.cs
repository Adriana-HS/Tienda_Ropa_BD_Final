using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TiendaRopaPOS.Data;
using TiendaRopaPOS.Models;
using TiendaRopaPOS.Config;

namespace TiendaRopaPOS.Services
{
    public class VentaService
    {
        private static readonly string[] FacturaProcedureCandidates =
        {
            "dbo.sp_Obtener_Factura",
            "sp_Obtener_Factura",
            "dbo.ObtenerFactura",
            "ObtenerFactura",
            "dbo.Obtener_Factura",
            "Obtener_Factura"
        };

        public async Task<int> CrearPedidoAsync(int idCliente)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@id_cliente", idCliente),
                new SqlParameter("@id_pedido_necesario", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("CrearPedido", parameters);

                var idPedido = Convert.ToInt32(parameters[1].Value);

                var setEstadoParameters = new[]
                {
                    new SqlParameter("@id_pedido", idPedido),
                    new SqlParameter("@nuevo_estado", "P")
                };
                await db.ExecuteStoredProcedureNonQueryAsync("ActualizarEstadoPedido", setEstadoParameters);

                return idPedido;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear pedido: {ex.Message}", ex);
            }
        }

        public async Task AgregarProductoAsync(int idPedido, int idProducto, int cantidad)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@id_pedido", idPedido),
                new SqlParameter("@id_producto", idProducto),
                new SqlParameter("@cantidad", cantidad)
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("AgregarProducto", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar producto: {ex.Message}", ex);
            }
        }

        public async Task<List<ItemPedido>> ObtenerItemsPedidoAsync(int idPedido)
        {
            using var db = new DatabaseAccess();
            var parameters = new[] { new SqlParameter("@id_pedido", idPedido) };

            var dataTable = await db.ExecuteStoredProcedureAsync("Obtener_Items_Pedido", parameters);

            var items = new List<ItemPedido>();
            foreach (DataRow row in dataTable.Rows)
            {
                var cantidad = Convert.ToInt32(row["cantidad"]);
                var precioUnitario = Convert.ToDecimal(row["precio_unitario_final"]);
                var descuentoAplicado = row["descuento_aplicado"] == DBNull.Value ? 0 : Convert.ToDecimal(row["descuento_aplicado"]);
                var subtotal = Convert.ToDecimal(row["subtotal"]);

                items.Add(new ItemPedido
                {
                    IdItem = Convert.ToInt32(row["id_detalle"]),
                    IdProducto = Convert.ToInt32(row["id_producto"]),
                    Cantidad = cantidad,
                    PrecioUnitario = precioUnitario,
                    Descuento = descuentoAplicado,
                    Subtotal = subtotal,
                    NombreProducto = row["Nombre"]?.ToString() ?? string.Empty,
                    Color = row["Color"] != DBNull.Value ? row["Color"].ToString() ?? "" : "",
                    Talla = row["Talla"] != DBNull.Value ? row["Talla"].ToString() ?? "" : "",
                });
            }

            return items;
        }

        private static object? GetValue(DataRow row, params string[] names)
        {
            foreach (var name in names)
            {
                if (row.Table.Columns.Contains(name))
                    return row[name];
            }

            return null;
        }

        private static int GetInt(DataRow row, int fallback, params string[] names)
        {
            var value = GetValue(row, names);
            if (value == null || value == DBNull.Value)
                return fallback;

            return Convert.ToInt32(value);
        }

        private static decimal GetDecimal(DataRow row, decimal fallback, params string[] names)
        {
            var value = GetValue(row, names);
            if (value == null || value == DBNull.Value)
                return fallback;

            return Convert.ToDecimal(value);
        }

        private static DateTime GetDateTime(DataRow row, DateTime fallback, params string[] names)
        {
            var value = GetValue(row, names);
            if (value == null || value == DBNull.Value)
                return fallback;

            return Convert.ToDateTime(value);
        }

        private static string GetString(DataRow row, string fallback, params string[] names)
        {
            var value = GetValue(row, names);
            if (value == null || value == DBNull.Value)
                return fallback;

            var s = value.ToString();
            return string.IsNullOrWhiteSpace(s) ? fallback : s;
        }

        public async Task<Factura> ObtenerFacturaAsync(int idPedido)
        {
            using var db = new DatabaseAccess();

            (DataTable FirstResult, DataTable? SecondResult) results = default;
            SqlException? lastNotFound = null;

            foreach (var procName in FacturaProcedureCandidates)
            {
                try
                {
                    var parameters = new[] { new SqlParameter("@id_pedido", idPedido) };
                    results = await db.ExecuteStoredProcedureMultiResultAsync(procName, parameters);
                    lastNotFound = null;
                    break;
                }
                catch (SqlException ex) when (
                    ex.Number == 2812 ||
                    ex.Message.Contains("Could not find stored procedure", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("No se pudo encontrar el procedimiento almacenado", StringComparison.OrdinalIgnoreCase))
                {
                    lastNotFound = ex;
                }
            }

            if (lastNotFound != null)
            {
                throw new Exception(
                    $"No se encontró el procedimiento de factura. Se intentó con: {string.Join(", ", FacturaProcedureCandidates)}. " +
                    $"Verifica que el SP exista en la base de datos '{ConnectionConfig.DatabaseName}' y que el usuario tenga permisos.",
                    lastNotFound);
            }

            var firstResult = results.FirstResult;
            var secondResult = results.SecondResult;

            var factura = new Factura();
            factura.Items = new List<ItemPedido>();

            // Procesar primera tabla (Header/Cabecera)
            if (firstResult != null && firstResult.Rows.Count > 0)
            {
                var row = firstResult.Rows[0];

                factura.IdPedido = GetInt(row, idPedido, "id_pedido", "IdPedido", "idPedido");
                factura.Fecha = GetDateTime(row, DateTime.Now, "fecha", "Fecha", "fecha_pedido", "FechaPedido");

                var clienteNombre = GetString(row, string.Empty, "Cliente_Nombre", "Cliente", "NombreCliente", "cliente", "cliente_nombre", "NombreCompleto");
                if (string.IsNullOrWhiteSpace(clienteNombre))
                {
                    var nombre = GetString(row, string.Empty, "Nombre", "nombre", "nombres");
                    var apellido = GetString(row, string.Empty, "Apellido", "apellido", "apellidos");
                    clienteNombre = (nombre + " " + apellido).Trim();
                }
                factura.Cliente = clienteNombre;

                factura.Total = GetDecimal(row, 0m, "total", "Total", "monto_total", "MontoTotal");
            }

            // Procesar segunda tabla (Items/Detalles)
            decimal totalCalculado = 0;
            decimal descuentoTotalCalculado = 0;
            DataTable? itemsSource = null;

            if (secondResult != null && secondResult.Rows.Count > 0)
            {
                itemsSource = secondResult;
            }
            else if (firstResult != null && firstResult.Rows.Count > 0)
            {
                var hasCantidad = firstResult.Columns.Contains("cantidad") || firstResult.Columns.Contains("Cantidad") || firstResult.Columns.Contains("qty");
                var hasNombreProducto = firstResult.Columns.Contains("Producto") || firstResult.Columns.Contains("NombreProducto") || firstResult.Columns.Contains("Nombre") || firstResult.Columns.Contains("nombre_producto");
                if (hasCantidad && hasNombreProducto)
                {
                    itemsSource = firstResult;
                }
            }

            if (itemsSource == null)
            {
                var itemsTable = await db.ExecuteStoredProcedureAsync(
                    "Obtener_Items_Pedido",
                    new SqlParameter("@id_pedido", idPedido));

                if (itemsTable.Rows.Count > 0)
                {
                    itemsSource = itemsTable;
                }
            }

            if (itemsSource != null)
            {
                // DEBUG: Crear archivo de log
                var logPath = @"C:\temp\debug_factura.txt";
                System.IO.Directory.CreateDirectory(@"C:\temp");
                
                foreach (DataRow row in itemsSource.Rows)
                {
                    var logLines = new List<string>();
                    logLines.Add("=== COLUMNAS DISPONIBLES ===");
                    
                    foreach (DataColumn col in row.Table.Columns)
                    {
                        logLines.Add($"{col.ColumnName} = {row[col.ColumnName]}");
                    }
                    
                    var cantidad = GetInt(row, 0, "cantidad", "Cantidad", "qty", "CantidadProducto");
                    var precioUnitario = GetDecimal(row, 0m, "Precio_Unitario", "precio_unitario", "precio_unitario_final", "PrecioUnitario");
                    var subtotalItem = GetDecimal(row, 0m, "Subtotal", "subtotal", "SubTotal");
                    
                    // Intentar obtener descuento por unidad de múltiples columnas posibles
                    var ahorroPorUnidad = GetDecimal(row, 0m, "Ahorro_Por_Unidad", "descuento_aplicado", "descuento_por_unidad", "DescuentoAplicado", "Descuento", "AhorroPorUnidad");
                    
                    logLines.Add($"Ahorro por unidad obtenido: {ahorroPorUnidad}");
                    logLines.Add("===========================");
                    
                    System.IO.File.AppendAllLines(logPath, logLines);
                    
                    // Si no hay subtotal calculado, calcularlo
                    if (subtotalItem == 0m && cantidad > 0 && precioUnitario > 0m)
                        subtotalItem = cantidad * precioUnitario;

                    // Si no hay precio unitario, calcularlo desde el subtotal
                    if (precioUnitario == 0m && cantidad > 0 && subtotalItem > 0m)
                        precioUnitario = subtotalItem / cantidad;

                    // CRÍTICO: Calcular descuento total de la línea
                    // ahorroPorUnidad ya es el descuento por unidad
                    var descuentoLinea = ahorroPorUnidad * cantidad;

                    // El subtotal YA incluye el descuento aplicado (es el precio final)
                    // Entonces: PrecioOriginal = Subtotal + DescuentoTotal
                    totalCalculado += subtotalItem;
                    descuentoTotalCalculado += descuentoLinea;

                    factura.Items.Add(new ItemPedido
                    {
                        IdItem = GetInt(row, 0, "id_detalle", "IdItem", "id_item"),
                        IdProducto = GetInt(row, 0, "id_producto", "IdProducto", "idProducto"),
                        NombreProducto = GetString(row, string.Empty, "Producto", "producto", "NombreProducto", "Nombre", "nombre_producto"),
                        Color = GetString(row, "", "Color", "color"),
                        Talla = GetString(row, "", "Talla", "talla"),
                        Cantidad = cantidad,
                        PrecioUnitario = precioUnitario,
                        Descuento = descuentoLinea,  // Este es el descuento TOTAL de la línea
                        Subtotal = subtotalItem
                    });
                }
            }

            // Calcular subtotal y descuento total
            // Subtotal = suma de precios SIN descuento
            // Total = suma de precios CON descuento (lo que se paga)
            // DescuentoTotal = Subtotal - Total
            if (factura.Items.Count > 0)
            {
                // El totalCalculado es la suma de los subtotales con descuento ya aplicado
                factura.Total = totalCalculado;
                factura.DescuentoTotal = descuentoTotalCalculado;
                // El subtotal debe ser el total ANTES de aplicar descuentos
                factura.Subtotal = totalCalculado + descuentoTotalCalculado;
            }
            else
            {
                factura.Subtotal = factura.Total;
                factura.DescuentoTotal = 0m;
            }

            return factura;
        }

        public async Task EliminarProductoPedidoAsync(int idDetalle)
        {
            using var db = new DatabaseAccess();
            var parameters = new[] { new SqlParameter("@id_detalle", idDetalle) };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("EliminarProductoPedido", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar producto: {ex.Message}", ex);
            }
        }

        public async Task ActualizarEstadoPedidoAsync(int idPedido, string nuevoEstado)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@id_pedido", idPedido),
                new SqlParameter("@nuevo_estado", nuevoEstado) // 'P', 'C', o 'E'
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("ActualizarEstadoPedido", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar estado: {ex.Message}", ex);
            }
        }

        public async Task EliminarPedidoAsync(int idPedido)
        {
            using var db = new DatabaseAccess();
            var parameters = new[] { new SqlParameter("@id_pedido", idPedido) };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("EliminarPedido", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar pedido: {ex.Message}", ex);
            }
        }
    }
}