using SistemaEntregas.Modelos;

namespace SistemaEntregas.Servicos;

public class ServicoGrafo
{
    private readonly List<NoGrafo> _nos = new();
    private readonly List<Rota> _rotas = new();

    public ServicoGrafo() => CarregarSeed();

    public IReadOnlyList<NoGrafo> Nos => _nos;
    public IReadOnlyList<Rota> Rotas => _rotas;

    public IEnumerable<NoGrafo> Centros => _nos.Where(n => n.Tipo == TipoNo.CentroDistribuicao);
    public IEnumerable<NoGrafo> Clientes => _nos.Where(n => n.Tipo == TipoNo.Cliente);

    public NoGrafo? BuscarNo(string id) => _nos.FirstOrDefault(n => n.Id == id);

    public void AdicionarNo(NoGrafo no) => _nos.Add(no);

    public void RemoverNo(string id)
    {
        _nos.RemoveAll(n => n.Id == id);
        _rotas.RemoveAll(r => r.OrigemId == id || r.DestinoId == id);
    }

    public void AdicionarRota(Rota rota)
    {
        if (rota.DistanciaKm <= 0 && BuscarNo(rota.OrigemId) is { } a && BuscarNo(rota.DestinoId) is { } b)
            rota.DistanciaKm = CalculadoraHaversine.CalcularDistanciaKm(a.Latitude, a.Longitude, b.Latitude, b.Longitude);

        _rotas.Add(rota);
    }

    public void RemoverRota(string id) => _rotas.RemoveAll(r => r.Id == id);

    public IEnumerable<(string De, string Para, Rota Rota)> ArestasDirecionadas()
    {
        foreach (var rota in _rotas)
        {
            yield return (rota.OrigemId, rota.DestinoId, rota);
            if (rota.Bidirecional)
                yield return (rota.DestinoId, rota.OrigemId, rota);
        }
    }

    public IEnumerable<(Rota Rota, string Vizinho)> Adjacencias(string noId) =>
        ArestasDirecionadas().Where(a => a.De == noId).Select(a => (a.Rota, a.Para));

    private static NoGrafo Centro(string id, string nome, double lat, double lng) =>
        new() { Id = id, Nome = nome, Tipo = TipoNo.CentroDistribuicao, Latitude = lat, Longitude = lng };

    private static NoGrafo Cliente(string id, string nome, double lat, double lng) =>
        new() { Id = id, Nome = nome, Tipo = TipoNo.Cliente, Latitude = lat, Longitude = lng };

    private void CarregarSeed()
    {
        _nos.AddRange(
        [
            Centro("cd-rp", "CD Ribeirão Preto", -21.1775, -47.8103),
            Centro("cd-cam", "CD Campinas", -22.9099, -47.0626),
            Centro("cd-sp", "CD São Paulo", -23.5505, -46.6333),

            Cliente("cl-fra", "Franca", -20.5386, -47.4008),
            Cliente("cl-sca", "São Carlos", -22.0175, -47.8908),
            Cliente("cl-ara", "Araraquara", -21.7845, -48.1758),
            Cliente("cl-pir", "Piracicaba", -22.7253, -47.6492),
            Cliente("cl-lim", "Limeira", -22.5641, -47.4017),
            Cliente("cl-jun", "Jundiaí", -23.1857, -46.8978),
            Cliente("cl-sor", "Sorocaba", -23.5015, -47.4526),
            Cliente("cl-bau", "Bauru", -22.3147, -49.0606),
        ]);

        (string De, string Para, double Custo)[] rotas =
        [
            ("cd-rp", "cl-fra", 120),
            ("cd-rp", "cl-sca", 150),
            ("cd-rp", "cl-ara", 130),
            ("cl-ara", "cl-sca", 60),
            ("cl-sca", "cl-pir", 110),
            ("cl-ara", "cl-bau", 180),
            ("cd-cam", "cl-pir", 90),
            ("cd-cam", "cl-lim", 70),
            ("cl-lim", "cl-pir", 50),
            ("cl-pir", "cd-rp", 220),
            ("cd-cam", "cl-jun", 80),
            ("cl-jun", "cd-sp", 90),
            ("cd-cam", "cl-sor", 130),
            ("cl-sor", "cd-sp", 110),
            ("cl-jun", "cl-sor", 100),
            ("cd-cam", "cd-sp", 160),
            ("cl-bau", "cd-cam", 240),
        ];

        foreach (var (de, para, custo) in rotas)
            AdicionarRota(new Rota { OrigemId = de, DestinoId = para, Custo = custo });
    }
}
