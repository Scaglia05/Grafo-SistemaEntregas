# Escopo — Sistema de Entregas e Logística (Tema 7, Grupo 04)

Teoria dos Grafos · Engenharia da Computação · 10º semestre
FHO – Centro Universitário Fundação Hermínio Ometto, Araras/SP ·
Prof. Thiago Giroto Milani

> **Atualização (pós-implementação):** o projeto foi implementado em
> **Blazor Web App / .NET 10** (modelo moderno, sem `_Host.cshtml`) e com
> nomes de pastas/arquivos em **português-BR**. As seções abaixo já refletem
> o estado real do código. O histórico da mudança está na seção 3.

## 1. Objetivo do trabalho

Representar centros de distribuição, clientes e rotas como um **grafo ponderado**, e
encontrar o **menor caminho** de um centro até um cliente, considerando **distância**
ou **custo** como critério. Algoritmos exigidos: **Dijkstra** e **Bellman-Ford**
(implementar os dois, para comparar resultado e desempenho — distância e custo de
entrega são sempre não-negativos, então em tese os dois convergem para o mesmo
caminho; o valor didático está em comparar).

## 2. Stack técnica

- **Blazor Web App (.NET 10)** + C#, render mode `InteractiveServer` global
  (definido em `App.razor`, sem `_Host.cshtml` nem `MapBlazorHub`)
- Sem banco de dados — estado mantido em memória via serviço singleton
  (`ServicoGrafo`), com dados de exemplo pré-carregados (seed)
- **Sem API paga de mapas.** Visualização geográfica via **Leaflet.js +
  OpenStreetMap** (gratuito, sem chave de API), carregados por CDN no
  `App.razor` e controlados via JS interop do Blazor (`wwwroot/js/mapa.js`)
- Peso "distância" calculado automaticamente por **fórmula de Haversine** a partir
  das coordenadas reais (lat/long) dos nós — sem chamada a nenhum serviço externo
  de roteamento
- Peso "custo" é sempre definido manualmente (frete depende de pedágio,
  combustível, tipo de veículo etc., não é função direta da distância)

## 3. Decisões já tomadas (não reabrir sem motivo forte)

- Linguagem/plataforma: **C# / Blazor**, confirmado pelo usuário
- **Versão do framework:** o escopo original previa Blazor Server .NET 8 com
  hospedagem clássica (`_Host.cshtml` + `MapBlazorHub` +
  `MapFallbackToPage("/_Host")`). A pedido do usuário, migrou-se para o modelo
  atual: **Blazor Web App .NET 10**, `App.razor` como raiz, `<Routes>`/`<HeadOutlet>`
  com `@rendermode="InteractiveServer"`. Para reverter: trocar `<TargetFramework>`
  no `.csproj` e reintroduzir `_Host.cshtml`.
- **Idioma dos identificadores:** pastas, arquivos e classes de serviço em
  português-BR (`Modelos/`, `Servicos/`, `Componentes/`, `ServicoGrafo`,
  `CalculadoraHaversine` etc.). Mantidos em inglês apenas os nomes que são
  convenção fixa do Blazor: `Program.cs`, `App.razor`, `_Imports.razor`,
  `wwwroot/`, `Properties/`.
- Abordagem visual: **mapa real (Leaflet/OSM) + Haversine**, confirmado pelo
  usuário — descartadas as opções "diagrama abstrato sem mapa" e "API paga do
  Google Maps"
- Ambos os algoritmos (Dijkstra e Bellman-Ford) devem ser implementados e
  comparáveis lado a lado na interface
- Rotas são bidirecionais por padrão
- Nós têm dois tipos: `CentroDistribuicao` e `Cliente`

## 4. Estrutura de pastas do projeto

```
Milani/
├── .gitignore
├── .gitattributes
├── escopo-sistema-entregas.md
└── SistemaEntregas/
    ├── SistemaEntregas.csproj          (net10.0)
    ├── Program.cs                       (boilerplate + DI)
    ├── appsettings.json / appsettings.Development.json
    ├── Properties/
    │   └── launchSettings.json
    ├── Modelos/
    │   ├── Enum/
    │   │   ├── TipoNoEnum.cs            enum TipoNo (CentroDistribuicao | Cliente)
    │   │   └── CriterioEnum.cs          enum Criterio (Distancia | Custo)
    │   ├── NoGrafo.cs
    │   ├── Rota.cs
    │   └── ResultadoCaminho.cs
    ├── Servicos/
    │   ├── CalculadoraHaversine.cs
    │   ├── ServicoGrafo.cs
    │   ├── ServicoDijkstra.cs
    │   ├── ServicoBellmanFord.cs
    │   └── CaminhoUtil.cs               reconstrução do caminho (predecessores)
    ├── Componentes/
    │   ├── App.razor                    raiz HTML + CDN Leaflet + render mode
    │   ├── Roteador.razor               <Router> + layout padrão
    │   ├── _Imports.razor
    │   ├── Compartilhado/
    │   │   ├── LayoutPrincipal.razor
    │   │   └── MenuNavegacao.razor
    │   └── Paginas/
    │       ├── Inicio.razor             "/"      calculadora de rota + mapa
    │       ├── Grafo.razor              "/grafo" CRUD de centros/clientes/rotas
    │       └── Erro.razor               "/erro"
    └── wwwroot/
        ├── css/site.css
        └── js/mapa.js                   interop Leaflet
```

## 5. Implementado

**`Modelos/Enum/TipoNoEnum.cs`** — enum `TipoNo` = `CentroDistribuicao | Cliente`

