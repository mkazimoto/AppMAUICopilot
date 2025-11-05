using System;
using System.Threading.Tasks;

namespace CameraApp.Services
{
    public interface IPostureService
    {
        /// <summary>
        /// Evento disparado quando a postura está incorreta
        /// </summary>
        event EventHandler<PostureAlertEventArgs> PostureAlert;

        /// <summary>
        /// Evento disparado quando os dados do acelerômetro são atualizados
        /// </summary>
        event EventHandler<AccelerometerDataEventArgs> AccelerometerDataUpdated;

        /// <summary>
        /// Inicia o monitoramento da postura
        /// </summary>
        Task StartMonitoringAsync();

        /// <summary>
        /// Para o monitoramento da postura
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// Indica se o monitoramento está ativo
        /// </summary>
        bool IsMonitoring { get; }

        /// <summary>
        /// Define a sensibilidade do detector de postura (0.0 a 1.0)
        /// </summary>
        double Sensitivity { get; set; }

        /// <summary>
        /// Tempo em segundos para aguardar antes de disparar alerta
        /// </summary>
        int AlertDelaySeconds { get; set; }
    }

    public class PostureAlertEventArgs : EventArgs
    {
        public string Message { get; set; } = string.Empty;
        public PostureStatus Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class AccelerometerDataEventArgs : EventArgs
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Inclination { get; set; }
        public PostureStatus Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public enum PostureStatus
    {
        Good,
        Warning,
        Poor
    }
}