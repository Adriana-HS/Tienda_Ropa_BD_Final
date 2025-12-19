using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TiendaRopaPOS.Data;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Services
{
    public class CategoriaService
    {
        public async Task<List<Categoria>> ObtenerCategoriasAsync()
        {
            using var db = new DatabaseAccess();
            var dataTable = await db.ExecuteStoredProcedureAsync("Obtener_Categorias"); // Assuming this SP exists

            var categorias = new List<Categoria>();
            foreach (DataRow row in dataTable.Rows)
            {
                categorias.Add(new Categoria
                {
                    IdCategoria = Convert.ToInt32(row["id_categoria"]),
                    Nombre = row["Nombre"].ToString() ?? string.Empty,
                    Detalles = row["Detalles"] != DBNull.Value ? row["Detalles"].ToString() ?? string.Empty : string.Empty
                });
            }

            return categorias;
        }

        public async Task InsertarCategoriaAsync(string nombre, string? detalles = null)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@Nombre", nombre),
                new SqlParameter("@Detalles", detalles ?? (object)DBNull.Value)
            };
            await db.ExecuteStoredProcedureNonQueryAsync("Insertar_Categoria", parameters);
        }

        public async Task AgregarSubcategoriaAsync(string nombre, int idCategoria, string? detalles = null)
        {
            using var db = new DatabaseAccess();
            var parameters = new[]
            {
                new SqlParameter("@Nombre", nombre),
                new SqlParameter("@Detalles", detalles ?? (object)DBNull.Value),
                new SqlParameter("@id_categoria", idCategoria)
            };
            await db.ExecuteStoredProcedureNonQueryAsync("AgregarSubcategoria", parameters);
        }

        public async Task<List<Subcategoria>> ObtenerSubcategoriasAsync()
        {
            using var db = new DatabaseAccess();
            var dataTable = await db.ExecuteStoredProcedureAsync("Obtener_Subcategorias");

            var subcategorias = new List<Subcategoria>();
            foreach (DataRow row in dataTable.Rows)
            {
                subcategorias.Add(new Subcategoria
                {
                    IdSubcategoria = Convert.ToInt32(row["id_subcategoria"]),
                    Nombre = row["Nombre"].ToString() ?? string.Empty,
                    Detalles = row["Detalles"] != DBNull.Value ? row["Detalles"].ToString() ?? string.Empty : string.Empty,
                    IdCategoria = Convert.ToInt32(row["id_categoria"])
                });
            }

            return subcategorias;
        }
    }
}