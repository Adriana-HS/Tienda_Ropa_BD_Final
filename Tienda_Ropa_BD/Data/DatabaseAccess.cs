using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TiendaRopaPOS.Config;

namespace TiendaRopaPOS.Data
{
    public class DatabaseAccess : IDisposable
    {
        private SqlConnection? _connection;

        public DatabaseAccess()
        {
            _connection = new SqlConnection(ConnectionConfig.GetConnectionString());
        }

        private async Task<SqlConnection> GetConnectionAsync()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(ConnectionConfig.GetConnectionString());
            }

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            return _connection;
        }

        public async Task<DataTable> ExecuteStoredProcedureAsync(string procedureName, params SqlParameter[] parameters)
        {
            var connection = await GetConnectionAsync();
            using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            using var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        public async Task<int> ExecuteStoredProcedureScalarAsync(string procedureName, params SqlParameter[] parameters)
        {
            var connection = await GetConnectionAsync();
            using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            var result = await command.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        public async Task ExecuteStoredProcedureNonQueryAsync(string procedureName, params SqlParameter[] parameters)
        {
            var connection = await GetConnectionAsync();
            using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                // Re-lanzar la excepción con el mensaje completo de SQL
                var errorMessage = ex.Message;
                if (ex.Number > 0)
                {
                    errorMessage = $"Error SQL {ex.Number}: {ex.Message}";
                }
                throw new Exception(errorMessage, ex);
            }
            catch (Exception ex)
            {
                // Capturar cualquier otro error
                throw new Exception($"Error al ejecutar procedimiento almacenado: {ex.Message}", ex);
            }
        }

        public async Task<(DataTable FirstResult, DataTable? SecondResult)> ExecuteStoredProcedureMultiResultAsync(
            string procedureName, params SqlParameter[] parameters)
        {
            var connection = await GetConnectionAsync();
            using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            using var reader = await command.ExecuteReaderAsync();
            var dataSet = new DataSet();
            dataSet.Load(reader, LoadOption.OverwriteChanges, "FirstResult", "SecondResult");

            var firstTable = dataSet.Tables.Count > 0 ? dataSet.Tables[0] : new DataTable();
            DataTable? secondTable = dataSet.Tables.Count > 1 ? dataSet.Tables[1] : null;
            return (firstTable, secondTable);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}

