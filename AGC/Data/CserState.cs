using System.Security.Cryptography.X509Certificates;
using AGC.Utilities;

namespace AGC.Data
{
    public class CserState
    {
        public AgcTuple Radius { get; set; }
        public AgcTuple Velocity { get; set; }
        public CserState Previous { get; set; }
        public double Dtcp { get; set; }
        public double Xcp { get; set; }
        public double A { get; set; }
        public double D { get; set; }
        public double E { get; set; }

        public CserState(AgcTuple radius, AgcTuple velocity, double dtcp, double xcp, double a, double d, double e, CserState previous)
        {
            Radius = radius;
            Velocity = velocity;
            Dtcp = dtcp;
            Xcp = xcp;
            A = a;
            D = d;
            E = e;
            Previous = previous;
        }
    }
}