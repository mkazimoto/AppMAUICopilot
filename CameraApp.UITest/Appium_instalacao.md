# Configuração do Appium para Testes de UI

Este guia descreve os passos para instalar e configurar o Appium para executar os testes de UI do projeto **CameraApp**.

---

## Pré-requisitos

- [Node.js](https://nodejs.org/) v18 ou superior (inclui `npm`)
- [Java JDK](https://adoptium.net/) 11 ou superior (necessário para Android)
- [Android Studio](https://developer.android.com/studio) com Android SDK instalado
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Dispositivo físico Android conectado via USB **ou** emulador Android em execução

---

## 1. Instalar o Appium

Instale o Appium globalmente via npm:

```bash
npm install -g appium
```

Verifique a instalação:

```bash
appium --version
```

---

## 2. Instalar o Driver UIAutomator2

O driver UIAutomator2 é necessário para automação Android:

```bash
appium driver install uiautomator2
```

Confirme a instalação:

```bash
appium driver list --installed
```

---

## 3. Configurar Variáveis de Ambiente

Adicione as seguintes variáveis de ambiente ao seu sistema:

| Variável         | Exemplo de Valor                          |
|------------------|-------------------------------------------|
| `JAVA_HOME`      | `C:\Program Files\Eclipse Adoptium\jdk-21`|
| `ANDROID_HOME`   | `C:\Users\<usuario>\AppData\Local\Android\Sdk` |

Adicione ao `PATH`:
- `%JAVA_HOME%\bin`
- `%ANDROID_HOME%\platform-tools`
- `%ANDROID_HOME%\tools`

---

## 4. Verificar Dependências com Appium Doctor

Instale o `appium-doctor` para diagnosticar possíveis problemas:

```bash
npm install -g @appium/doctor
appium-doctor --android
```

Corrija todos os itens marcados com ❌ antes de continuar.

---

## 5. Iniciar o Servidor Appium

Inicie o servidor Appium localmente (porta padrão `4723`):

```bash
appium
```

O servidor estará disponível em: `http://localhost:4723/`

> Este endereço é o configurado em `AppiumSetup.cs` (`AppiumServerUri`).

---

## 6. Compilar o APK do CameraApp

Antes de executar os testes, gere o APK de debug do projeto:

```bash
dotnet build -f net10.0-android ../CameraApp/CameraApp.csproj
```

O APK gerado será utilizado automaticamente pelo `AppiumFixture`:

```
CameraApp\bin\Debug\net10.0-android\com.companyname.cameraapp-Signed.apk
```

---

## 7. Executar os Testes de UI

Com o servidor Appium rodando e um dispositivo/emulador disponível, execute os testes:

```bash
dotnet test CameraApp.UITest.csproj
```

Ou pelo Visual Studio / VS Code, via Test Explorer.

---

## Dicas

- Use `adb devices` para listar dispositivos Android conectados.
- Para usar um emulador, inicie-o pelo Android Studio **antes** de executar os testes.
- O parâmetro `appium:noReset = false` em `AppiumSetup.cs` reinstala o app a cada execução — mude para `true` para acelerar execuções repetidas em desenvolvimento.

---

## Referências

- [Appium Docs](https://appium.io/docs/en/latest/)
- [UIAutomator2 Driver](https://github.com/appium/appium-uiautomator2-driver)
- [Appium.WebDriver NuGet (v5)](https://www.nuget.org/packages/Appium.WebDriver)
