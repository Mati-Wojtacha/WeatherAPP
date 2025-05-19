namespace WeatherApi.Models
{
    public class City
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public Coordinate? Coord { get; set; }

    }

    public class Coordinate
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
    }
}
