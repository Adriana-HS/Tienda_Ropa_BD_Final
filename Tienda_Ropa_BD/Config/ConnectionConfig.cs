namespace TiendaRopaPOS.Config
{
    public static class ConnectionConfig
    {
        // Configuración de conexión a SQL Server
        // Modifica estos valores según tu entorno
        public static string ServerName { get; set; } = ".";
        public static string DatabaseName { get; set; } = "Tienda_Ropa";
        public static string UserId { get; set; } = "Cajera";
        public static string Password { get; set; } = "1234";
        public static bool IntegratedSecurity { get; set; } = false;
        
        public static string GetConnectionString()
        {
            if (IntegratedSecurity)
            {
                return $"Server={ServerName};Database={DatabaseName};Integrated Security=True;TrustServerCertificate=True;";
            }
            else
            {
                return $"Server={ServerName};Database={DatabaseName};User Id={UserId};Password={Password};TrustServerCertificate=True;";
            }
        }
    }
}

