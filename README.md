# CameraApp - Aplicativo MAUI para Captura de Fotos

Este é um aplicativo .NET MAUI que permite tirar fotos usando a câmera do dispositivo ou selecionar imagens da galeria.

## Funcionalidades

- ✅ Tirar fotos usando a câmera
- ✅ Selecionar fotos da galeria
- ✅ Visualizar a foto selecionada/capturada
- ✅ Limpar a foto atual
- ✅ Interface responsiva e intuitiva

## Arquitetura

O projeto segue as melhores práticas do .NET MAUI:

### Estrutura de Pastas
```
├── Views/          # Páginas XAML
├── ViewModels/     # ViewModels com MVVM
├── Services/       # Serviços da aplicação
├── Models/         # Modelos de dados
├── Converters/     # Conversores para XAML
└── Platforms/      # Código específico por plataforma
    ├── Android/
    └── iOS/
```

### Padrões Utilizados
- **MVVM** com CommunityToolkit.Mvvm
- **Dependency Injection** nativo do .NET
- **Data Binding** para interface reativa
- **Command Pattern** para ações de UI

## Tecnologias

- .NET 9
- .NET MAUI
- CommunityToolkit.Mvvm
- CommunityToolkit.Maui

## Plataformas Suportadas

- Android (API 21+)
- iOS (15.0+)

## Como Executar

### Pré-requisitos
- Visual Studio 2022 ou .NET CLI
- Workload do .NET MAUI instalado

### Compilar e Executar

#### Android
```bash
dotnet build -t:Run -f net9.0-android
```

#### iOS (apenas no macOS)
```bash
dotnet build -t:Run -f net9.0-ios
```

## Permissões

### Android
- `android.permission.CAMERA` - Para acessar a câmera
- `android.permission.READ_EXTERNAL_STORAGE` - Para ler da galeria
- `android.permission.WRITE_EXTERNAL_STORAGE` - Para salvar imagens

### iOS
- `NSCameraUsageDescription` - Para acessar a câmera
- `NSPhotoLibraryUsageDescription` - Para acessar a galeria

## Uso da Interface

1. **Tirar Foto**: Toque no botão "Tirar Foto" para abrir a câmera
2. **Galeria**: Toque no botão "Galeria" para selecionar uma foto existente
3. **Visualizar**: A foto será exibida na tela após captura/seleção
4. **Limpar**: Use o botão "Limpar Foto" para remover a imagem atual

## Estrutura do Código

### CameraService
Serviço responsável por:
- Capturar fotos da câmera
- Selecionar fotos da galeria
- Gerenciar permissões
- Tratamento de erros

### CameraPageViewModel
ViewModel que gerencia:
- Estado da foto atual
- Comandos para ações da UI
- Binding com a interface

### CameraPage
Interface XAML com:
- Botões para ações
- Área de visualização da foto
- Layout responsivo

## Melhorias Futuras

- [ ] Suporte para vídeos
- [ ] Filtros de imagem
- [ ] Compartilhamento de fotos
- [ ] Armazenamento em nuvem
- [ ] Geolocalização das fotos

## Contribuição

Este projeto segue as melhores práticas de desenvolvimento .NET MAUI. Para contribuir:

1. Mantenha o padrão MVVM
2. Use injeção de dependência
3. Implemente tratamento de erros
4. Teste em múltiplas plataformas
5. Documente mudanças no código
