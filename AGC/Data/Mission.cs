namespace AGC.Data
{
    public class Mission
    {

        public Mission(double payload, double apoapsis, double periapsis, double altitude, double inclination,
            double lan, string direction)
        {
            Payload = payload;
            Apoapsis = apoapsis;
            Periapsis = periapsis;
            Altitude = altitude;
            Inclination = inclination;
            Lan = lan;
            Direction = direction;
        }
        
        public double Payload { get; }
        public double Apoapsis { get; }
        public double Periapsis { get; }
        public double Altitude { get; }
        public double Inclination { get; }
        public double Lan { get; }
        public string Direction { get; set; }
        
        public double LaunchAzimuth => 0;
    }
}