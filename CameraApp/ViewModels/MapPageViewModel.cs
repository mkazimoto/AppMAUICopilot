using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CameraApp.Services;
using System.Globalization;

namespace CameraApp.ViewModels;

/// <summary>
/// Provides properties and commands for displaying the device's current location on a map.
/// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="MapPageViewModel" /> class and loads the default map view.
    /// </summary>
    /// <param name="locationService">The location service used to retrieve the device's geographic position.</param>
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

                var loc = LocalizationResourceManager.Instance;
                await (Shell.Current?.DisplayAlertAsync(
                    loc["Common_Error"].ToString()!,
                    loc["Map_LocationError"].ToString()!,
                    loc["Common_Ok"].ToString()!) ?? Task.CompletedTask);
            }
        }
        catch (Exception ex)
        {
            HasLocation = false;
            var loc = LocalizationResourceManager.Instance;
            await (Shell.Current?.DisplayAlertAsync(
                loc["Common_Error"].ToString()!,
                $"{loc["Map_LocationFetchError"]} {ex.Message}",
                loc["Common_Ok"].ToString()!) ?? Task.CompletedTask);
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