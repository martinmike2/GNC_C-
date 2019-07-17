using AGC.Utilities;

namespace AGC.Data
{
    public class Target
    {
        public double Radius { get; set; }
        public double Velocity { get; set; }
        public double Angle { get; set; }

        public AgcTuple Normal { get; set; }

        public Target(double radius, double velocity, double angle, AgcTuple normal)
        {
            Radius = radius;
            Velocity = velocity;
            Angle = angle;
            Normal = normal;
        }

        public AgcTuple calculateNormal(double missionDataInclination, double missionDataLan)
        {
            var highPoint = Utilities.Utilities.rodrigues(
                Globals.VehicleData.getSolarPrimeVector(),
                new AgcTuple(0, 1, 0),
                90 - missionDataLan
            );
            
            var rotationAxis = new AgcTuple(
                -highPoint.Z,
                highPoint.Y,
                highPoint.X
                );

            var normal = Utilities.Utilities.rodrigues(highPoint, rotationAxis, 90 - missionDataInclination);

            return -1 * Utilities.Utilities.vecYZ(normal);
        }
    }
}