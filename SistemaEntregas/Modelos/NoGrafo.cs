namespace SistemaEntregas.Modelos;

public class NoGrafo
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    public string Nome { get; set; } = string.Empty;

    public TipoNo Tipo { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Rotulo => Tipo == TipoNo.CentroDistribuicao ? $"🏭 {Nome}" : $"📦 {Nome}";
}
