using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TiendaRopaPOS.Data;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Services
{
    public class ClienteService
    {
        public async Task<List<Cliente>> ObtenerClientesAsync(string? filtro = null)
        {
            using var db = new DatabaseAccess();
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                parameters.Add(new SqlParameter("@Filtro", filtro));
            }
            else
            {
                parameters.Add(new SqlParameter("@Filtro", DBNull.Value));
            }

            var dataTable = await db.ExecuteStoredProcedureAsync("Obtener_Clientes", parameters.ToArray());
            
            var clientes = new List<Cliente>();
            foreach (DataRow row in dataTable.Rows)
            {
                clientes.Add(new Cliente
                {
                    IdCliente = Convert.ToInt32(row["id_cliente"]),
                    Nombre = row["Nombre"].ToString() ?? string.Empty,
                    Apellido = row["Apellido"].ToString() ?? string.Empty,
                    Email = row["Email"] != DBNull.Value ? row["Email"].ToString() ?? string.Empty : string.Empty,
                    Telefono = row["Telefono"] != DBNull.Value ? row["Telefono"].ToString() ?? string.Empty : string.Empty,
                    FechaRegistro = row["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(row["FechaRegistro"]) : (DateTime?)null
                });
            }

            return clientes;
        }

        public async Task<Cliente?> ObtenerClienteDetalleAsync(int? idCliente = null, string? email = null)
        {
            using var db = new DatabaseAccess();
            var parameters = new List<SqlParameter>();
            
            if (idCliente.HasValue)
            {
                parameters.Add(new SqlParameter("@id_cliente", idCliente.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@id_cliente", DBNull.Value));
            }
            
            if (!string.IsNullOrWhiteSpace(email))
            {
                parameters.Add(new SqlParameter("@Email", email));
            }
            else
            {
                parameters.Add(new SqlParameter("@Email", DBNull.Value));
            }
            
            var dataTable = await db.ExecuteStoredProcedureAsync("Obtener_Cliente_Detalle", parameters.ToArray());

            if (dataTable.Rows.Count == 0)
                return null;

            var row = dataTable.Rows[0];
            return new Cliente
            {
                IdCliente = Convert.ToInt32(row["id_cliente"]),
                Nombre = row["Nombre"].ToString() ?? string.Empty,
                Apellido = row["Apellido"].ToString() ?? string.Empty,
                Email = row["Email"] != DBNull.Value ? row["Email"].ToString() ?? string.Empty : string.Empty,
                Telefono = row["Telefono"] != DBNull.Value ? row["Telefono"].ToString() ?? string.Empty : string.Empty,
                FechaRegistro = null
            };
        }

        public async Task InsertarClienteAsync(string nombre, string apellido, string? email = null, string? telefono = null)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@Nombre", nombre),
                new SqlParameter("@Apellido", apellido),
                new SqlParameter("@Email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email),
                new SqlParameter("@Telefono", string.IsNullOrWhiteSpace(telefono) ? DBNull.Value : telefono)
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("Insertar_Cliente", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar cliente: {ex.Message}", ex);
            }
        }

        public async Task EditarClienteAsync(int idCliente, string? nombre = null, string? apellido = null, string? email = null, string? telefono = null)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@id_cliente", idCliente),
                new SqlParameter("@Nombre", string.IsNullOrWhiteSpace(nombre) ? DBNull.Value : nombre),
                new SqlParameter("@Apellido", string.IsNullOrWhiteSpace(apellido) ? DBNull.Value : apellido),
                new SqlParameter("@Email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email),
                new SqlParameter("@Telefono", string.IsNullOrWhiteSpace(telefono) ? DBNull.Value : telefono)
            };

            try
            {
                await db.ExecuteStoredProcedureNonQueryAsync("Editar_Cliente", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al editar cliente: {ex.Message}", ex);
            }
        }
    }
}

