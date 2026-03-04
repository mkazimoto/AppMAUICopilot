# Folder Structure Example

## Overview

This example shows a fully populated MAUI project using the recommended structure.

```code
[root]
├─ Dependencies
│  └─ [The default Dependencies folder created by the MAUI template]
├─ Properties
│  └─ launchSettings.json
├─ Models
│  └─ [Add model classes here]
├─ Platforms
│  ├─ Android
│  │  ├─ Resources
│  │  │  └─ values
│  │  │     └─ colors.xml
│  │  ├─ AndroidManifest.xml
│  │  ├─ MainActivity.cs
│  │  └─ MainApplication.cs
│  ├─ iOS
│  │  ├─ Resources
│  │  │  └─ PrivacyInfo.xcprivacy
│  │  ├─ AppDelegate.cs
│  │  ├─ Info.plist
│  │  └─ Program.cs
│  ├─ MacCatalyst
│  │  ├─ AppDelegate.cs
│  │  ├─ Entitlements.plist
│  │  ├─ Info.plist
│  │  └─ Program.cs
│  └─ Windows
│     ├─ app.manifest
│     ├─ App.xaml
│     │  └─ App.xaml.cs
│     └─ Package.appxmanifest
├─ Resources
│  ├─ AppIcon
│  │  ├─ appicon.svg
│  │  └─ appiconfg.svg
│  ├─ Fonts
│  │  ├─ OpenSans-Regular.ttf
│  │  └─ OpenSans-Semibold.ttf
│  ├─ Images
│  │  └─ dotnet_bot.png
│  ├─ raw
│  │  └─ AboutAssets.txt
│  ├─ Splash
│  │  └─ splash.svg
│  └─ Styles
│     ├─ Colors.xaml
│     └─ Styles.xaml
├─ Services
│  ├─ Interfaces
│  │  └─ [Add service interfaces here]
│  └─ [Add service classes here]
├─ ViewModels
│  ├─ MainViewModel.cs
│  └─ [Add other ViewModels here]
├─ Views
│  ├─ Controls
│  │  └─ [Add custom controls here]
│  ├─ MainPage.xaml
│  │  └─ MainPage.xaml.cs
│  └─ Templates
│    └─ [Add custom templates here]
├─ App.xaml
│  └─ App.xaml.cs
└─ MauiProgram.cs
```
