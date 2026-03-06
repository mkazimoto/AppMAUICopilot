
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices.Sensors;

namespace CameraApp.Services
{
    /// <summary>
    /// Monitors device posture in real time using the device accelerometer and raises alerts when poor posture is detected.
    /// </summary>
    public class PostureService : IPostureService
    {
        private readonly IAccelerometer _accelerometer;
        private readonly IVibration _vibration;
        private readonly Func<DateTime> _clock;
        private readonly ILogger<PostureService>? _logger;

        private Timer? _monitoringTimer;
        private DateTime _lastPoorPostureTime;
        private bool _isInPoorPosture;
        private Vector3 _lastReading;
        private const int MonitoringIntervalMs = 500;

        /// <inheritdoc/>
        public event EventHandler<PostureAlertEventArgs>? PostureAlert;
        /// <inheritdoc/>
        public event EventHandler<AccelerometerDataEventArgs>? AccelerometerDataUpdated;

        /// <inheritdoc/>
        public bool IsMonitoring { get; private set; }
        /// <inheritdoc/>
        public double Sensitivity { get; set; } = 0.3;
        /// <inheritdoc/>
        public int AlertDelaySeconds { get; set; } = 5;

        /// <summary>
        /// Initializes a new instance of <see cref="PostureService" /> using the default MAUI implementations.
        /// </summary>
        public PostureService()
            : this(Accelerometer.Default, Vibration.Default, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PostureService" /> with injectable dependencies.
        /// </summary>
        /// <param name="accelerometer">The accelerometer sensor provider.</param>
        /// <param name="vibration">The vibration provider.</param>
        /// <param name="logger">The logger instance for structured logging.</param>
        /// <param name="clock">A delegate that returns the current date and time; defaults to <see cref="DateTime.Now" />.</param>
        public PostureService(IAccelerometer accelerometer, IVibration vibration, ILogger<PostureService>? logger = null, Func<DateTime>? clock = null)
        {
            _accelerometer = accelerometer;
            _vibration = vibration;
            _logger = logger;
            _clock = clock ?? (() => DateTime.Now);
        }

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">The device accelerometer is not available.</exception>
        public async Task StartMonitoringAsync()
        {
            if (IsMonitoring)
                return;

            try
            {
                if (!_accelerometer.IsSupported)
                {
                    _logger?.LogWarning("Acelerômetro não está disponível neste dispositivo");
                    throw new NotSupportedException("Acelerômetro não está disponível neste dispositivo.");
                }

                if (!_accelerometer.IsMonitoring)
                {
                    _accelerometer.ReadingChanged += OnAccelerometerReadingChanged;
                    _accelerometer.Start(SensorSpeed.Default);
                }

                IsMonitoring = true;
                _isInPoorPosture = false;
                _lastPoorPostureTime = DateTime.MinValue;

                _monitoringTimer = new Timer(CheckPostureStatus, null, 0, MonitoringIntervalMs);

                _logger?.LogInformation("Monitoramento de postura iniciado");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao iniciar monitoramento de postura");
                throw new InvalidOperationException($"Erro ao iniciar monitoramento: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public void StopMonitoring()
        {
            if (!IsMonitoring)
                return;

            IsMonitoring = false;
            _monitoringTimer?.Dispose();
            _monitoringTimer = null;

            if (_accelerometer.IsMonitoring)
            {
                _accelerometer.ReadingChanged -= OnAccelerometerReadingChanged;
                _accelerometer.Stop();
            }

            _logger?.LogInformation("Monitoramento de postura parado");
        }

        private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
        {
            _lastReading = e.Reading.Acceleration;
        }

        private void CheckPostureStatus(object? state)
        {
            if (_lastReading == Vector3.Zero || !IsMonitoring)
                return;

            ProcessReading(_lastReading);
        }

        /// <summary>
        /// Processes a single accelerometer reading: computes inclination, raises <see cref="AccelerometerDataUpdated" />,
        /// and evaluates whether a posture alert should be raised.
        /// </summary>
        internal void ProcessReading(Vector3 reading)
        {
            var inclination = CalculateInclination(reading.X, reading.Y, reading.Z);
            var status = DeterminePostureStatus(inclination);

            AccelerometerDataUpdated?.Invoke(this, new AccelerometerDataEventArgs
            {
                X = reading.X,
                Y = reading.Y,
                Z = reading.Z,
                Inclination = inclination,
                Status = status
            });

            HandlePostureAlert(status, inclination);
        }

        /// <summary>
        /// Calculates the inclination angle (in degrees) of the device relative to vertical
        /// using the formula <c>atan2(sqrt(x² + z²), |y|)</c>.
        /// </summary>
        /// <returns>Angle in degrees; returns <c>90</c> when <paramref name="y" /> is zero (device fully horizontal).</returns>
        internal double CalculateInclination(double x, double y, double z)
        {
            var horizontalComponent = Math.Sqrt(x * x + z * z);
            var verticalComponent = Math.Abs(y);

            if (verticalComponent == 0)
                return 90;

            var angleRadians = Math.Atan2(horizontalComponent, verticalComponent);
            return angleRadians * (180.0 / Math.PI);
        }

        /// <summary>
        /// Determines the <see cref="PostureStatus" /> for a given inclination angle,
        /// adjusted by the current <see cref="Sensitivity" /> value.
        /// </summary>
        internal PostureStatus DeterminePostureStatus(double inclination)
        {
            var goodThreshold = 15 * (1 - Sensitivity);
            var warningThreshold = 30 * (1 - Sensitivity * 0.5);

            if (inclination <= goodThreshold)
                return PostureStatus.Good;

            if (inclination <= warningThreshold)
                return PostureStatus.Warning;

            return PostureStatus.Poor;
        }

        private async void HandlePostureAlert(PostureStatus status, double inclination)
        {
            try
            {
                var now = _clock();

                if (status == PostureStatus.Poor)
                {
                    if (!_isInPoorPosture)
                    {
                        _isInPoorPosture = true;
                        _lastPoorPostureTime = now;
                    }
                    else if ((now - _lastPoorPostureTime).TotalSeconds >= AlertDelaySeconds)
                    {
                        TriggerAlert("Postura incorreta detectada! Endireite as costas.", status, inclination);
                        _lastPoorPostureTime = now;
                    }
                }
                else
                {
                    _isInPoorPosture = false;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao processar alerta de postura");
            }

            await Task.CompletedTask;
        }

        private void TriggerAlert(string message, PostureStatus status, double inclination)
        {
            try
            {
                if (_vibration.IsSupported)
                {
                    var duration = status == PostureStatus.Poor
                        ? TimeSpan.FromMilliseconds(500)
                        : TimeSpan.FromMilliseconds(200);

                    _vibration.Vibrate(duration);
                }

                var alertMessage = $"{message} (Inclinação: {inclination:F1}°)";
                _logger?.LogWarning("Alerta de postura: {Message}, Status: {Status}, Inclinação: {Inclination:F1}°", 
                    message, status, inclination);

                PostureAlert?.Invoke(this, new PostureAlertEventArgs
                {
                    Message = alertMessage,
                    Status = status
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao disparar alerta de postura");
            }
        }
    }
}