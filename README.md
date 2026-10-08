# Grafo-SistemaEntregas

Sistema de **entregas e logística** que modela centros de distribuição, clientes e
rotas como um **grafo ponderado** e calcula o **menor caminho** de um centro até um
cliente — por **distância** ou por **custo** — usando **Dijkstra** e **Bellman-Ford**,
com os resultados comparáveis lado a lado e o trajeto desenhado sobre um mapa real.

> Trabalho de **Teoria dos Grafos** — Tema 7, Grupo 04
> Engenharia da Computação · 10º semestre
> FHO – Centro Universitário Fundação Hermínio Ometto · Araras/SP

![.NET](https://img.shields.io/badge/.NET-10-512BD4)
![Blazor](https://img.shields.io/badge/Blazor-Web%20App-512BD4)
![Leaflet](https://img.shields.io/badge/Leaflet-OpenStreetMap-199900)

---

## Sobre

- **Nós:** centros de distribuição (🏭) e clientes (📦), com coordenadas reais (lat/long).
- **Arestas:** rotas bidirecionais por padrão, com dois pesos:
  - **Distância (km)** — calculada automaticamente pela fórmula de **Haversine** a
    partir das coordenadas, sem nenhum serviço externo de roteamento.
  - **Custo (R$)** — informado manualmente (frete não é função direta da distância).
- **Objetivo didático:** como distância e custo são sempre não-negativos, Dijkstra e
  Bellman-Ford devem chegar ao **mesmo caminho** — o valor está em comparar
  desempenho e número de operações.

## Funcionalidades

- Escolher origem (centro), destino (cliente), critério e algoritmo.
- Modo **"Comparar os dois"**: tabela lado a lado com peso total, nós visitados,
  arestas relaxadas e tempo em microssegundos de cada algoritmo, mais um aviso de
  concordância/divergência.
- **Mapa Leaflet + OpenStreetMap** mostrando todos os nós e destacando o trajeto
  calculado com uma polyline.
- Tela de **gestão do grafo**: cadastrar e remover centros, clientes e rotas;
  a distância é auto-calculada quando não informada.
- Estado em memória com **seed** pré-carregado (região Ribeirão Preto / Campinas /
  São Paulo): 3 centros, 8 clientes e 17 rotas com caminhos alternativos.

## Algoritmos

| | Dijkstra | Bellman-Ford |
|---|---|---|
| Estrutura | fila de prioridade (`PriorityQueue`) | relaxamento de todas as arestas `V-1` vezes |
| Pesos negativos | não suporta | suporta + detecta ciclo negativo |
| Parada antecipada | ao alcançar o destino | quando uma iteração não altera nada |
| Métricas coletadas | nós visitados, tempo | arestas relaxadas, tempo |

Ambos retornam o mesmo tipo (`ResultadoCaminho`), o que permite a comparação direta
na interface.

## Stack

- **C# / Blazor Web App (.NET 10)** — render mode `InteractiveServer`
- Sem banco de dados — serviço singleton `ServicoGrafo` mantém o estado
- **Leaflet.js + OpenStreetMap** por CDN, controlados via JS interop (`wwwroot/js/mapa.js`)
- Sem API paga de mapas ou roteamento

## Como executar

Lista completa de pré-requisitos em [`REQUISITOS.md`](REQUISITOS.md). Em resumo:
**.NET 10 SDK**, um navegador e conexão com a internet (o mapa é carregado por CDN).

### 1. Verifique o .NET

```bash
dotnet --version
```

Deve mostrar `10.x.x` ou superior. Caso contrário, instale o SDK em
https://dotnet.microsoft.com/download/dotnet/10.0

### 2. Baixe o projeto

```bash
git clone https://github.com/Scaglia05/Grafo-SistemaEntregas.git
cd Grafo-SistemaEntregas/SistemaEntregas
```

Sem Git: no GitHub, **Code → Download ZIP**, extraia e entre na pasta
`SistemaEntregas` (a que contém o `SistemaEntregas.csproj`).

### 3. Execute

```bash
dotnet run
```

Na primeira vez o .NET restaura e compila o projeto (alguns segundos). Quando
aparecer `Now listening on: http://localhost:5080`, o sistema está no ar.

### 4. Abra no navegador

**http://localhost:5080**

Para encerrar, volte ao terminal e pressione `Ctrl+C`.

### Rodando pelo Visual Studio

1. Abra a pasta `SistemaEntregas` (ou o arquivo `SistemaEntregas.csproj`).
2. Escolha o perfil **http** ou **https** na barra superior.
3. Pressione **F5** (com depuração) ou **Ctrl+F5** (sem depuração).

### Como usar

1. Em **Calculadora de Rota** (`/`), escolha o centro de distribuição de origem,
   o cliente de destino, o critério (**Distância** ou **Custo**) e o algoritmo
   (**Dijkstra**, **Bellman-Ford** ou **Comparar os dois**).
2. Clique em **Calcular rota**. O caminho aparece em laranja no mapa, e o painel
   mostra a sequência de paradas, o peso total e as métricas de cada algoritmo.
3. Em **Gestão do Grafo** (`/grafo`), cadastre novos centros, clientes e rotas ou
   remova os existentes. Se a distância da rota ficar em `0`, ela é calculada
   automaticamente por Haversine.
4. Volte à calculadora e recalcule: o novo cliente ou rota já entra nas opções.

> Os dados ficam só em memória: ao reiniciar a aplicação, o grafo volta ao
> estado inicial (3 centros, 8 clientes, 17 rotas).

### Problemas comuns

| Sintoma | Causa provável | Solução |
|---|---|---|
| `dotnet` não é reconhecido | SDK não instalado ou terminal aberto antes da instalação | instale o .NET 10 SDK e abra um novo terminal |
| `The framework 'Microsoft.NETCore.App', version '10.0.0' was not found` | só o Runtime ou uma versão antiga instalada | instale o **SDK** 10 |
| `Address already in use` | porta 5080 ocupada | feche o outro processo ou mude `applicationUrl` em `Properties/launchSettings.json` |
| Mapa cinza, sem imagem | sem internet ou CDN bloqueada | verifique a conexão; o Leaflet e os tiles vêm da internet |
| Aviso de certificado ao usar `https://localhost:7080` | certificado de desenvolvimento não confiável | rode `dotnet dev-certs https --trust` ou use a porta HTTP `5080` |
| `No project was found` | comando executado fora da pasta do projeto | entre em `SistemaEntregas/` (onde está o `.csproj`) |

## Estrutura

```
SistemaEntregas/
├── Program.cs                 boilerplate + injeção de dependência
├── Modelos/
│   ├── Enum/                  TipoNo, Criterio
│   ├── NoGrafo.cs  Rota.cs  ResultadoCaminho.cs
├── Servicos/
│   ├── CalculadoraHaversine.cs    distância geodésica
│   ├── ServicoGrafo.cs            estado + CRUD + seed + adjacências
│   ├── ServicoDijkstra.cs
│   ├── ServicoBellmanFord.cs
│   └── CaminhoUtil.cs             reconstrução do caminho
├── Componentes/
│   ├── App.razor  Roteador.razor  _Imports.razor
│   ├── Compartilhado/         LayoutPrincipal, MenuNavegacao
│   └── Paginas/
│       ├── Inicio.razor       "/"       calculadora de rota + mapa
│       ├── Grafo.razor        "/grafo"  gestão do grafo
│       └── Erro.razor         "/erro"
└── wwwroot/                   css/site.css · js/mapa.js
```

O documento de escopo completo está em
[`escopo-sistema-entregas.md`](escopo-sistema-entregas.md).

## Grupo 04

| Nome | RA | E-mail |
|---|---|---|
| Caroline da Silva Grizante | 114105 | carolinegrizante105@alunos.fho.edu.br |
| Emilly Emanuelly R. dos Santos | 114095 | emillyribeiro@alunos.fho.edu.br |
| Guilherme Augusto Scaglia | 111598 | scaglia@alunos.fho.edu.br |
| Marcela Lovatto | 113626 | marcela.lovatto@alunos.fho.edu.br |

## Agradecimentos

Ao professor **Thiago Giroto Milani**, pela orientação na disciplina de
Teoria dos Grafos.

---

Projeto acadêmico — uso educacional.
