using System.Diagnostics;
using SistemaEntregas.Modelos;

namespace SistemaEntregas.Servicos;

public class ServicoDijkstra
{
    public ResultadoCaminho Calcular(ServicoGrafo grafo, string origemId, string destinoId, Criterio criterio)
    {
        var resultado = CaminhoUtil.Novo("Dijkstra", criterio);
        if (grafo.BuscarNo(origemId) is null || grafo.BuscarNo(destinoId) is null)
        {
            resultado.Mensagem = "Origem ou destino inválido.";
            return resultado;
        }

        var sw = Stopwatch.StartNew();

        var dist = grafo.Nos.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var anterior = new Dictionary<string, string?>();
        var fila = new PriorityQueue<string, double>();

        dist[origemId] = 0;
        fila.Enqueue(origemId, 0);

        while (fila.TryDequeue(out var atual, out var distAtual))
        {
            if (distAtual > dist[atual]) continue;

            resultado.NosVisitados++;
            if (atual == destinoId) break;

            foreach (var (rota, vizinho) in grafo.Adjacencias(atual))
            {
                resultado.ArestasRelaxadas++;

                double candidato = distAtual + rota.Peso(criterio);
                if (candidato < dist[vizinho])
                {
                    dist[vizinho] = candidato;
                    anterior[vizinho] = atual;
                    fila.Enqueue(vizinho, candidato);
                }
            }
        }

        sw.Stop();
        resultado.TempoMicrossegundos = sw.Elapsed.TotalMicroseconds;

        return CaminhoUtil.Concluir(resultado, grafo, dist, anterior, origemId, destinoId);
    }
}
