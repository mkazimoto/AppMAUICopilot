# 🔐 Fluxo de Autenticação Obrigatório

O aplicativo foi modificado para implementar um fluxo de autenticação obrigatório, onde a tela de login é a primeira a aparecer e o usuário só pode acessar as funcionalidades principais após fazer login.

## 🚀 Fluxo de Navegação

### 1. **Inicialização do App**
- **Primeira tela**: Tela de Login (sempre)
- **Verificação automática**: O app tenta restaurar uma sessão salva
- **Se já autenticado**: Navega automaticamente para o app principal
- **Se não autenticado**: Permanece na tela de login

### 2. **Após Login Bem-sucedido**
- **Navegação automática**: Vai para o MainShell (app principal)
- **Persistência**: Token salvo no SecureStorage
- **Acesso às funcionalidades**: Câmera, Mapa, Postura

### 3. **Logout**
- **Botão "Sair"**: Disponível na barra superior do app principal
- **Limpeza**: Remove todos os dados de autenticação
- **Retorno**: Volta automaticamente para a tela de login

## 📱 Estrutura de Shells

### **AppShell** (Tela de Login)
```xml
- LoginPage (única página)
- NavBar: Oculta
- TabBar: Oculta
```

### **MainShell** (App Principal)
```xml
- CameraPage
- MapPage  
- PosturePage
- Botão Logout na barra superior
```

## 🔄 Estados de Autenticação

### **❌ Não Autenticado**
- Mostra: AppShell com LoginPage
- Funcionalidades: Apenas login
- Persistência: Nenhuma

### **✅ Autenticado** 
- Mostra: MainShell com todas as páginas
- Funcionalidades: Câmera, Mapa, Postura + Logout
- Persistência: Token salvo e renovável

## 🎯 Benefícios da Implementação

### **Segurança**
- ✅ Acesso protegido a todas as funcionalidades
- ✅ Token Bearer obrigatório para usar o app
- ✅ Logout limpa completamente a sessão

### **Experiência do Usuário**
- ✅ Login único por sessão
- ✅ Restauração automática de sessão
- ✅ Interface clara entre autenticado/não autenticado
- ✅ Logout acessível de qualquer tela

### **Arquitetura**
- ✅ Separação clara entre telas públicas e protegidas
- ✅ Reutilização do sistema de autenticação
- ✅ Navegação controlada por estado de autenticação

## 🎨 Interface Atualizada

### **Tela de Login**
- Header com ícone do app e branding
- Formulário de credenciais centralizado  
- Estados visuais para loading e erros
- Design como "splash screen" de autenticação

### **App Principal**
- TabBar com 3 funcionalidades principais
- Botão "Sair" sempre visível na barra superior
- Layout familiar pós-autenticação

## 🔧 Configurações Técnicas

### **Credenciais Padrão**
- **Usuário**: `mestre`
- **Senha**: `totvs`
- **Service Alias**: `CorporeRM` (opcional)

### **URL do Servidor**
```csharp
// AuthService.cs - linha 14
private readonly string _baseUrl = "http://localhost:8051";
```

### **Restauração de Sessão**
- Automática na inicialização
- Usa SecureStorage para buscar token salvo
- Valida expiração antes de restaurar

## 🚀 Como Testar

1. **Instale o app** no dispositivo
2. **Primeira execução**: Verá a tela de login
3. **Digite credenciais** e faça login
4. **Acesse funcionalidades** no app principal
5. **Teste logout** usando botão "Sair"
6. **Feche e reabra** o app para testar restauração de sessão

Este fluxo garante que apenas usuários autenticados possam acessar as funcionalidades do aplicativo, mantendo a segurança e uma experiência de usuário fluida.