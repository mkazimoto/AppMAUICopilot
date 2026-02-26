---
name: criar-app-maui
description: Cria um novo projeto MAUI com configuração básica.
---

# Criação do Projeto

* Crie a estrutura base do projeto com: 
```
dotnet new maui -n MeuAppMaui
```

* Altere o target framework para Net 9 (testes unitários), Android e iOS:
```
<TargetFrameworks>net9.0;net9.0-android;net9.0-ios</TargetFrameworks>
```		

* Mantenha apenas as pastas Android e IOS:
	- Platforms/Android
	- Platforms/iOS

* Instale os seguintes pacotes:
	- CommunityToolkit.Mvvm
	- CommunityToolkit.Maui