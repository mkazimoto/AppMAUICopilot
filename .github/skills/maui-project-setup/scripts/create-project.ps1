param(
    [Parameter(Mandatory = $true)]
    [string]$Name
)

dotnet new maui -n $Name

Set-Location $Name

# Create folder structure
New-Item -ItemType Directory -Force -Path "Views","Views/Templates","Views/Controls","ViewModels","Services","Services/Interfaces","Models"

# Add base ViewModel
@"
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace $Name.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
"@ | Out-File "ViewModels/BaseViewModel.cs"