using AGC.Utilities;

namespace AGC.Data
{
    public class UpfgState
    {
        public CserState Cser { get; set; }
        public AgcTuple Rbias { get; set; }
        public AgcTuple Rd { get; set; }
        public AgcTuple Rgrav { get; set; }
        public double Time { get; set; }
        public double Velocity { get; set; }
        public AgcTuple Vgo { get; set; }
        public double Tgo { get; set; }
        public double Tb { get; set; }
        public double Dt { get; set; }

        public UpfgState(CserState cser, AgcTuple rbias, AgcTuple rd, AgcTuple rgrav, double time, double velocity, AgcTuple vgo, double tb, double tgo, double dt)
        {
            Cser = cser;
            Rbias = rbias;
            Rd = rd;
            Rgrav = rgrav;
            Time = time;
            Velocity = velocity;
            Vgo = vgo;
            Tb = tb;
            Tgo = tgo;
            Dt = dt;
        }
    }
}