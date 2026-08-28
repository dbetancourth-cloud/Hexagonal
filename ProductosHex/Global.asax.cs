using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Web.Http;
using ProductosHex.Infrastructure.Data;
using ProductosHex.Infrastructure.Logging;

namespace ProductosHex
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var logger = new FileLoggerService();

            try
            {
                GlobalConfiguration.Configure(WebApiConfig.Register);

                // Desactiva la verificación de migraciones de EF
                Database.SetInitializer<ProductosDbContext>(null);

                var dbPath = Path.Combine(
                    AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
                    "productos.db");

                bool existeConTablas = false;

                if (File.Exists(dbPath))
                {
                    using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table'";
                            existeConTablas = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                        }
                    }
                }

                if (!existeConTablas)
                {
                    if (File.Exists(dbPath))
                        File.Delete(dbPath);

                    CrearEsquema(dbPath);
                    logger.LogInfo("Base de datos SQLite creada y esquema inicializado con éxito.");
                }
                else
                {
                    // Asegura que las columnas requeridas existan en DBs previas
                    AsegurarColumnaNumeroDocumento(dbPath);
                    AsegurarColumnasFactura(dbPath);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error durante la inicialización de la aplicación en Application_Start", ex);
                throw;
            }
        }

        private void CrearEsquema(string dbPath)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();

                string script = @"
                    CREATE TABLE IF NOT EXISTS Categorias (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Clientes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        NumeroDocumento TEXT,
                        Nombre TEXT,
                        Telefono TEXT,
                        Email TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Productos (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL,
                        Precio DECIMAL NOT NULL,
                        Stock INTEGER NOT NULL,
                        CategoriaId INTEGER NOT NULL,
                        PorcentajeIva DECIMAL NOT NULL,
                        FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
                    );

                    CREATE TABLE IF NOT EXISTS Facturas (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        NumeroFactura TEXT,
                        Fecha TEXT,
                        ClienteId INTEGER NOT NULL,
                        ClienteNombreHistorico TEXT,
                        ClienteEmailHistorico TEXT,
                        ClienteDocumentoHistorico TEXT,
                        Descuento DECIMAL NOT NULL,
                        Iva DECIMAL NOT NULL,
                        ValorTotal DECIMAL NOT NULL,
                        FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
                    );

                    CREATE TABLE IF NOT EXISTS FacturaDetalles (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FacturaEncabezado_Id INTEGER,
                        FacturaEncabezadoId INTEGER,
                        ProductoId INTEGER NOT NULL,
                        ProductoNombreHistorico TEXT,
                        Cantidad INTEGER NOT NULL,
                        PrecioUnitario DECIMAL NOT NULL,
                        PrecioBruto DECIMAL NOT NULL,
                        PorcentajeDescuento DECIMAL NOT NULL DEFAULT 0,
                        PorcentajeIva DECIMAL NOT NULL,
                        ValorIva DECIMAL NOT NULL,
                        FOREIGN KEY (FacturaEncabezado_Id) REFERENCES Facturas(Id),
                        FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
                    );
                ";

                using (var cmd = conn.CreateCommand())
                {
                    foreach (var statement in script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (string.IsNullOrWhiteSpace(statement)) continue;
                        cmd.CommandText = statement;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void AsegurarColumnaNumeroDocumento(string dbPath)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA table_info(Clientes);";
                    bool existeColumna = false;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["name"].ToString().Equals("NumeroDocumento", StringComparison.OrdinalIgnoreCase))
                            {
                                existeColumna = true;
                                break;
                            }
                        }
                    }

                    if (!existeColumna)
                    {
                        cmd.CommandText = "ALTER TABLE Clientes ADD COLUMN NumeroDocumento TEXT;";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void AsegurarColumnasFactura(string dbPath)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();

                // 1. Verificar columnas en Facturas
                var columnasFacturas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA table_info(Facturas);";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columnasFacturas.Add(reader["name"].ToString());
                        }
                    }
                }

                if (!columnasFacturas.Contains("NumeroFactura"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "ALTER TABLE Facturas ADD COLUMN NumeroFactura TEXT;";
                        cmd.ExecuteNonQuery();
                    }
                }

                if (!columnasFacturas.Contains("Fecha"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "ALTER TABLE Facturas ADD COLUMN Fecha TEXT;";
                        cmd.ExecuteNonQuery();
                    }
                }

                if (!columnasFacturas.Contains("ClienteDocumentoHistorico"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "ALTER TABLE Facturas ADD COLUMN ClienteDocumentoHistorico TEXT;";
                        cmd.ExecuteNonQuery();
                    }
                }

                // 2. Verificar columnas en FacturaDetalles
                var columnasDetalles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA table_info(FacturaDetalles);";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columnasDetalles.Add(reader["name"].ToString());
                        }
                    }
                }

                if (!columnasDetalles.Contains("PorcentajeDescuento"))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "ALTER TABLE FacturaDetalles ADD COLUMN PorcentajeDescuento DECIMAL NOT NULL DEFAULT 0;";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null)
            {
                var logger = new FileLoggerService();
                logger.LogError("Error no controlado a nivel de aplicación (Global.asax)", ex);
            }
        }
    }
}