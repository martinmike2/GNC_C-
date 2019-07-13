using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGC
{
    class Engine
    {

        public Engine(double g0, double throttle, double isp, double thrust = 0, double flow = 0)
        {
            ISP = isp;
            Thrust = thrust;

            if (flow == 0)
            {
                Flow = thrust / (isp * g0) * throttle;
            } else
            {
                Flow = flow;
            }
        }

        public double ISP { get; set; }

        public double Thrust { get; set; }
        public double Flow { get; set; }
    }
}
