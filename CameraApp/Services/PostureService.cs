
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace CameraApp.Services
{
    public class PostureService : IPostureService
    {
        private Timer? _monitoringTimer;
        private DateTime _lastPoorPostureTime;
        private bool _isInPoorPosture;
        private const int MonitoringIntervalMs = 500; // Monitora a cada 500ms

        public event EventHandler<PostureAlertEventArgs>? PostureAlert;
        public event EventHandler<AccelerometerDataEventArgs>? AccelerometerDataUpdated;

        public bool IsMonitoring { get; private set; }
        public double Sensitivity { get; set; } = 0.3; // 30% de tolerância
        public int AlertDelaySeconds { get; set; } = 5; // Alerta após 5 segundos

        public async Task StartMonitoringAsync()
        {
            if (IsMonitoring)
                return;

            try
            {
                // Verificar se o acelerômetro está disponível
                if (!Accelerometer.Default.IsSupported)
                {
                    throw new NotSupportedException("Acelerômetro não está disponível neste dispositivo.");
                }

                // Iniciar o acelerômetro
                if (!Accelerometer.Default.IsMonitoring)
                {
                    Accelerometer.Default.ReadingChanged += OnAccelerometerReadingChanged;
                    Accelerometer.Default.Start(SensorSpeed.Default);
                }

                IsMonitoring = true;
                _isInPoorPosture = false;
                _lastPoorPostureTime = DateTime.MinValue;

                // Iniciar timer de monitoramento
                _monitoringTimer = new Timer(CheckPostureStatus, null, 0, MonitoringIntervalMs);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao iniciar monitoramento: {ex.Message}", ex);
            }
        }

        public void StopMonitoring()
        {
            if (!IsMonitoring)
                return;

            IsMonitoring = false;
            _monitoringTimer?.Dispose();
            _monitoringTimer = null;

            if (Accelerometer.Default.IsMonitoring)
            {
                Accelerometer.Default.ReadingChanged -= OnAccelerometerReadingChanged;
                Accelerometer.Default.Stop();
            }
        }

        private Vector3 _lastReading;

        private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
        {
            _lastReading = e.Reading.Acceleration;
        }

        private void CheckPostureStatus(object? state)
        {
            if (_lastReading == Vector3.Zero || !IsMonitoring)
                return;

            var reading = _lastReading;
            
            // Calcular a inclinação baseada nos valores do acelerômetro
            // Para um celular no bolso da camisa, quando a pessoa está ereta:
            // - X deve estar próximo de 0 (sem inclinação lateral)
            // - Y deve ser negativo (apontando para baixo devido à gravidade)
            // - Z deve estar próximo de 0 (sem inclinação frontal/traseira)
            
            var inclination = CalculateInclination(reading.X, reading.Y, reading.Z);
            var status = DeterminePostureStatus(inclination);

            // Disparar evento de atualização dos dados
            AccelerometerDataUpdated?.Invoke(this, new AccelerometerDataEventArgs
            {
                X = reading.X,
                Y = reading.Y,
                Z = reading.Z,
                Inclination = inclination,
                Status = status
            });

            // Verificar se precisa disparar alerta
            HandlePostureAlert(status, inclination);
        }

        private double CalculateInclination(double x, double y, double z)
        {
            // Calcular o ângulo de inclinação em relação à vertical
            // Usando a fórmula: atan2(sqrt(x² + z²), |y|) * (180/π)
            var horizontalComponent = Math.Sqrt(x * x + z * z);
            var verticalComponent = Math.Abs(y);
            
            if (verticalComponent == 0)
                return 90; // Completamente horizontal
            
            var angleRadians = Math.Atan2(horizontalComponent, verticalComponent);
            var angleDegrees = angleRadians * (180.0 / Math.PI);
            
            return angleDegrees;
        }

        private PostureStatus DeterminePostureStatus(double inclination)
        {
            // Definir limiares baseados na sensibilidade
            var goodPostureThreshold = 15 * (1 - Sensitivity); // Menos sensível = mais tolerante
            var warningThreshold = 30 * (1 - Sensitivity * 0.5);

            if (inclination <= goodPostureThreshold)
                return PostureStatus.Good;
            else if (inclination <= warningThreshold)
                return PostureStatus.Warning;
            else
                return PostureStatus.Poor;
        }

        private async void HandlePostureAlert(PostureStatus status, double inclination)
        {
            var now = DateTime.Now;

            if (status == PostureStatus.Poor)
            {
                if (!_isInPoorPosture)
                {
                    _isInPoorPosture = true;
                    _lastPoorPostureTime = now;
                }
                else if ((now - _lastPoorPostureTime).TotalSeconds >= AlertDelaySeconds)
                {
                    // Disparar alerta e vibração
                    await TriggerAlert("Postura incorreta detectada! Endireite as costas.", status, inclination);
                    _lastPoorPostureTime = now; // Resetar para próximo alerta
                }
            }
            else
            {
                _isInPoorPosture = false;
            }
        }

        private async Task TriggerAlert(string message, PostureStatus status, double inclination)
        {
            try
            {
                // Vibrar o dispositivo
                if (Vibration.Default.IsSupported)
                {
                    var vibrationPattern = status == PostureStatus.Poor ? 
                        TimeSpan.FromMilliseconds(500) : 
                        TimeSpan.FromMilliseconds(200);
                    
                    Vibration.Default.Vibrate(vibrationPattern);
                }

                // Disparar evento de alerta
                PostureAlert?.Invoke(this, new PostureAlertEventArgs
                {
                    Message = $"{message} (Inclinação: {inclination:F1}°)",
                    Status = status
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao disparar alerta: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}