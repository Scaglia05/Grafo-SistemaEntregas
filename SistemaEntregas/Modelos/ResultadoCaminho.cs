namespace SistemaEntregas.Modelos;

public class ResultadoCaminho
{
    public string Algoritmo { get; set; } = string.Empty;

    public bool Encontrado { get; set; }

    public List<NoGrafo> Caminho { get; set; } = new();

    public double PesoTotal { get; set; }

    public Criterio CriterioUsado { get; set; }

    public int NosVisitados { get; set; }

    public int ArestasRelaxadas { get; set; }

    public double TempoMicrossegundos { get; set; }

    public string? Mensagem { get; set; }
}
