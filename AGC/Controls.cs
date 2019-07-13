using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGC
{
    class Controls
    {
        private double launchTimeAdvance;
        private double verticalAscentTime;
        private double pitchOverAngle;
        private double upfgActivation;
        private double launchAzimuth;
        private double initialRoll;

        public Controls(double launchTimeAdvance, double verticalAscentTime, double pitchOverAngle, double upfgActivation, double launchAzimuth = 0, double initialRoll = 0)
        {
            this.launchTimeAdvance = launchTimeAdvance;
            this.verticalAscentTime = verticalAscentTime;
            this.pitchOverAngle = pitchOverAngle;
            this.upfgActivation = upfgActivation;
            this.launchAzimuth = launchAzimuth;
            this.initialRoll = initialRoll;
        }
    }
}
