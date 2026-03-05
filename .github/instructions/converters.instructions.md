---
description: "Use ao criar, refatorar ou revisar Converters .NET MAUI (IValueConverter). Cobre estrutura, seguranca de tipos, ConvertBack, singleton, documentacao XML e convencoes de testes unitarios."
applyTo: "**/Converters/*.cs"
---

# Diretrizes de Converters MAUI

Ao gerar ou sugerir codigo em `Converters`, use estas convencoes como guideline preferencial do projeto:

## 1. Papel dos Converters
- Converters sao responsaveis exclusivamente por transformar valores para exibicao na UI.
- Nao coloque regras de negocio, chamadas de servico, acesso a dados ou logica de navegacao em converters.
- Mantenha cada converter com responsabilidade unica e nome descritivo do que converte (ex.: `StringToBoolConverter`, `EditModeToTextConverter`).

## 2. Implementacao de IValueConverter
- Implemente sempre `IValueConverter` explicitamente.
- Use `object?` como tipo de retorno e de parametros em `Convert` e `ConvertBack`.
- Use pattern matching com `is` para verificar e fazer cast do valor de entrada antes de operar.
- Retorne um valor de fallback seguro e semanticamente neutro quando o tipo de entrada for inesperado (ex.: `false`, `0`, string padrao).
- Nunca lance excecao em `Convert` para entradas invalidas; retorne o fallback sem propagar o erro.

## 3. ConvertBack
- Implemente `ConvertBack` com a logica inversa quando a conversao for simetrica (ex.: `InvertedBoolConverter`).
- Quando a conversao inversa nao for aplicavel, lance `NotImplementedException` sem mensagem adicional.
- Nunca deixe `ConvertBack` com corpo vazio ou retornando `null` silenciosamente quando a intencao for nao suportar a conversao.

## 4. Singleton
- Exponha `public static readonly Instance = new()` nos converters sem estado mutavel.
- Use a instancia singleton no registro de recursos XAML e nos testes, evitando instanciacao repetida.
- Nao use singleton em converters que receberem estado via construtor ou propriedade configuravel.

## 5. Registro em XAML
- Registre converters em `Resources/Styles/` ou em `ResourceDictionary` da pagina/componente somente se forem usados localmente.
- Para converters compartilhados, prefira registro global (ex.: `GlobalXmlns.cs` ou `App.xaml`) para evitar declaracoes repetidas.
- Use a instancia singleton como `StaticResource` ao referenciar o converter em XAML.

## 6. Documentacao e estilo C#
- Documente a classe e todos os membros publicos com comentarios XML (`///`).
- No `<returns>` de `Convert`, descreva os valores de saida possiveis e o comportamento para entradas invalidas.
- No `<returns>` de `ConvertBack` que lanca excecao, indique `<exception cref="NotImplementedException">` e que o metodo nunca retorna normalmente.
- Use `file-scoped namespace` consistente com o restante do projeto.
- Nomeie em PascalCase com sufixo `Converter`.

## 7. Testes unitarios
- Crie um arquivo de teste `[ConverterName]Tests.cs` em `CameraApp.Test/Converters/` para cada converter.
- Instancie o converter como campo `private readonly _converter = new()` na classe de teste.
- Nomeie metodos de teste no padrao `Metodo_Cenario_ResultadoEsperado` (ex.: `Convert_NullOrEmptyString_ReturnsFalse`).
- Use `[Theory]` + `[InlineData]` para cobrir multiplos valores de entrada; use `[Fact]` para casos isolados.
- Passe `CultureInfo.InvariantCulture` e `null!` para os parametros nao utilizados (`targetType` e `parameter`).
- Cubra obrigatoriamente: valor valido esperado, valor invalido/nulo, tipo errado e, quando aplicavel, `ConvertBack` (logica inversa ou `NotImplementedException`).
- Para singleton, verifique com `Assert.Same(Converter.Instance, Converter.Instance)`.
