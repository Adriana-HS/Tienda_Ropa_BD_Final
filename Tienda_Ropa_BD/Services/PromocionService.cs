using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TiendaRopaPOS.Data;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Services
{
    public class PromocionService
    {
        public async Task<int> CrearPromocionAsync(string nombre, string? descripcion, 
            decimal? porcentaje, decimal? monto, DateTime fechaInicio, DateTime fechaFin)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@Nombre", nombre),
                new SqlParameter("@Descripcion", string.IsNullOrWhiteSpace(descripcion) ? DBNull.Value : descripcion),
                new SqlParameter("@Porcentaje", porcentaje.HasValue ? (object)porcentaje.Value : DBNull.Value),
                new SqlParameter("@Monto", monto.HasValue ? (object)monto.Value : DBNull.Value),
                new SqlParameter("@FechaInicio", fechaInicio.Date),
                new SqlParameter("@FechaFin", fechaFin.Date),
                new SqlParameter("@id_promo_necesario", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("CrearPromocion", parameters);
                return Convert.ToInt32(parameters[6].Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear promoción: {ex.Message}", ex);
            }
        }

        public async Task AsignarPromocionAsync(int idPromocion, decimal montoMinCompra, 
            int? idProducto = null, int? idCategoria = null, int? idSubcategoria = null)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@id_producto", idProducto.HasValue ? (object)idProducto.Value : DBNull.Value),
                new SqlParameter("@id_categoria", idCategoria.HasValue ? (object)idCategoria.Value : DBNull.Value),
                new SqlParameter("@id_subcategoria", idSubcategoria.HasValue ? (object)idSubcategoria.Value : DBNull.Value),
                new SqlParameter("@id_promo", idPromocion),
                new SqlParameter("@monto_min_compra", montoMinCompra)
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("AsignarPromocion", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al asignar promoción: {ex.Message}", ex);
            }
        }

        public async Task DesactivarPromocionAsync(int idPromocion)
        {
            using var db = new DatabaseAccess();
            var parameters = new[] { new SqlParameter("@id_promo", idPromocion) };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("Desactivar_Promocion", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desactivar promoción: {ex.Message}", ex);
            }
        }

        public async Task<List<KeyValuePair<int, string>>> ObtenerPromocionesActivasAsync()
        {
            using var db = new DatabaseAccess();
            var dataTable = await db.ExecuteStoredProcedureAsync("Obtener_Promociones_Activas");

            var promociones = new List<KeyValuePair<int, string>>();
            foreach (DataRow row in dataTable.Rows)
            {
                int id = Convert.ToInt32(row["id_promocion"]);
                string nombre = row["nombre"]?.ToString() ?? string.Empty;
                promociones.Add(new KeyValuePair<int, string>(id, nombre));
            }
            return promociones;
        }
    }
}

