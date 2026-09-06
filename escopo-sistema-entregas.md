# Escopo — Sistema de Entregas e Logística (Tema 7, Grupo 04)

## 1. Objetivo do trabalho

Representar centros de distribuição, clientes e rotas como um **grafo ponderado**, e
encontrar o **menor caminho** de um centro até um cliente, considerando **distância**
ou **custo** como critério. Algoritmos exigidos: **Dijkstra** e **Bellman-Ford**
(implementar os dois, para comparar resultado e desempenho — distância e custo de
entrega são sempre não-negativos, então em tese os dois convergem para o mesmo
caminho; o valor didático está em comparar).

## 2. Stack técnica

- **Blazor Server (.NET 8)** + C#
- Sem banco de dados — estado mantido em memória via serviço singleton
  (`GrafoService`), com dados de exemplo pré-carregados (seed)
- **Sem API paga de mapas.** Visualização geográfica via **Leaflet.js +
  OpenStreetMap** (gratuito, sem chave de API), carregados por CDN e controlados
  via JS interop do Blazor
- Peso "distância" calculado automaticamente por **fórmula de Haversine** a partir
  das coordenadas reais (lat/long) dos nós — sem chamada a nenhum serviço externo
  de roteamento
- Peso "custo" é sempre definido manualmente (frete depende de pedágio,
  combustível, tipo de veículo etc., não é função direta da distância)

## 3. Decisões já tomadas (não reabrir sem motivo forte)

- Linguagem/plataforma: **C# / Blazor Server**, confirmado pelo usuário
- Abordagem visual: **mapa real (Leaflet/OSM) + Haversine**, confirmado pelo
  usuário — descartadas as opções "diagrama abstrato sem mapa" e "API paga do
  Google Maps"
- Ambos os algoritmos (Dijkstra e Bellman-Ford) devem ser implementados e
  comparáveis lado a lado na interface
- Rotas são bidirecionais por padrão
- Nós têm dois tipos: `CentroDistribuicao` e `Cliente`

## 4. Estrutura de pastas do projeto

```
SistemaEntregas/
├── SistemaEntregas.csproj
├── Program.cs
├── App.razor
├── _Imports.razor
├── Models/
│   ├── TipoNo.cs
│   ├── Criterio.cs
│   ├── NoGrafo.cs
│   ├── Rota.cs
│   └── ResultadoCaminho.cs
├── Services/
│   ├── HaversineService.cs
│   ├── GrafoService.cs          (PENDENTE)
│   ├── DijkstraService.cs       (PENDENTE)
│   └── BellmanFordService.cs    (PENDENTE)
├── Shared/
│   ├── MainLayout.razor         (PENDENTE)
│   └── NavMenu.razor            (PENDENTE)
├── Pages/
│   ├── _Host.cshtml             (PENDENTE)
│   ├── Index.razor              (PENDENTE — calculadora de rota + mapa)
│   └── Grafo.razor              (PENDENTE — CRUD de centros/clientes/rotas)
└── wwwroot/
    ├── css/site.css             (PENDENTE)
    └── js/mapa.js               (PENDENTE — interop Leaflet)
```

## 5. Já implementado (código pronto, revisar mas não reescrever do zero)

**`Models/TipoNo.cs`** — enum `CentroDistribuicao | Cliente`

**`Models/Criterio.cs`** — enum `Distancia | Custo`

**`Models/NoGrafo.cs`** — `Id`, `Nome`, `Tipo`, `Latitude`, `Longitude`,
propriedade calculada `Rotulo` (emoji + nome conforme o tipo)

**`Models/Rota.cs`** — `Id`, `OrigemId`, `DestinoId`, `DistanciaKm`, `Custo`,
`Bidirecional` (default `true`), método `Peso(Criterio)` que retorna
`DistanciaKm` ou `Custo` conforme o critério pedido

**`Models/ResultadoCaminho.cs`** — `Algoritmo`, `Encontrado`, `Caminho`
(`List<NoGrafo>`), `PesoTotal`, `CriterioUsado`, `NosVisitados`,
`ArestasRelaxadas`, `TempoMicrossegundos`, `Mensagem?` — pensado para alimentar
a tela de comparação Dijkstra vs Bellman-Ford

**`Services/HaversineService.cs`** — método estático
`CalcularDistanciaKm(lat1, lon1, lat2, lon2)`, fórmula padrão de Haversine,
raio da Terra 6371 km, resultado arredondado a 1 casa decimal

## 6. Pendente de implementação

