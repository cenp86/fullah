using System;
using MySql.Data.MySqlClient;

namespace UalaAccounting.api.Services
{
	public class FormedSqlExecution : IFormedSqlExecution
    {
        private readonly IConfiguration _config;
        private readonly ILogger<FormedSqlExecution> _logger;
        private string connectionString = string.Empty;

        public FormedSqlExecution(ILogger<FormedSqlExecution> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;

        }

        public void SetConnectionString(string connection)
        {
            this.connectionString = connection;
        }

        public async Task ExecuteDatabaseNativeCommand(string strQuery)
        {
            try
            {
                //string connectionString = "server=localhost;database=PEACHUB;user=root;password=mambu123";
                using (MySqlConnection connection = new MySqlConnection(this.connectionString))
                {
                    connection.Open();


                    using (MySqlCommand command = new MySqlCommand(strQuery, connection))
                    {


                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Inserción exitosa");
                        }
                        else
                        {
                            Console.WriteLine("No se pudo insertar");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.LogError($"An error occurred during the process 'ExecuteDatabaseCommand' -- module: {exc} -- query: {strQuery}");
                throw;
            }
        }
    }
}

