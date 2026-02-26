# Skills do GitHub Copilot - Projeto .NET MAUI

Este documento mapeia todas as skills disponíveis para uso com GitHub Copilot neste projeto.

## Como Usar as Skills

As skills são ativadas automaticamente pelo GitHub Copilot quando você faz perguntas ou solicita ações relacionadas ao domínio específico de cada skill. Para invocar manualmente uma skill, mencione o nome ou a descrição da tarefa.

## Skills Disponíveis

### 1. criar-app-maui

**Descrição:** Cria um novo projeto MAUI com configuração básica.

**Localização:** [criar-app-maui/SKILL.md](criar-app-maui/SKILL.md)

**Uso:**
- Configuração inicial de projetos .NET MAUI
- Setup de target frameworks (.NET 9, Android, iOS)
- Instalação de pacotes essenciais (CommunityToolkit.Mvvm, CommunityToolkit.Maui)
- Organização de estrutura de pastas

**Exemplo de Invocação:**
- "Crie um novo app MAUI"
- "Configure um projeto MAUI básico"
- "Inicialize um projeto MAUI com as configurações padrão"

---

### 2. criar-teste-unitario

**Descrição:** Cria um teste unitário básico para um projeto MAUI.

**Localização:** [criar-teste-unitario/SKILL.md](criar-teste-unitario/SKILL.md)

**Uso:**
- Geração de testes unitários usando MSTest
- Testes para ViewModels (comandos, propriedades, métodos assíncronos)
- Testes para Services com mocking usando NSubstitute
- Testes para Converters (IValueConverter)
- Configuração de projetos de teste

**Exemplo de Invocação:**
- "Crie um teste unitário para o LoginViewModel"
- "Gere testes para o FormService"
- "Crie testes para o converter InvertedBoolConverter"
- "Configure um projeto de teste unitário"

---

## Estrutura de uma Skill

Cada skill está organizada em sua própria pasta contendo:
- `SKILL.md` - Arquivo com instruções detalhadas da skill
- Frontmatter YAML com `name` e `description`
- Conteúdo estruturado com orientações específicas

## Adicionar Nova Skill

Para adicionar uma nova skill:

1. Crie uma nova pasta em `.github/skills/` com o nome da skill (kebab-case)
2. Crie um arquivo `SKILL.md` com o seguinte formato:

```markdown
---
name: nome-da-skill
description: Breve descrição do que a skill faz.
---

# Título da Skill

Instruções detalhadas aqui...
```

3. Atualize este arquivo `SKILLS.md` com a nova entrada
4. Adicione a skill em `.github/copilot-instructions.md` na seção `<skills>`

---

**Última atualização:** 25/02/2026
