using System;
using System.Diagnostics;
using MySqlConnector;

namespace UalaAccounting.api.Services
{
	public class ShellServices : IShellServices
    {
        private string _dbServer;
        private string _dbUser;
        private uint _dbPort;
        private string _dbPass;
        private string _dbName;
        private string _mysqlPath;
        private string _connectionString;
        private uint defaultPort = 3306;
        private readonly ILogger<ShellServices> _logger;

        public ShellServices(ILogger<ShellServices> logger, IConfiguration configuration)
		{
            _logger = logger;
            _connectionString = configuration["ACHUB"];
        }

        public async Task FormatConnectionString(string connectionString = null)
        {
            try
            {
                MySqlConnectionStringBuilder builder;

                if (string.IsNullOrEmpty(connectionString))
                {
                    builder = new MySqlConnectionStringBuilder(_connectionString);
                }
                else
                {
                    builder = new MySqlConnectionStringBuilder(connectionString);
                }

                _dbServer = builder.Server;
                _dbPort = builder.Port != 0? builder.Port : defaultPort;
                _dbUser = builder.UserID;
                _dbPass = builder.Password;
                _dbName = builder.Database;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetMySqlPath(string pathOfmysql)
        {
            _mysqlPath = pathOfmysql;
        }

        public async Task ImportRestore(string? fileForRestore)
        {
            try
            {
                string arguments = $"--init-command=\"SET foreign_key_checks=0;\" --host={this._dbServer} --user={this._dbUser} --port={this._dbPort} --password={this._dbPass} -e \"source {fileForRestore}\" {this._dbName}";

                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = this._mysqlPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = processInfo;
                    process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                    process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();

                    Console.WriteLine("Restore Proccess Executed OK.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

