# Diferença entre Prompt, Skill, Hook, Agent e Instruction no GitHub Copilot

Vamos analisar a diferença entre esses cinco conceitos no contexto do **GitHub Copilot** e da programação com IA. Embora alguns desses termos (como "agente") sejam mais amplos na indústria de IA, dentro do ecossistema Copilot e extensões similares (como VS Code), eles têm significados específicos.

---

## 1. Prompt
- **O que é:** É a **entrada do usuário**. É o texto que você digita para dizer ao Copilot o que você quer.
- **Função:** É a matéria-prima da interação. Pode ser um comentário em linguagem natural (`// criar função de soma`), uma função incompleta, ou uma pergunta no chat.
- **Exemplo:** Você digita: `def calcular_idade(ano_nascimento):` e o Copilot sugere o resto do código. O texto que você digitou é o *prompt*.

## 2. Instruction
- **O que é:** É um tipo específico de **prompt**, geralmente usado para **comandar** uma ação direta. É uma diretiva explícita.
- **Diferença:** Enquanto um prompt pode ser apenas um contexto ("Aqui está meu código..."), uma instruction é uma ordem: "Refatore isso", "Explique aquilo", "Adicione logs aqui".
- **Exemplo:** No chat do Copilot, você digita: "@workspace /explique este código". A parte "/explique" é a instruction dizendo ao agente o que fazer com o código que você forneceu.

## 3. Hook
- **O que é:** Um **gancho** ou ponto de extensão. No contexto de ferramentas como o Copilot, hooks são pontos no fluxo de trabalho do editor (VS Code) ou da aplicação onde você pode injetar comportamento personalizado ou acionar a IA.
- **Contexto Técnico:** Relacionado aos "Git Hooks" ou "Lifecycle Hooks". Por exemplo, você pode ter um hook que é executado *antes* de um commit, que chama o Copilot para revisar o código automaticamente.
- **Exemplo:** Um hook configurado para que, toda vez que você salvar um arquivo (`onSave`), o Copilot execute uma verificação de segurança no código escrito.

## 4. Skill (ou "Habilidade")
- **O que é:** No contexto do GitHub Copilot Extensions (ou Agents), uma **skill** é uma **capacidade especializada** que um agente possui. É uma função específica que a IA pode executar, muitas vezes integrada com uma API externa.
- **Função:** Permite que o Copilot não apenas gere texto, mas *aja* no mundo real (buscar dados, executar comandos).
- **Exemplo:** Imagine um agente "@weather". Ele pode ter uma *skill* chamada `get_weather_data`. Quando você pergunta "Como está o tempo?", o agente usa essa skill para chamar uma API de meteorologia, pegar os dados e te responder. O Copilot padrão não tem skills externas; isso é para extensões.

## 5. Agent (ou "Agente")
- **O que é:** É uma **entidade autônoma** que combina prompts, instruções e skills para executar tarefas. No GitHub Copilot, o "agente" é o modelo de IA que processa seu pedido, mas o termo ficou famoso com o recurso **"@agent"** (ou GitHub Copilot Agent Mode).
- **Função:** Um agente pode iterar sobre o problema. Em vez de você pedir "faça X" e ele fazer X, você pede "construa um jogo da velha" e ele planeja, escreve os arquivos, testa e corrige os erros sozinho (ciclo iterativo).
- **Exemplo:** No VS Code Insiders, o **Agent Mode** (@agent) permite que o Copilot não só sugira código, mas também execute terminal, identifique erros de runtime e tente corrigi-los sem você precisar copiar e colar o erro de volta.

---

## Resumo para não confundir:

| Termo       | Tradução     | Função Principal                                           | Quem usa?                        |
| :---------- | :----------- | :--------------------------------------------------------- | :------------------------------- |
| **Prompt**  | Comando      | A **pergunta** ou **contexto** que você digita.           | O usuário.                       |
| **Instruction** | Instrução   | Um comando **específico** dentro do prompt (ex: "traduza", "explique"). | O usuário (para direcionar a IA). |
| **Hook**    | Gancho       | Um ponto no **sistema** onde a IA pode agir automaticamente. | O sistema (VS Code / Git).       |
| **Skill**   | Habilidade   | Uma **ação específica** que a IA sabe fazer (ex: buscar preço de ação). | O Agente (para executar tarefas). |
| **Agent**   | Agente       | O **cérebro autônomo** que decide qual skill usar e como executar a instrução. | O modelo de IA (Copilot).        |

---

## Analogia: Restaurante

- **Prompt:** "Estou com fome, me traga comida italiana."
- **Instruction:** "Quero um macarrão ao pesto, sem cebola."
- **Hook:** O garçom percebe que você derrubou o garfo e automaticamente traz outro (sem você pedir).
- **Skill:** A habilidade do chef de cozinhar um macarrão perfeito.
- **Agent:** O chef, que pega sua instrução, decide se vai ferver a água primeiro ou preparar o molho, e executa a(s) skill(s) necessária(s) para entregar o prato.