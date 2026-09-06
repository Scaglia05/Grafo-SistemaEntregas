# SistemaEntregas — Entregas e Logística (Tema 7, Grupo 04)

Grafo ponderado de centros de distribuição, clientes e rotas, com cálculo do menor
caminho por **Dijkstra** e **Bellman-Ford**, comparáveis lado a lado.

## Stack

- **Blazor Web App / .NET 10** (render mode `InteractiveServer` global)
- Estado em memória via `ServicoGrafo` (singleton), com seed pré-carregado
- Mapa: **Leaflet.js + OpenStreetMap** por CDN, controlado via JS interop
- Peso "distância" calculado por **Haversine** a partir de lat/long reais
- Peso "custo" definido manualmente

> O escopo original pedia Blazor Server .NET 8 com `_Host.cshtml`. Foi usado o
> modelo moderno (.NET 10, `App.razor` como raiz, sem `_Host.cshtml`) a pedido.
> Para voltar ao .NET 8, troque `<TargetFramework>` e reintroduza `_Host.cshtml`.

## Rodar

```bash
cd SistemaEntregas
dotnet run
```

Abre em `http://localhost:5080` (ou `https://localhost:7080`).

## Estrutura

```
SistemaEntregas/
├── Program.cs                      boilerplate + DI (ServicoGrafo singleton, algoritmos scoped)
├── Modelos/                        TipoNo, Criterio, NoGrafo, Rota, ResultadoCaminho
├── Servicos/
│   ├── CalculadoraHaversine.cs     distância geodésica
│   ├── ServicoGrafo.cs             estado + CRUD + seed + adjacências
│   ├── ServicoDijkstra.cs          fila de prioridade
│   ├── ServicoBellmanFord.cs       relaxamento V-1 vezes + ciclo negativo
│   └── CaminhoUtil.cs              reconstrução do caminho
├── Componentes/
│   ├── App.razor / Roteador.razor  raiz + roteamento
│   ├── Compartilhado/              LayoutPrincipal, MenuNavegacao
│   └── Paginas/
│       ├── Inicio.razor            calculadora de rota + mapa + comparação  (/)
│       ├── Grafo.razor             CRUD de nós e rotas                       (/grafo)
│       └── Erro.razor              (/erro)
└── wwwroot/
    ├── css/site.css
    └── js/mapa.js                  interop Leaflet
```

Nomes mantidos em inglês por serem convenção fixa do Blazor: `Program.cs`,
`App.razor`, `_Imports.razor`, `wwwroot`, `Properties`.

## Seed

3 centros (Ribeirão Preto, Campinas, São Paulo) + 8 clientes da região, 17 rotas
bidirecionais com caminhos alternativos.

## Estado de implementação

Tudo compila e roda (`dotnet build` / `dotnet run` testados; `/`, `/grafo` e
`/erro` respondendo 200). Algoritmos completos; telas funcionais.

Pendências opcionais do escopo: clicar no mapa para capturar coordenadas na tela
de gestão; ícones customizados para centro vs cliente (hoje usa emoji via
`divIcon`).
