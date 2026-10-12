using SistemaEntregas.Modelos;

namespace SistemaEntregas.Servicos;

internal static class CaminhoUtil
{
    public static ResultadoCaminho Novo(string algoritmo, Criterio criterio) =>
        new() { Algoritmo = algoritmo, CriterioUsado = criterio };

    public static ResultadoCaminho Concluir(
        ResultadoCaminho resultado,
        ServicoGrafo grafo,
        Dictionary<string, double> dist,
        Dictionary<string, string?> anterior,
        string origemId,
        string destinoId)
    {
        if (double.IsPositiveInfinity(dist[destinoId]))
        {
            resultado.Mensagem =
                $"Não há caminho de {grafo.BuscarNo(origemId)?.Nome} até {grafo.BuscarNo(destinoId)?.Nome}.";
            return resultado;
        }

        resultado.Encontrado = true;
        resultado.PesoTotal = Math.Round(dist[destinoId], 2);

        for (string? id = destinoId; id is not null; anterior.TryGetValue(id, out id))
            resultado.Caminho.Add(grafo.BuscarNo(id)!);
        resultado.Caminho.Reverse();

        return resultado;
    }
}
