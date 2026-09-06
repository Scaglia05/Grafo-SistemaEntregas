using System.Diagnostics;
using SistemaEntregas.Modelos;

namespace SistemaEntregas.Servicos;

public class ServicoDijkstra
{
    public ResultadoCaminho Calcular(ServicoGrafo grafo, string origemId, string destinoId, Criterio criterio)
    {
        var resultado = new ResultadoCaminho
        {
            Algoritmo = "Dijkstra",
            CriterioUsado = criterio
        };

        var origem = grafo.BuscarNo(origemId);
        var destino = grafo.BuscarNo(destinoId);
        if (origem is null || destino is null)
        {
            resultado.Mensagem = "Origem ou destino inválido.";
            return resultado;
        }

        var sw = Stopwatch.StartNew();

        var dist = grafo.Nos.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var anterior = new Dictionary<string, string?>();
        var finalizado = new HashSet<string>();

        dist[origemId] = 0;

        var fila = new PriorityQueue<string, double>();
        fila.Enqueue(origemId, 0);

        while (fila.TryDequeue(out var atual, out _))
        {
            if (!finalizado.Add(atual)) continue;
            resultado.NosVisitados++;

            if (atual == destinoId) break;

            foreach (var (rota, vizinho) in grafo.Adjacencias(atual))
            {
                if (finalizado.Contains(vizinho.Id)) continue;

                double candidato = dist[atual] + rota.Peso(criterio);
                if (candidato < dist[vizinho.Id])
                {
                    dist[vizinho.Id] = candidato;
                    anterior[vizinho.Id] = atual;
                    fila.Enqueue(vizinho.Id, candidato);
                }
            }
        }

        sw.Stop();
        resultado.TempoMicrossegundos = sw.Elapsed.TotalMicroseconds;

        if (double.IsPositiveInfinity(dist[destinoId]))
        {
            resultado.Mensagem = $"Cliente inalcançável a partir de {origem.Nome}.";
            return resultado;
        }

        resultado.Encontrado = true;
        resultado.PesoTotal = Math.Round(dist[destinoId], 2);
        resultado.Caminho = CaminhoUtil.Reconstruir(grafo, anterior, origemId, destinoId);
        return resultado;
    }
}
