using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TiendaRopaPOS.Data;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Services
{
    public class InventarioService
    {
        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            using var db = new DatabaseAccess();
            var dataTable = await db.ExecuteStoredProcedureAsync("Obtener_Productos");

            var productos = new List<Producto>();
            foreach (DataRow row in dataTable.Rows)
            {
                productos.Add(new Producto
                {
                    IdProducto = Convert.ToInt32(row["id_producto"]),
                    Nombre = row["Nombre"].ToString() ?? string.Empty,
                    Descripcion = row["Descripcion"] != DBNull.Value ? row["Descripcion"].ToString() ?? string.Empty : string.Empty,
                    Categoria = row["Categoria"] != DBNull.Value ? row["Categoria"].ToString() ?? string.Empty : string.Empty, 
                    Subcategoria = row["Subcategoria"] != DBNull.Value ? row["Subcategoria"].ToString() ?? string.Empty : string.Empty,
                    PrecioBase = Convert.ToDecimal(row["Precio_base"]),
                    PrecioVenta = Convert.ToDecimal(row["Precio_venta"]),
                    StockActual = row["Stock"] != DBNull.Value ? Convert.ToInt32(row["Stock"]) : 0,
                    // Dentro de la creación del objeto Producto, añade estas líneas:
                    Color = row["Color"] != DBNull.Value ? row["Color"].ToString() ?? "" : "",
                    Talla = row["Talla"] != DBNull.Value ? row["Talla"].ToString() ?? "" : "",
                });
            }

            return productos;
        }

        public async Task<Producto?> ObtenerProductoInfoAsync(int idProducto)
        {
            using var db = new DatabaseAccess();
            var parameters = new[] { new SqlParameter("@id_producto", idProducto) };
            var dataTable = await db.ExecuteStoredProcedureAsync("Obtener_Producto_Info", parameters);

            if (dataTable.Rows.Count == 0)
                return null;

            var row = dataTable.Rows[0];
            return new Producto
            {
                IdProducto = Convert.ToInt32(row["id_producto"]),
                Nombre = row["Nombre"].ToString() ?? string.Empty,
                Descripcion = row["Descripcion"] != DBNull.Value ? row["Descripcion"].ToString() ?? string.Empty : string.Empty,
                Categoria = row["Categoria"] != DBNull.Value ? row["Categoria"].ToString() ?? string.Empty : string.Empty, 
                Subcategoria = row["Subcategoria"] != DBNull.Value ? row["Subcategoria"].ToString() ?? string.Empty : string.Empty,
                PrecioBase = Convert.ToDecimal(row["Precio_base"]),
                PrecioVenta = Convert.ToDecimal(row["Precio_venta"]),
                StockActual = row["Stock"] != DBNull.Value ? Convert.ToInt32(row["Stock"]) : 0,
                Color = row["Color"] != DBNull.Value ? row["Color"].ToString() ?? "" : "",
                Talla = row["Talla"] != DBNull.Value ? row["Talla"].ToString() ?? "" : "",
            };
        }

        public async Task EditarPrecioProductoAsync(int idProducto, decimal precioBase, decimal precioVenta)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@id_producto", idProducto),
                new SqlParameter("@Precio_base", precioBase),
                new SqlParameter("@Precio_venta", precioVenta)
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("Editar_Precio_Producto", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al editar precio: {ex.Message}", ex);
            }
        }

        public async Task ActualizarStockAsync(int idProducto, int cantidadAjuste)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@id_producto", idProducto),
                new SqlParameter("@CantidadAjuste", cantidadAjuste) // Puede ser positivo (entrada) o negativo (salida)
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("Actualizar_Stock", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar stock: {ex.Message}", ex);
            }
        }

        public async Task InsertarProductoAsync(string nombre, string? descripcion, decimal precioBase, 
            decimal precioVenta, string color, string talla, int idSubcategoria)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@Nombre", nombre),
                new SqlParameter("@Descripcion", string.IsNullOrWhiteSpace(descripcion) ? DBNull.Value : descripcion),
                new SqlParameter("@Precio_base", precioBase),
                new SqlParameter("@Precio_venta", precioVenta),
                new SqlParameter("@Color", color),
                new SqlParameter("@Talla", talla),
                new SqlParameter("@id_subcategoria", idSubcategoria)
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("Insertar_Producto", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar producto: {ex.Message}", ex);
            }
        }
    }
}

