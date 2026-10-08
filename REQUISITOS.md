# Requisitos

O projeto é em **C# / .NET**, então não há `requirements.txt` nem `pip`. As
dependências de código (pacotes NuGet) são restauradas automaticamente pelo
`dotnet run`. O que precisa estar instalado na máquina está abaixo.

## Obrigatórios

| Requisito | Versão | Para quê | Como verificar |
|---|---|---|---|
| **.NET SDK** | **10.0 ou superior** | compilar e executar o projeto (`net10.0`) | `dotnet --version` |
| **Navegador moderno** | Chrome, Edge, Firefox ou Safari recentes | usar a interface web | — |
| **Conexão com a internet** | — | carregar o Leaflet (CDN `unpkg.com`) e as imagens do mapa (OpenStreetMap) | abrir https://tile.openstreetmap.org/0/0/0.png |

Download do .NET SDK: https://dotnet.microsoft.com/download/dotnet/10.0

> O SDK já inclui o runtime. Instalar apenas o "Runtime" **não** basta para
> `dotnet run`.

## Opcionais

| Requisito | Para quê |
|---|---|
| **Git** | clonar o repositório (alternativa: baixar o ZIP no GitHub) |
| **Visual Studio 2026** (workload *ASP.NET e desenvolvimento web*) ou **VS Code** + extensão *C# Dev Kit* | editar/depurar com IDE |

## Sistemas operacionais

Windows 10/11, Linux e macOS (qualquer um com suporte ao .NET 10).

## Portas utilizadas

| Porta | Protocolo | Uso |
|---|---|---|
| `5080` | HTTP | endereço padrão ao rodar `dotnet run` |
| `7080` | HTTPS | perfil `https` (precisa do certificado de desenvolvimento confiável) |

Se alguma porta estiver ocupada, ajuste `applicationUrl` em
`SistemaEntregas/Properties/launchSettings.json`.

## Dependências de código

Nenhum pacote NuGet de terceiros: o projeto usa apenas o framework
`Microsoft.AspNetCore.App` (já incluído no SDK).

Bibliotecas de front-end carregadas por CDN (não precisam ser instaladas):

| Biblioteca | Versão | Origem |
|---|---|---|
| Leaflet | 1.9.4 | `https://unpkg.com/leaflet@1.9.4/` |
| Tiles OpenStreetMap | — | `https://tile.openstreetmap.org/` |

## Não é necessário

- Banco de dados (o estado fica em memória)
- Chave de API de mapas ou de roteamento
- Node.js / npm
- Python