### `Services/GrafoService.cs`
- Estado em memória: `List<NoGrafo>`, `List<Rota>`
- Métodos CRUD: adicionar/remover nó, adicionar/remover rota, listar
  adjacências de um nó
- Ao adicionar uma rota, se `DistanciaKm` não for informada manualmente,
  calcular automaticamente via `HaversineService` a partir das coordenadas dos
  dois nós
- **Seed data**: pelo menos 2–3 centros de distribuição e 6–8 clientes, usando
  **coordenadas reais de cidades brasileiras** (ex.: região de Ribeirão
  Preto/Campinas/São Paulo, já que é a região do usuário — mas pode usar
  qualquer conjunto plausível), e ~15 rotas conectando-os de forma que existam
  caminhos alternativos (para o cálculo de menor caminho fazer sentido)

### `Services/DijkstraService.cs`
- Implementação clássica com fila de prioridade (`PriorityQueue<T, TPriority>`
  do .NET)
- Assinatura sugerida: `ResultadoCaminho Calcular(GrafoService grafo, string origemId, string destinoId, Criterio criterio)`
- Preencher `NosVisitados`, `TempoMicrossegundos` (via `Stopwatch`) e
  `Mensagem` (ex.: "cliente inalcançável" se não houver caminho)
- **Não** precisa suportar pesos negativos (Dijkstra não suporta — isso é
  ponto de comparação com Bellman-Ford no relatório)

### `Services/BellmanFordService.cs`
- Implementação clássica: relaxamento de todas as arestas `V-1` vezes
- Mesma assinatura de retorno (`ResultadoCaminho`) para permitir comparação
  direta na UI
- Detectar e reportar ciclo negativo (mesmo não sendo esperado neste domínio,
  é parte do algoritmo e vale mencionar no relatório)
- Preencher `ArestasRelaxadas` e `TempoMicrossegundos`

### `Pages/Index.razor` — tela principal
- Formulário: dropdown de origem (só centros), dropdown de destino (só
  clientes), seleção de critério (distância/custo), seleção de algoritmo
  (Dijkstra / Bellman-Ford / Comparar os dois)
- Mapa Leaflet (via JS interop) mostrando todos os nós como marcadores
  (ícone diferente para centro vs cliente) e, após o cálculo, o caminho
  encontrado destacado com uma polyline colorida
- Painel de resultado: lista ordenada do caminho, peso total, e — se "Comparar
  os dois" for selecionado — tabela lado a lado com as métricas de
  `ResultadoCaminho` de cada algoritmo

### `Pages/Grafo.razor` — gestão de dados
- Formulário para adicionar centro/cliente (nome, tipo, lat/long — ou clicar
  no mapa para pegar coordenadas, se der tempo)
- Formulário para adicionar rota (origem, destino, custo manual; distância
  auto-calculada mas editável)
- Listagem/remoção dos nós e rotas existentes

### `wwwroot/js/mapa.js` — interop Leaflet
- Funções JS invocáveis do Blazor: inicializar mapa, adicionar/atualizar
  marcadores, desenhar/limpar polyline do caminho
- Referenciar Leaflet via CDN (CSS + JS) no `_Host.cshtml`, sem necessidade de
  build step adicional

### `Program.cs`, `App.razor`, `_Imports.razor`, `Pages/_Host.cshtml`, `Shared/MainLayout.razor`, `Shared/NavMenu.razor`
- Boilerplate padrão de Blazor Server (.NET 8, hospedagem clássica com
  `MapBlazorHub` + `MapFallbackToPage("/_Host")`), registrar `GrafoService`
  como singleton, `DijkstraService`/`BellmanFordService` como scoped ou
  singleton (sem estado, tanto faz)

## 7. Fora de escopo (não implementar sem pedido explícito)

- Autenticação/usuários
- Banco de dados persistente
- Qualquer API paga de mapas/roteamento (Google Maps, etc.)
- Otimização multi-critério (distância + custo combinados) — mencionado como
  possível extensão futura, não faz parte do núcleo do trabalho

## 8. Critério de "pronto"

- `dotnet run` sobe a aplicação sem erros
- É possível escolher um centro, um cliente e um critério, e ver o caminho
  calculado tanto por Dijkstra quanto por Bellman-Ford, com os dois
  concordando no resultado (peso total igual)
- O mapa mostra os nós nas posições geográficas corretas e destaca o caminho
  encontrado
- É possível cadastrar um novo cliente e uma nova rota pela interface e
  recalcular um caminho que passe por eles
