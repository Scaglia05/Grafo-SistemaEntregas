using SistemaEntregas.Modelos;

namespace SistemaEntregas.Servicos;

internal static class CaminhoUtil
{
    public static List<NoGrafo> Reconstruir(
        ServicoGrafo grafo,
        IReadOnlyDictionary<string, string?> anterior,
        string origemId,
        string destinoId)
    {
        var caminho = new List<NoGrafo>();
        var atual = destinoId;

        while (true)
        {
            var no = grafo.BuscarNo(atual);
            if (no is not null) caminho.Insert(0, no);

            if (atual == origemId) break;
            if (!anterior.TryGetValue(atual, out var proximo) || proximo is null) break;
            atual = proximo;
        }

        return caminho;
    }
}
