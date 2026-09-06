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

Pré-requisito: **.NET 10 SDK**.

```bash
git clone https://github.com/Scaglia05/Grafo-SistemaEntregas.git
cd Grafo-SistemaEntregas/SistemaEntregas
dotnet run
```

Acesse **http://localhost:5080** (ou `https://localhost:7080`).

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
