using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGC
{
    class Vehicle
    {
        private List<VehicleStage> stages;

        public void addVehicleStage(VehicleStage stage) => stages.Add(stage);
    }
}
