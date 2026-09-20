namespace BSC.Api
{
    /* Modelo de pronóstico del clima de ejemplo generado por la plantilla de API. */
    public class WeatherForecast
    {
        /* Fecha del pronóstico meteorológico. */
        public DateOnly Date { get; set; }

        /* Temperatura registrada en grados Celsius. */
        public int TemperatureC { get; set; }

        /* Temperatura equivalente calculada en grados Fahrenheit. */
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /* Descripción textual resumida del estado del clima. */
        public string? Summary { get; set; }
    }
}
