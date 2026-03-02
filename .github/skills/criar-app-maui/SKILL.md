---
name: criar-app-maui
description: Cria um novo projeto MAUI com configuração básica.
---

# Criação do Projeto

* Crie a estrutura base do projeto com: 
```
dotnet new maui -n [ProjectName]
```

* Centralize as versões do .NET e do MAUI em `Directory.Build.props` para fácil manutenção:
```
<Project>
  <PropertyGroup>
    <NetVersion>10.0</NetVersion>
    <MauiVersion>10.0.41</MauiVersion>
  </PropertyGroup>
</Project>
```

* Altere o target framework para Net (testes unitários), Android e iOS:
```
<TargetFrameworks>net$(NetVersion);net$(NetVersion)-android;net$(NetVersion)-ios</TargetFrameworks>
```

* Mantenha apenas as pastas Android e IOS:
	- Platforms/Android
	- Platforms/iOS

* Instale os seguintes pacotes:
	- CommunityToolkit.Mvvm
	- CommunityToolkit.Maui

## Estrutura do Projeto

- Mantenha a separação clara entre lógica de negócio (ViewModels/Services) e interface (Views).
- Utilize o padrão MVVM para facilitar testes, manutenção e reutilização de código.
- Organize arquivos em pastas como `Views`, `ViewModels`, `Models`, `Services` e `Resources`.

## Recursos e Internacionalização

- Armazene imagens em `Resources/Images` e fontes em `Resources/Fonts`.
- Use arquivos `.resx` para suporte a múltiplos idiomas.

## Navegação

- Use o `Shell` do MAUI para navegação e gerenciamento de rotas.
- Defina rotas nomeadas para facilitar a navegação entre páginas.

## Serviços e Injeção de Dependência

- Registre serviços no `MauiProgram.cs` usando o container de DI padrão.
- Prefira interfaces para abstrair serviços e facilitar testes.

## Build
- Sempre confira a existencia de todos os StaticResource antes do build
- Faça o build apenas para o android:

```
dotnet build -t:Run -f net9.0-android
```

## Boas Práticas Gerais

- Evite lógica de negócio no code-behind das Views.
- Implemente comandos (`ICommand`) para ações de UI.
- Utilize `ObservableCollection` para listas dinâmicas.
- Prefira métodos assíncronos (`async/await`) para operações de I/O.
- Teste em múltiplas plataformas (Android, iOS).

## Performance

- Use `CollectionView` em vez de `ListView` para melhor desempenho.
- Carregue imagens de forma assíncrona.
- Minimize o uso de layouts aninhados.

## Segurança

- Nunca armazene segredos ou chaves diretamente no código.
- Use armazenamento seguro (Secure Storage) para dados sensíveis.

---

Siga estas práticas para garantir aplicativos MAUI robustos, escaláveis e fáceis de manter.