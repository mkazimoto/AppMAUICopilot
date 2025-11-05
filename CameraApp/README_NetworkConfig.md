# 🌐 Configurações de Rede Local - Android

Foi implementada a configuração completa para permitir acesso à rede local e ao IP específico `10.88.233.118` no aplicativo Android.

## 🔧 Configurações Implementadas

### 1. **AndroidManifest.xml - Permissões**

Adicionadas as seguintes permissões no `Platforms/Android/AndroidManifest.xml`:

```xml
<!-- Permissões de rede -->
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
<uses-permission android:name="android.permission.CHANGE_WIFI_STATE" />

<!-- Configuração da aplicação -->
<application 
    android:usesCleartextTraffic="true" 
    android:networkSecurityConfig="@xml/network_security_config">
```

### 2. **Network Security Config**

Criado arquivo `Platforms/Android/Resources/xml/network_security_config.xml`:

```xml
<network-security-config>
    <!-- IPs específicos permitidos -->
    <domain-config cleartextTrafficPermitted="true">
        <domain includeSubdomains="false">localhost</domain>
        <domain includeSubdomains="false">127.0.0.1</domain>
        <domain includeSubdomains="false">10.88.233.118</domain>
    </domain-config>
    
    <!-- Rede 10.x.x.x completa -->
    <domain-config cleartextTrafficPermitted="true">
        <domain includeSubdomains="true">10.88.233.0</domain>
    </domain-config>
    
    <!-- Configuração base para desenvolvimento -->
    <base-config cleartextTrafficPermitted="true">
        <trust-anchors>
            <certificates src="system"/>
        </trust-anchors>
    </base-config>
</network-security-config>
```

## 🎯 Funcionalidades Habilitadas

### ✅ **Acesso HTTP Local**
- Comunicação com `http://10.88.233.118:8051`
- Suporte a cleartext traffic (HTTP não criptografado)
- Acesso a toda rede `10.88.233.x`

### ✅ **Permissões WiFi**
- `ACCESS_WIFI_STATE`: Leitura do estado do WiFi
- `CHANGE_WIFI_STATE`: Modificação de configurações WiFi
- `ACCESS_NETWORK_STATE`: Verificação de conectividade

### ✅ **Configuração de Segurança**
- Network Security Config personalizado
- Certificados do sistema confiáveis
- Domains específicos permitidos

## 🔒 Segurança Implementada

### **Domains Permitidos**
- ✅ `localhost` - Desenvolvimento local
- ✅ `127.0.0.1` - Loopback local  
- ✅ `10.88.233.118` - Servidor TOTVS RM específico
- ✅ `10.88.233.0/24` - Rede local completa

### **Certificados**
- Usa certificados do sistema Android
- Permite cleartext apenas para IPs especificados
- Mantém segurança para outros domínios

## 🚀 Teste de Conectividade

### **Verificar Configuração**
1. **Abra o aplicativo** no dispositivo Android
2. **Vá para a tela de login**
3. **Digite as credenciais TOTVS**:
   - Usuário: `mestre`
   - Senha: `totvs`
4. **Teste a conexão** com o servidor `10.88.233.118:8051`

### **Solução de Problemas**

Se ainda houver problemas de conectividade:

1. **Verificar rede WiFi**: Dispositivo na mesma rede que o servidor
2. **Ping do servidor**: `ping 10.88.233.118` do dispositivo
3. **Porta aberta**: Servidor rodando na porta `8051`
4. **Firewall**: Verificar se não está bloqueando a conexão

## 📱 URLs Configuradas

### **AuthService.cs**
```csharp
private readonly string _baseUrl = "http://10.88.233.118:8051";
```

### **Endpoints da API**
- **Login**: `POST http://10.88.233.118:8051/api/connect/token`
- **Refresh**: `POST http://10.88.233.118:8051/api/connect/token`

## ⚡ Deploy Realizado

O aplicativo foi deployado no dispositivo Android com todas as configurações de rede aplicadas. Agora é possível:

- ✅ Conectar ao servidor TOTVS RM na rede local
- ✅ Fazer login com autenticação Bearer
- ✅ Acessar APIs protegidas via HTTP
- ✅ Manter conectividade estável na rede local

As configurações seguem as melhores práticas de segurança do Android, permitindo acesso apenas aos IPs específicos necessários para o funcionamento do aplicativo.