**`Modelos/Enum/CriterioEnum.cs`** — enum `Criterio` = `Distancia | Custo`

**`Modelos/NoGrafo.cs`** — `Id`, `Nome`, `Tipo`, `Latitude`, `Longitude`,
propriedade calculada `Rotulo` (emoji + nome conforme o tipo)

**`Modelos/Rota.cs`** — `Id`, `OrigemId`, `DestinoId`, `DistanciaKm`, `Custo`
(`decimal`), `Bidirecional` (default `true`), método `Peso(Criterio)` que retorna
`DistanciaKm` ou `Custo` conforme o critério

**`Modelos/ResultadoCaminho.cs`** — `Algoritmo`, `Encontrado`, `Caminho`
(`List<NoGrafo>`), `PesoTotal`, `CriterioUsado`, `NosVisitados`,
`ArestasRelaxadas`, `TempoMicrossegundos`, `Mensagem?` — alimenta a tela de
comparação Dijkstra vs Bellman-Ford

**`Servicos/CalculadoraHaversine.cs`** — método estático
`CalcularDistanciaKm(lat1, lon1, lat2, lon2)`, fórmula padrão de Haversine,
raio da Terra 6371 km, resultado arredondado a 1 casa decimal

**`Servicos/ServicoGrafo.cs`** — singleton com `List<NoGrafo>` e `List<Rota>`;
CRUD de nós e rotas; `Adjacencias(noId)` e `ArestasDirecionadas()` para os
algoritmos; cálculo automático da distância via `CalculadoraHaversine` quando
`DistanciaKm` não é informada; **seed** com 3 centros (Ribeirão Preto, Campinas,
São Paulo) + 8 clientes da região e 17 rotas com caminhos alternativos

**`Servicos/ServicoDijkstra.cs`** — Dijkstra com
`PriorityQueue<string, double>`, saída antecipada ao alcançar o destino,
preenche `NosVisitados`, `TempoMicrossegundos` (`Stopwatch`) e `Mensagem`
("cliente inalcançável" quando não há caminho)

**`Servicos/ServicoBellmanFord.cs`** — relaxamento de todas as arestas `V-1`
vezes com parada antecipada quando uma iteração não altera nada; detecção e
report de ciclo negativo; preenche `ArestasRelaxadas` e `TempoMicrossegundos`

**`Servicos/CaminhoUtil.cs`** — `Reconstruir(...)` a partir do dicionário de
predecessores, compartilhado pelos dois algoritmos

**`Componentes/Paginas/Inicio.razor`** — dropdown de origem (só centros), destino
(só clientes), critério (distância/custo), algoritmo (Dijkstra / Bellman-Ford /
Comparar os dois); mapa Leaflet com todos os nós e polyline do caminho; painel
de resultado com o caminho em lista ordenada e tabela lado a lado das métricas de
`ResultadoCaminho`; aviso de concordância/divergência entre os dois algoritmos

**`Componentes/Paginas/Grafo.razor`** — formulários para adicionar nó
(nome, tipo, lat/long) e rota (origem, destino, distância opcional, custo,
bidirecional); listagem e remoção de nós e rotas

**`Componentes/Roteador.razor`, `App.razor`, `_Imports.razor`,
`LayoutPrincipal.razor`, `MenuNavegacao.razor`, `wwwroot/js/mapa.js`,
`wwwroot/css/site.css`** — boilerplate e interop concluídos

**`Program.cs`** — `AddRazorComponents().AddInteractiveServerComponents()`,
`ServicoGrafo` como singleton, `ServicoDijkstra`/`ServicoBellmanFord` como scoped,
`MapRazorComponents<App>().AddInteractiveServerRenderMode()`

## 6. Pendências (opcionais / melhorias)

Nenhuma pendência bloqueante — o critério de "pronto" (seção 8) está atendido.
Itens que ficaram como possível melhoria:

- Clicar no mapa na tela de gestão para capturar lat/long automaticamente
- Ícones customizados de verdade para centro vs cliente (hoje usa emoji via
  `L.divIcon`)
- Ressincronizar o mapa da tela `Inicio` quando o grafo é editado em `Grafo`
  sem precisar recarregar a página
- Thread-safety no `ServicoGrafo` (é singleton mutável acessado pela UI; com
  vários circuitos Blazor simultâneos há risco de corrida)
- Pré-computar a lista de adjacência uma vez em vez de varrer `Rotas` a cada
  passo do Dijkstra (irrelevante nesta escala, mas é a forma "correta")
- Testes automatizados comparando Dijkstra vs Bellman-Ford
- Substituir as strings mágicas de seleção de algoritmo por um enum

## 7. Fora de escopo (não implementar sem pedido explícito)

- Autenticação/usuários
- Banco de dados persistente
- Qualquer API paga de mapas/roteamento (Google Maps, etc.)
- Otimização multi-critério (distância + custo combinados) — possível extensão
  futura, não faz parte do núcleo do trabalho

## 8. Critério de "pronto"

- `dotnet run` (dentro de `SistemaEntregas/`) sobe a aplicação sem erros
  em `http://localhost:5080` — **OK**
- É possível escolher um centro, um cliente e um critério, e ver o caminho
  calculado tanto por Dijkstra quanto por Bellman-Ford, com os dois
  concordando no peso total — **OK**
- O mapa mostra os nós nas posições geográficas corretas e destaca o caminho
  encontrado — **OK**
- É possível cadastrar um novo cliente e uma nova rota pela interface e
  recalcular um caminho que passe por eles — **OK**
