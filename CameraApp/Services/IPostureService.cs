using System;
using System.Threading.Tasks;

namespace CameraApp.Services
{
    /// <summary>
    /// Defines operations for monitoring device posture using the accelerometer.
    /// </summary>
    public interface IPostureService
    {
        /// <summary>
        /// Occurs when an incorrect posture is detected.
        /// </summary>
        event EventHandler<PostureAlertEventArgs> PostureAlert;

        /// <summary>
        /// Occurs when the accelerometer sensor data is updated.
        /// </summary>
        event EventHandler<AccelerometerDataEventArgs> AccelerometerDataUpdated;

        /// <summary>
        /// Starts the posture monitoring loop.
        /// </summary>
        Task StartMonitoringAsync();

        /// <summary>
        /// Stops the posture monitoring loop.
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// Gets a value that indicates whether posture monitoring is currently active.
        /// </summary>
        /// <value><see langword="true" /> if monitoring is running; otherwise, <see langword="false" />.</value>
        bool IsMonitoring { get; }

        /// <summary>
        /// Gets or sets the posture detection sensitivity threshold.
        /// </summary>
        /// <value>A value between <c>0.0</c> (least sensitive) and <c>1.0</c> (most sensitive).</value>
        double Sensitivity { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds to wait before raising a posture alert.
        /// </summary>
        /// <value>The alert delay in seconds.</value>
        int AlertDelaySeconds { get; set; }
    }

    /// <summary>
    /// Provides data for the <see cref="IPostureService.PostureAlert" /> event.
    /// </summary>
    public class PostureAlertEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the alert message describing the posture issue.
        /// </summary>
        /// <value>A human-readable alert message. The default is an empty string.</value>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the posture status at the time of the alert.
        /// </summary>
        /// <value>One of the <see cref="PostureStatus" /> values.</value>
        public PostureStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the alert was raised.
        /// </summary>
        /// <value>The date and time of the alert. The default is <see cref="DateTime.Now" /> at the time of creation.</value>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Provides data for the <see cref="IPostureService.AccelerometerDataUpdated" /> event.
    /// </summary>
    public class AccelerometerDataEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the accelerometer reading along the X axis.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the accelerometer reading along the Y axis.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the accelerometer reading along the Z axis.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Gets or sets the computed device inclination angle in degrees.
        /// </summary>
        public double Inclination { get; set; }

        /// <summary>
        /// Gets or sets the posture status derived from the current accelerometer data.
        /// </summary>
        /// <value>One of the <see cref="PostureStatus" /> values.</value>
        public PostureStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the accelerometer reading.
        /// </summary>
        /// <value>The date and time of the reading. The default is <see cref="DateTime.Now" /> at the time of creation.</value>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Specifies the posture quality detected by the posture monitoring service.
    /// </summary>
    public enum PostureStatus
    {
        /// <summary>The posture is correct.</summary>
        Good,
        /// <summary>The posture shows a minor deviation.</summary>
        Warning,
        /// <summary>The posture shows a significant deviation.</summary>
        Poor
    }
}