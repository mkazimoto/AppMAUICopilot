using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CameraApp.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace CameraApp.ViewModels
{
    public partial class PosturePageViewModel : ObservableObject
    {
        private readonly IPostureService _postureService;

        public PosturePageViewModel(IPostureService postureService)
        {
            _postureService = postureService;
            _postureService.PostureAlert += OnPostureAlert;
            _postureService.AccelerometerDataUpdated += OnAccelerometerDataUpdated;
            
            // Valores iniciais
            Sensitivity = _postureService.Sensitivity;
            AlertDelay = _postureService.AlertDelaySeconds;
        }

        [ObservableProperty]
        private bool isMonitoring;

        [ObservableProperty]
        private string statusMessage = "Pronto para monitorar";

        [ObservableProperty]
        private string postureStatus = "Desconhecido";

        [ObservableProperty]
        private double accelerometerX;

        [ObservableProperty]
        private double accelerometerY;

        [ObservableProperty]
        private double accelerometerZ;

        [ObservableProperty]
        private double inclination;

        [ObservableProperty]
        private Color statusColor = Colors.Gray;

        [ObservableProperty]
        private double sensitivity = 0.3;

        [ObservableProperty]
        private int alertDelay = 5;

        [ObservableProperty]
        private string lastAlertMessage = "";

        [ObservableProperty]
        private DateTime lastAlertTime;

        [ObservableProperty]
        private int alertCount;

        [RelayCommand]
        private async Task StartMonitoringAsync()
        {
            try
            {
                // Atualizar configurações do serviço
                _postureService.Sensitivity = Sensitivity;
                _postureService.AlertDelaySeconds = AlertDelay;

                await _postureService.StartMonitoringAsync();
                IsMonitoring = true;
                StatusMessage = "Monitoramento ativo";
                PostureStatus = "Aguardando dados...";
                StatusColor = Colors.Blue;
                AlertCount = 0;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro: {ex.Message}";
                StatusColor = Colors.Red;
                
                // Mostrar alerta ao usuário
                if (Application.Current?.Windows?.Count > 0)
                {
                    var mainPage = Application.Current.Windows[0].Page;
                    if (mainPage != null)
                    {
                        await mainPage.DisplayAlert(
                            "Erro", 
                            $"Não foi possível iniciar o monitoramento:\n{ex.Message}", 
                            "OK");
                    }
                }
            }
        }

        [RelayCommand]
        private void StopMonitoring()
        {
            _postureService.StopMonitoring();
            IsMonitoring = false;
            StatusMessage = "Monitoramento parado";
            PostureStatus = "Desconhecido";
            StatusColor = Colors.Gray;
            
            // Resetar valores
            AccelerometerX = 0;
            AccelerometerY = 0;
            AccelerometerZ = 0;
            Inclination = 0;
        }

        [RelayCommand]
        private void ResetStats()
        {
            AlertCount = 0;
            LastAlertMessage = "";
            LastAlertTime = DateTime.MinValue;
        }

        private void OnAccelerometerDataUpdated(object? sender, AccelerometerDataEventArgs e)
        {
            // Atualizar na thread principal
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AccelerometerX = e.X;
                AccelerometerY = e.Y;
                AccelerometerZ = e.Z;
                Inclination = e.Inclination;
                
                // Atualizar status da postura
                PostureStatus = e.Status switch
                {
                    Services.PostureStatus.Good => "Boa Postura",
                    Services.PostureStatus.Warning => "Atenção",
                    Services.PostureStatus.Poor => "Postura Ruim",
                    _ => "Desconhecido"
                };

                // Atualizar cor do status
                StatusColor = e.Status switch
                {
                    Services.PostureStatus.Good => Colors.Green,
                    Services.PostureStatus.Warning => Colors.Orange,
                    Services.PostureStatus.Poor => Colors.Red,
                    _ => Colors.Gray
                };
            });
        }

        private void OnPostureAlert(object? sender, PostureAlertEventArgs e)
        {
            // Atualizar na thread principal
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LastAlertMessage = e.Message;
                LastAlertTime = e.Timestamp;
                AlertCount++;
                StatusMessage = $"Alerta: {e.Message}";
            });
        }

        partial void OnSensitivityChanged(double value)
        {
            if (_postureService.IsMonitoring)
            {
                _postureService.Sensitivity = value;
            }
        }

        partial void OnAlertDelayChanged(int value)
        {
            if (_postureService.IsMonitoring)
            {
                _postureService.AlertDelaySeconds = value;
            }
        }
    }
}