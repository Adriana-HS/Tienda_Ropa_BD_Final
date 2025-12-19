using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TiendaRopaPOS.Data;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Services
{
    public class HistorialService
    {
        public async Task<List<Pedido>> ObtenerHistorialClienteAsync(int idCliente)
        {
            using var db = new DatabaseAccess();
            var parameters = new[] { new SqlParameter("@id_cliente", idCliente) };
            var dataTable = await db.ExecuteStoredProcedureAsync("ObtenerHistorialCliente", parameters);

            var pedidos = new List<Pedido>();
            foreach (DataRow row in dataTable.Rows)
            {
                pedidos.Add(new Pedido
                {
                    IdPedido = Convert.ToInt32(row["id_pedido"]),
                    IdCliente = idCliente, // Lo sabemos del parámetro
                    NombreCliente = string.Empty, // No viene en el SP
                    FechaPedido = Convert.ToDateTime(row["fecha"]),
                    EstadoPedido = row["EstadoPedido"].ToString() ?? string.Empty, // El SP ya devuelve el estado traducido
                    Total = Convert.ToDecimal(row["total"])
                });
            }

            return pedidos;
        }
    }
}

