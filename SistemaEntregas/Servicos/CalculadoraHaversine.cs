namespace SistemaEntregas.Servicos;

public static class CalculadoraHaversine
{
    private const double RaioTerraKm = 6371.0;

    public static double CalcularDistanciaKm(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = GrausParaRad(lat2 - lat1);
        double dLon = GrausParaRad(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                 + Math.Cos(GrausParaRad(lat1)) * Math.Cos(GrausParaRad(lat2))
                 * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return Math.Round(RaioTerraKm * c, 1);
    }

    private static double GrausParaRad(double graus) => graus * Math.PI / 180.0;
}
