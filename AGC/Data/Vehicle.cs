using System.Collections.Generic;

namespace AGC.Data
{
    public class Vehicle
    {
        public List<VehicleStage> Stages { get; set; }

        public Vehicle()
        {
            Stages = new List<VehicleStage>();
        }

        public void addVehicleStage(VehicleStage stage) => Stages.Add(stage);
    }
}
