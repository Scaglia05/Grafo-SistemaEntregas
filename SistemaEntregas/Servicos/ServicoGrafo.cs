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
    public Rota? BuscarRota(string id) => _rotas.FirstOrDefault(r => r.Id == id);

    public NoGrafo AdicionarNo(NoGrafo no)
    {
        if (string.IsNullOrWhiteSpace(no.Id))
            no.Id = Guid.NewGuid().ToString("N")[..8];
        _nos.Add(no);
        return no;
    }

    public bool RemoverNo(string id)
    {
        var no = BuscarNo(id);
        if (no is null) return false;
        _nos.Remove(no);
        _rotas.RemoveAll(r => r.OrigemId == id || r.DestinoId == id);
        return true;
    }

    public Rota AdicionarRota(Rota rota)
    {
        if (string.IsNullOrWhiteSpace(rota.Id))
            rota.Id = Guid.NewGuid().ToString("N")[..8];

        if (rota.DistanciaKm <= 0)
        {
            var origem = BuscarNo(rota.OrigemId);
            var destino = BuscarNo(rota.DestinoId);
            if (origem is not null && destino is not null)
            {
                rota.DistanciaKm = CalculadoraHaversine.CalcularDistanciaKm(
                    origem.Latitude, origem.Longitude, destino.Latitude, destino.Longitude);
            }
        }

        _rotas.Add(rota);
        return rota;
    }

    public bool RemoverRota(string id)
    {
        var rota = BuscarRota(id);
        if (rota is null) return false;
        _rotas.Remove(rota);
        return true;
    }

    public IEnumerable<(Rota Rota, NoGrafo Destino)> Adjacencias(string noId)
    {
        foreach (var rota in _rotas)
        {
            if (rota.OrigemId == noId)
            {
                var destino = BuscarNo(rota.DestinoId);
                if (destino is not null) yield return (rota, destino);
            }
            else if (rota.Bidirecional && rota.DestinoId == noId)
            {
                var destino = BuscarNo(rota.OrigemId);
                if (destino is not null) yield return (rota, destino);
            }
        }
    }

    public IEnumerable<(string De, string Para, Rota Rota)> ArestasDirecionadas()
    {
        foreach (var rota in _rotas)
        {
            yield return (rota.OrigemId, rota.DestinoId, rota);
            if (rota.Bidirecional)
                yield return (rota.DestinoId, rota.OrigemId, rota);
        }
    }

    private void CarregarSeed()
    {
        AdicionarNo(new NoGrafo { Id = "cd-rp", Nome = "CD Ribeirão Preto", Tipo = TipoNo.CentroDistribuicao, Latitude = -21.1775, Longitude = -47.8103 });
        AdicionarNo(new NoGrafo { Id = "cd-cam", Nome = "CD Campinas", Tipo = TipoNo.CentroDistribuicao, Latitude = -22.9099, Longitude = -47.0626 });
        AdicionarNo(new NoGrafo { Id = "cd-sp", Nome = "CD São Paulo", Tipo = TipoNo.CentroDistribuicao, Latitude = -23.5505, Longitude = -46.6333 });

        AdicionarNo(new NoGrafo { Id = "cl-fra", Nome = "Franca", Tipo = TipoNo.Cliente, Latitude = -20.5386, Longitude = -47.4008 });
        AdicionarNo(new NoGrafo { Id = "cl-sca", Nome = "São Carlos", Tipo = TipoNo.Cliente, Latitude = -22.0175, Longitude = -47.8908 });
        AdicionarNo(new NoGrafo { Id = "cl-ara", Nome = "Araraquara", Tipo = TipoNo.Cliente, Latitude = -21.7845, Longitude = -48.1758 });
        AdicionarNo(new NoGrafo { Id = "cl-pir", Nome = "Piracicaba", Tipo = TipoNo.Cliente, Latitude = -22.7253, Longitude = -47.6492 });
        AdicionarNo(new NoGrafo { Id = "cl-lim", Nome = "Limeira", Tipo = TipoNo.Cliente, Latitude = -22.5641, Longitude = -47.4017 });
        AdicionarNo(new NoGrafo { Id = "cl-jun", Nome = "Jundiaí", Tipo = TipoNo.Cliente, Latitude = -23.1857, Longitude = -46.8978 });
        AdicionarNo(new NoGrafo { Id = "cl-sor", Nome = "Sorocaba", Tipo = TipoNo.Cliente, Latitude = -23.5015, Longitude = -47.4526 });
        AdicionarNo(new NoGrafo { Id = "cl-bau", Nome = "Bauru", Tipo = TipoNo.Cliente, Latitude = -22.3147, Longitude = -49.0606 });

        var seed = new (string Origem, string Destino, decimal Custo)[]
        {
            ("cd-rp",  "cl-fra", 120m),
            ("cd-rp",  "cl-sca", 150m),
            ("cd-rp",  "cl-ara", 130m),
            ("cl-ara", "cl-sca",  60m),
            ("cl-sca", "cl-pir", 110m),
            ("cl-ara", "cl-bau", 180m),
            ("cd-cam", "cl-pir",  90m),
            ("cd-cam", "cl-lim",  70m),
            ("cl-lim", "cl-pir",  50m),
            ("cl-pir", "cd-rp",  220m),
            ("cd-cam", "cl-jun",  80m),
            ("cl-jun", "cd-sp",   90m),
            ("cd-cam", "cl-sor", 130m),
            ("cl-sor", "cd-sp",  110m),
            ("cl-jun", "cl-sor", 100m),
            ("cd-cam", "cd-sp",  160m),
            ("cl-bau", "cd-cam", 240m),
        };

        foreach (var (origem, destino, custo) in seed)
            AdicionarRota(new Rota { OrigemId = origem, DestinoId = destino, Custo = custo });
    }
}
