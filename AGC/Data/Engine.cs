using System;

namespace AGC.Data
{
    public class Engine
    {

        public Engine(double g0, double throttle, double isp, double thrust = 0, double flow = 0)
        {
            Isp = isp;
            Thrust = thrust;

            if (Math.Abs(flow) < 0.0000001)
            {
                Flow = thrust / (isp * g0) * throttle;
            } else
            {
                Flow = flow;
            }
        }

        public double Isp { get; }

        private double Thrust { get; }
        public double Flow { get; }
    }
}
