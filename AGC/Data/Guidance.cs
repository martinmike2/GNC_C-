using AGC.Utilities;

namespace AGC.Data
{
    public class Guidance
    {
        public AgcTuple If { get; set; }
        public double Pitch { get; set; }
        public double Yaw { get; set; }
        public double PitchDot { get; set; }
        public double YawDot{ get; set; }
        public double Tgo { get; set; }

        public Guidance(AgcTuple @if, double pitch, double yaw, double pitchDot, double yawDot, double tgo)
        {
            If = @if;
            Pitch = pitch;
            Yaw = yaw;
            PitchDot = pitchDot;
            YawDot = yawDot;
            Tgo = tgo;
        }
    }
}