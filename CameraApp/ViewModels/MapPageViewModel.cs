using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CameraApp.Services;
using System.Globalization;

namespace CameraApp.ViewModels;

public partial class MapPageViewModel : ObservableObject
{
    private readonly ILocationService _locationService;
    
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private bool hasLocation;
    
    [ObservableProperty]
    private string locationText = string.Empty;
    
    [ObservableProperty]
    private string lastUpdateText = string.Empty;
    
    [ObservableProperty]
    private string mapUrl = "about:blank";
    
    private double _currentLatitude;
    private double _currentLongitude;

    public MapPageViewModel(ILocationService locationService)
    {
        _locationService = locationService;
        GenerateDefaultMap();
    }

    [RelayCommand]
    private async Task GetLocationAsync()
    {
        try
        {
            IsLoading = true;
            
            var location = await _locationService.GetCurrentLocationAsync();
            
            if (location != null)
            {
                _currentLatitude = location.Latitude;
                _currentLongitude = location.Longitude;
                
                HasLocation = true;
                LocationText = $"Lat: {location.Latitude:F6}, Lng: {location.Longitude:F6}";
                LastUpdateText = $"Atualizado em: {DateTime.Now:HH:mm:ss}";
                
                UpdateMapWithLocation(location.Latitude, location.Longitude);
            }
            else
            {
                HasLocation = false;
                LocationText = string.Empty;
                LastUpdateText = string.Empty;
                
                await Shell.Current.DisplayAlert(
                    "Erro", 
                    "Não foi possível obter sua localização. Verifique se as permissões estão habilitadas e se o GPS está ativo.", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            HasLocation = false;
            await Shell.Current.DisplayAlert(
                "Erro", 
                $"Erro ao obter localização: {ex.Message}", 
                "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ResetMap()
    {
        if (HasLocation)
        {
            UpdateMapWithLocation(_currentLatitude, _currentLongitude);
        }
        else
        {
            GenerateDefaultMap();
        }
    }

    private void UpdateMapWithLocation(double latitude, double longitude)
    {
        var lat = latitude.ToString("F6", CultureInfo.InvariantCulture);
        var lng = longitude.ToString("F6", CultureInfo.InvariantCulture);
        
        var html = GenerateMapHtml(lat, lng);
        MapUrl = $"data:text/html;charset=utf-8,{Uri.EscapeDataString(html)}";
    }

    private void GenerateDefaultMap()
    {
        // Mapa padrão centrado no Brasil
        var html = GenerateMapHtml("-14.2350", "-51.9253", 4);
        MapUrl = $"data:text/html;charset=utf-8,{Uri.EscapeDataString(html)}";
    }

    private static string GenerateMapHtml(string latitude, string longitude, int zoom = 15)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Mapa OpenStreetMap</title>
    <link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"" />
    <style>
        body {{ margin: 0; padding: 0; }}
        #map {{ height: 100vh; width: 100%; }}
        .custom-marker {{
            background-color: #ff4444;
            border: 3px solid #ffffff;
            border-radius: 50%;
            width: 20px;
            height: 20px;
        }}
    </style>
</head>
<body>
    <div id=""map""></div>
    
    <script src=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.js""></script>
    <script>
        // Inicializa o mapa
        var map = L.map('map').setView([{latitude}, {longitude}], {zoom});
        
        // Adiciona tile layer do OpenStreetMap
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            attribution: '© <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors',
            maxZoom: 19
        }}).addTo(map);
        
        // Adiciona marcador na localização atual (se não for o mapa padrão)
        {(zoom == 15 ? $@"
        var marker = L.marker([{latitude}, {longitude}]).addTo(map);
        marker.bindPopup('<b>Sua Localização</b><br>Lat: {latitude}<br>Lng: {longitude}').openPopup();
        
        // Adiciona círculo de precisão
        var circle = L.circle([{latitude}, {longitude}], {{
            color: '#ff4444',
            fillColor: '#ff4444',
            fillOpacity: 0.2,
            radius: 100
        }}).addTo(map);
        " : "")}
        
        // Tenta ajustar o zoom para mostrar ambos o marcador e círculo
        {(zoom == 15 ? "if (typeof circle !== 'undefined') { map.fitBounds(circle.getBounds()); }" : "")}
    </script>
</body>
</html>";
    }
}