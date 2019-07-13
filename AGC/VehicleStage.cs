using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGC
{
    class VehicleStage
    {
        private List<Engine> engines;
        private Staging staging;

        private double massTotal;
        private double massFuel;
        private double massDry;
        private double gLim;
        private double minThrottle;
        private double throttle;
        private double shutdownRequired;
        private double maxT;
        private string name;

        public VehicleStage(string name, double massTotal = 0, double massFuel = 0, double massDry = 0, double gLim = 0, double minThrottle = 0, double throttle = 1.0, bool shutdownRequired = true, double payload = 0)
        {
            this.name = name;
            this.gLim = gLim;

            this.minThrottle = minThrottle;
            if (minThrottle > 1) { this.minThrottle /= 100; }

            this.throttle = throttle;
            if (throttle > 1) { this.minThrottle /= 100; }

            if(massTotal > 0 && massDry > 0)
            {
                this.massFuel = massTotal - massDry;
            } else
            {
                this.massFuel = massFuel;
            }

            if (massFuel > 0 && massDry > 0)
            {
                this.massTotal = massFuel + massDry;
            } else
            {
                this.massTotal = massTotal;
            }

            if (massFuel > 0 && massTotal > 0)
            {
                this.massDry = massTotal - massFuel;
            } else
            {
                this.massDry = massDry;
            }

            if (payload > 0)
            {
                this.massTotal += payload;
                this.massDry += payload;
            }

        }


        public List<double> getThrust(double g0)
        {
            double F = 0;
            double dm = 0;

            foreach (Engine e in engines)
            {
                dm = dm + e.Flow;
                F = F + e.ISP * e.Flow * g0;
            }

            double isp = F / (dm * g0);

            return new List<double>() { F, dm, isp };
        }

        public void addEngine(Engine engine) => engines.Add(engine);

        public void addStaging(Staging staging) => this.staging = staging;
    }
}
