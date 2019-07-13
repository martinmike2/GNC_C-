using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGC
{
    class Staging
    {
        private bool jettison;
        private double waitBeforeJettison;
        private bool ignition;
        private double waitBeforeIgnition;
        private string ullage;
        private double ullageBurnTime;
        private double postUllageBurn;

        public Staging(bool jettison, double waitBeforeJettison, bool ignition, double waitBeforeIgnition, string ullage, double ullageBurnDuration = 0, double postUllageBurn = 0)
        {
            this.jettison = jettison;
            this.waitBeforeIgnition = waitBeforeIgnition;
            this.ignition = ignition;
            this.waitBeforeJettison = waitBeforeJettison;
            this.ullage = ullage;
            this.ullageBurnTime = ullageBurnDuration;
            this.postUllageBurn = postUllageBurn;
        }
    }
}
