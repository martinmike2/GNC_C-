using AGC.Utilities;

namespace AGC.Data
{
    public class VehicleState
    {
        public double Time { set; get; }
        public double Mass { get; set; }
        public double Velocity { get; set; }
        public AgcTuple Radius { get; set; }

        public VehicleState(double time, double mass, double velocity, AgcTuple radius)
        {
            Time = time;
            Mass = mass;
            Velocity = velocity;
            Radius = radius;
        }
    }
}