# Monitor de Postura - CameraApp

Esta funcionalidade foi adicionada ao aplicativo CameraApp para monitorar a postura da coluna quando o celular estiver no bolso da camisa, utilizando o acelerômetro do dispositivo.

## 📱 Como Usar

### 1. Preparação
- Coloque o celular no bolso da camisa com a **tela voltada para o peito**
- Certifique-se de que o celular está em posição vertical
- O aplicativo deve ter permissão para usar sensores e vibração

### 2. Iniciando o Monitoramento
1. Abra o aplicativo e navegue para a aba **"Postura"**
2. Ajuste as configurações conforme necessário:
   - **Sensibilidade**: Controla quão rigoroso é o detector (menor valor = mais sensível)
   - **Tempo para Alerta**: Quantos segundos esperar antes de disparar um alerta
3. Toque no botão **"Iniciar"**
4. O status mudará para "Monitoramento ativo"

### 3. Durante o Monitoramento
- O aplicativo mostrará em tempo real:
  - Status da postura (Boa Postura, Atenção, Postura Ruim)
  - Dados do acelerômetro (X, Y, Z)
  - Inclinação atual em graus
  - Estatísticas de alertas

### 4. Alertas de Postura
- Quando a postura estiver incorreta por mais tempo que o configurado:
  - O dispositivo **vibrará**
  - Uma mensagem de alerta será exibida
  - O contador de alertas será incrementado

## ⚙️ Configurações

### Sensibilidade (10% - 100%)
- **Baixa (10-30%)**: Mais sensível, alerta mesmo com pequenas inclinações
- **Média (30-70%)**: Equilibrio entre sensibilidade e tolerância
- **Alta (70-100%)**: Menos sensível, só alerta com inclinações maiores

### Tempo para Alerta (1-15 segundos)
- Tempo que o sistema aguarda antes de disparar um alerta
- Evita alertas falsos por movimentos temporários

## 🎯 Como Funciona

### Detecção de Postura
O aplicativo usa o acelerômetro para detectar a orientação do dispositivo:

- **Boa Postura**: Inclinação < 15° (ajustado pela sensibilidade)
- **Atenção**: Inclinação entre 15° e 30°
- **Postura Ruim**: Inclinação > 30°

### Cálculo da Inclinação
- Utiliza os três eixos do acelerômetro (X, Y, Z)
- Calcula o ângulo de inclinação em relação à vertical
- Considera a orientação típica de um celular no bolso da camisa

## 📊 Interface

### Status Principal
- **Cor Verde**: Boa postura
- **Cor Laranja**: Atenção - postura levemente incorreta
- **Cor Vermelha**: Postura ruim - correção necessária

### Dados em Tempo Real
- **X, Y, Z**: Valores brutos do acelerômetro
- **Inclinação**: Ângulo calculado em graus
- **Total de alertas**: Contador de alertas disparados
- **Último alerta**: Horário e mensagem do último alerta

## 🔧 Funcionalidades Técnicas

### Serviços Implementados
- **IPostureService**: Interface para o serviço de monitoramento
- **PostureService**: Implementação usando MAUI Essentials
- **PosturePageViewModel**: ViewModel com padrão MVVM
- **PosturePage**: Interface XAML responsiva

### Recursos Utilizados
- **Accelerometer**: Sensor de aceleração do dispositivo
- **Vibration**: Feedback tátil para alertas
- **Timer**: Monitoramento contínuo (atualização a cada 500ms)
- **MainThread**: Atualização segura da interface

### Permissões Android
- `android.permission.VIBRATE`: Para vibração de alerta
- `android.hardware.sensor.accelerometer`: Para acesso ao acelerômetro

## 🚀 Arquitetura

O código segue as melhores práticas do .NET MAUI:

- **Padrão MVVM** com CommunityToolkit.Mvvm
- **Injeção de Dependência** configurada no MauiProgram
- **Separação de responsabilidades** entre Services, ViewModels e Views
- **Interface responsiva** com ScrollView e Frames
- **Observável properties** para binding de dados
- **Commands** para ações da interface

## 🎨 Design

- Interface limpa e organizada em cards
- Cores intuitivas para status (Verde/Laranja/Vermelho)
- Controles deslizantes para configurações
- Botões de ação claramente identificados
- Estatísticas em tempo real
- Instruções de uso incluídas na interface

## 📱 Compatibilidade

- **Android**: API 21+ (testado)
- **iOS**: iOS 15.0+ (compatível)
- Requer dispositivo com acelerômetro
- Função de vibração opcional (funciona sem se não disponível)

## 🔍 Troubleshooting

### Problemas Comuns
1. **"Acelerômetro não disponível"**: Dispositivo pode não ter o sensor
2. **Alertas muito frequentes**: Diminua a sensibilidade
3. **Poucos alertas**: Aumente a sensibilidade
4. **Não vibra**: Verifique se o dispositivo tem vibração habilitada

### Debug
- Monitore os valores X, Y, Z em tempo real
- Observe a inclinação calculada
- Ajuste sensibilidade baseado no seu uso
- Use o botão "Reset" para limpar estatísticas

---

**Desenvolvido seguindo as diretrizes do .NET MAUI e padrões de clean architecture.**