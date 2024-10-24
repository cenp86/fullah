using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Forms;

namespace UalaAccounting.test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        FilesProcess file = new FilesProcess();
        file.ProcessFile();
    }


    [Fact]
    public void RemoveFK()
    {
        string filePath = "/Users/alexis.salinas@mambu.com/Documents/customers/uala/ah/zipFolder/ualamxdev.sql"; // Cambia este path por el de tu archivo
        string outputFile = "/Users/alexis.salinas@mambu.com/Documents/customers/uala/ah/zipFolder/path_to_your_cleaned_restore.sql"; // Cambia este path por el de tu archivo

        try
        {
            ValidateSqlFile(filePath, outputFile);
            Console.WriteLine("Todas las sentencias CREATE TABLE parecen correctas.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en la validación: {ex.Message}");
        }
    }

    public void ValidateSqlFile(string filePath, string outputFile)
    {
        string[] lines = File.ReadAllLines(filePath);
        bool inCreateTable = false;
        string createTableBlock = string.Empty;

        using (StreamReader reader = new StreamReader(filePath))
        using (StreamWriter writer = new StreamWriter(outputFile))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, @"^\s*CREATE\s+TABLE", RegexOptions.IgnoreCase))
                {
                    if (inCreateTable)
                    {
                        throw new Exception("Se encontró un bloque CREATE TABLE dentro de otro. Esto no es válido.");
                    }
                    inCreateTable = true;
                    createTableBlock = line + "\n";
                }
                else if (inCreateTable)
                {
                    createTableBlock += line + "\n";

                    if (line.Trim().StartsWith(") ENGINE=InnoDB"))
                    {
                        // El bloque CREATE TABLE ha terminado
                        inCreateTable = false;

                        // Validar que el bloque no tenga errores
                        line = ValidateCreateTableBlock(createTableBlock);

                        // Reiniciar el bloque
                        createTableBlock = string.Empty;
                    }
                }

                if(!inCreateTable && !Regex.IsMatch(line, @"^\/\*\!\d+", RegexOptions.IgnoreCase))
                    writer.WriteLine(line);

            }
        }

        if (inCreateTable)
        {
            throw new Exception("El archivo terminó sin cerrar un bloque CREATE TABLE.");
        }
    }

    public string ValidateCreateTableBlock(string block)
    {
        // Validar que no haya comas antes de ); si hay "ENGINE"
        if (Regex.IsMatch(block, @",\s*\)\s*ENGINE", RegexOptions.IgnoreCase))
        {
            block = Regex.Replace(block, @",\s*(\)\s*ENGINE)", "$1", RegexOptions.IgnoreCase);
        }

        // Validar que no haya FOREIGN KEY o CONSTRAINT no deseadas
        if (Regex.IsMatch(block, @"CONSTRAINT.*?FOREIGN KEY", RegexOptions.IgnoreCase))
        {
            throw new Exception("Se encontró una restricción FOREIGN KEY en el bloque CREATE TABLE.");
        }

        return block;
    }

    [Fact]
    public void Test2()
    {
        try
        {
            string arguments = $"--host=localhost --user=root --port=3306 --password=mambu123 -e \"source /Users/alexis.salinas@mambu.com/Documents/customers/uala/ah/dataFolder/FileImportToDB.sql\" conta";

            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "/usr/local/mysql-8.3.0-macos14-x86_64/bin/mysql",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = processInfo;

                process.OutputDataReceived += (sender, e) => {
                    if (!string.IsNullOrEmpty(e.Data))
                        Console.WriteLine("Output: " + e.Data);
                };

                process.ErrorDataReceived += (sender, e) => {
                    if (!string.IsNullOrEmpty(e.Data))
                        Console.WriteLine("Error: " + e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine("Restore Process Failed with Exit Code: " + process.ExitCode);

                }

                Console.WriteLine("Restore Proccess Executed OK.");
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
