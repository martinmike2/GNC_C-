using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGC
{
    class Settings
    {
        private UInt16 pitchOverLimit = 20;
        private UInt16 upfgConvergenceDelay = 5;
        private UInt16 upfgFinalizationTime = 5;
        private UInt16 stagingKillRotationTime = 5;
        private float upfgConvergenceCriterion = 0.1f;
        private UInt16 upfgGoodSolutionCriterion = 15;
    }
}
