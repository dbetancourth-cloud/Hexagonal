using System;
using System.IO;
using ProductosHex.Domain.Ports;

namespace ProductosHex.Infrastructure.Logging
{
    public class FileLoggerService : ILoggerService
    {
        private readonly string _logPath;

        public FileLoggerService()
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            _logPath = Path.Combine(folder, $"log_{DateTime.Now:yyyyMMdd}.txt");
        }

        public void LogInfo(string mensaje)
        {
            Escribir($"[INFO] [{DateTime.Now:HH:mm:ss}] {mensaje}");
        }

        public void LogWarning(string mensaje)
        {
            Escribir($"[WARN] [{DateTime.Now:HH:mm:ss}] {mensaje}");
        }

        public void LogError(string mensaje, Exception ex = null)
        {
            var detalle = ex != null ? $" | Excepcion: {ex.Message} | StackTrace: {ex.StackTrace}" : "";
            Escribir($"[ERROR] [{DateTime.Now:HH:mm:ss}] {mensaje}{detalle}");
        }

        private void Escribir(string linea)
        {
            File.AppendAllLines(_logPath, new[] { linea });
        }
    }
}