using System.Collections.Generic;

namespace AGC.Data
{
    public class VehicleStage
    {
        private List<Engine> Engines { get; }

        private Staging Staging { get; set; }

        public double MassTotal { get; }

        private double MassFuel { get; }

        private double MassDry { get; }

        public double GLim { get; }

        private double MinThrottle { get; }

        public double Throttle { get; }

        public double ShutdownRequired { get; set; }

        public double MaxT { get; set; }

        private string Name { get; }
        
        public int UpfgMode { get; }

        public VehicleStage(string name, double massTotal = 0, double massFuel = 0, double massDry = 0, double gLim = 0, double minThrottle = 0, double throttle = 1.0, bool shutdownRequired = true, double payload = 0)
        {
            Name = name;
            GLim = gLim;
            UpfgMode = 1;
            Engines = new List<Engine>();

            MinThrottle = minThrottle;
            if (MinThrottle > 1) { MinThrottle /= 100; }

            Throttle = throttle;
            if (Throttle > 1) { Throttle /= 100; }

            if(massTotal > 0 && massDry > 0)
            {
                MassFuel = massTotal - massDry;
            } else
            {
                MassFuel = massFuel;
            }

            if (massFuel > 0 && massDry > 0)
            {
                MassTotal = massFuel + massDry;
            } else
            {
                MassTotal = massTotal;
            }

            if (massFuel > 0 && massTotal > 0)
            {
                MassDry = massTotal - massFuel;
            } else
            {
                MassDry = massDry;
            }

            if (!(payload > 0)) return;
            MassTotal += payload;
            MassDry += payload;

        }


        public List<double> getThrust(double g0)
        {
            double f = 0;
            double dm = 0;

            foreach (var e in Engines)
            {
                dm += e.Flow;
                f += e.Isp * e.Flow * g0;
            }

            var isp = f / (dm * g0);

            return new List<double>() { f, dm, isp };
        }

        public void addEngine(Engine engine) => Engines.Add(engine);

        public void addStaging(Staging staging) => Staging = staging;
    }
}
