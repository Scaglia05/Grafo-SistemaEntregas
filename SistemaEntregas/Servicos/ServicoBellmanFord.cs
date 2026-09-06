using System.Diagnostics;
using SistemaEntregas.Modelos;

namespace SistemaEntregas.Servicos;

public class ServicoBellmanFord
{
    public ResultadoCaminho Calcular(ServicoGrafo grafo, string origemId, string destinoId, Criterio criterio)
    {
        var resultado = new ResultadoCaminho
        {
            Algoritmo = "Bellman-Ford",
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

        var arestas = grafo.ArestasDirecionadas()
            .Select(a => (a.De, a.Para, Peso: a.Rota.Peso(criterio)))
            .ToList();

        var dist = grafo.Nos.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var anterior = new Dictionary<string, string?>();
        dist[origemId] = 0;

        int totalNos = grafo.Nos.Count;

        for (int i = 0; i < totalNos - 1; i++)
        {
            bool houveMudanca = false;

            foreach (var (de, para, peso) in arestas)
            {
                if (double.IsPositiveInfinity(dist[de])) continue;

                resultado.ArestasRelaxadas++;

                if (dist[de] + peso < dist[para])
                {
                    dist[para] = dist[de] + peso;
                    anterior[para] = de;
                    houveMudanca = true;
                }
            }

            if (!houveMudanca) break;
        }

        foreach (var (de, para, peso) in arestas)
        {
            if (!double.IsPositiveInfinity(dist[de]) && dist[de] + peso < dist[para])
            {
                sw.Stop();
                resultado.TempoMicrossegundos = sw.Elapsed.TotalMicroseconds;
                resultado.Mensagem = "Ciclo negativo detectado — menor caminho indefinido.";
                return resultado;
            }
        }

        sw.Stop();
        resultado.TempoMicrossegundos = sw.Elapsed.TotalMicroseconds;
        resultado.NosVisitados = dist.Count(kv => !double.IsPositiveInfinity(kv.Value));

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
