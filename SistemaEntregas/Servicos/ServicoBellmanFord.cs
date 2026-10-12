using System.Diagnostics;
using SistemaEntregas.Modelos;

namespace SistemaEntregas.Servicos;

public class ServicoBellmanFord
{
    public ResultadoCaminho Calcular(ServicoGrafo grafo, string origemId, string destinoId, Criterio criterio)
    {
        var resultado = CaminhoUtil.Novo("Bellman-Ford", criterio);
        if (grafo.BuscarNo(origemId) is null || grafo.BuscarNo(destinoId) is null)
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

        for (int i = 0; i < grafo.Nos.Count - 1; i++)
        {
            bool mudou = false;

            foreach (var (de, para, peso) in arestas)
            {
                if (double.IsPositiveInfinity(dist[de])) continue;

                resultado.ArestasRelaxadas++;

                if (dist[de] + peso < dist[para])
                {
                    dist[para] = dist[de] + peso;
                    anterior[para] = de;
                    mudou = true;
                }
            }

            if (!mudou) break;
        }

        bool cicloNegativo = arestas.Any(a =>
            !double.IsPositiveInfinity(dist[a.De]) && dist[a.De] + a.Peso < dist[a.Para]);

        sw.Stop();
        resultado.TempoMicrossegundos = sw.Elapsed.TotalMicroseconds;

        if (cicloNegativo)
        {
            resultado.Mensagem = "Ciclo negativo detectado — menor caminho indefinido.";
            return resultado;
        }

        resultado.NosVisitados = dist.Count(kv => !double.IsPositiveInfinity(kv.Value));

        return CaminhoUtil.Concluir(resultado, grafo, dist, anterior, origemId, destinoId);
    }
}
