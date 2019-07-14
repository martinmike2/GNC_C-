using AGC.Utilities;

namespace AGC.Data
{
    public class Target
    {
        public double Radius { get; set; }
        public double Velocity { get; set; }
        public double Angle { get; set; }

        public AgcTuple Normal { get; set; }

        public Target(double radius, double velocity, double angle, AgcTuple normal)
        {
            Radius = radius;
            Velocity = velocity;
            Angle = angle;
            Normal = normal;
        }
    }
}