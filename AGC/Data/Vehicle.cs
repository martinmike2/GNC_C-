using System.Collections.Generic;
using AGC.Utilities;
using KRPC.Client.Services.SpaceCenter;

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

        public AgcTuple getSolarPrimeVector()
        {
            return Utilities.Utilities.solarPrimeVector(
                    Globals.KrpConnection.SpaceCenter().ActiveVessel.OrbitalReferenceFrame
            );
        }
    }
}
