namespace SistemaEntregas.Modelos;

public class Rota
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    public string OrigemId { get; set; } = string.Empty;

    public string DestinoId { get; set; } = string.Empty;

    public double DistanciaKm { get; set; }

    public decimal Custo { get; set; }

    public bool Bidirecional { get; set; } = true;

    public double Peso(Criterio criterio) =>
        criterio == Criterio.Distancia ? DistanciaKm : (double)Custo;
}
