using System;
using AGC.Data;
using KRPC.Client.Services.SpaceCenter;

namespace AGC.Utilities
{
    public static class Utilities
    {
        public static AgcTuple solarPrimeVector(ReferenceFrame referenceFrame)
        {
            var sun = Globals.KrpConnection.SpaceCenter().Bodies["Sun"];
            var secondsPerDegree = sun.RotationalPeriod / 360;
            var rotationOffset = (Globals.KrpConnection.SpaceCenter().UT / secondsPerDegree) % 360;
            AgcTuple sunPosition = sun.Position(referenceFrame);
            AgcTuple sunPosition2 = sun.SurfacePosition(0, 0 - rotationOffset, referenceFrame);


            var primeVector = sunPosition2 - sunPosition;

            return AgcMath.normalize(primeVector);

        }

        public static AgcTuple aimAndRoll(AgcTuple aimingVector, double rollAngle)
        {
            AgcTuple up = Globals.KrpConnection.SpaceCenter().TransformDirection(
                new AgcTuple(0, 0, -1),
                Globals.KrpConnection.SpaceCenter().ActiveVessel.ReferenceFrame,
                Globals.KrpConnection.SpaceCenter().ActiveVessel.SurfaceReferenceFrame
            );

            AgcTuple rollVector = rodrigues(up, aimingVector, -rollAngle);

            return aimingVector * rollVector;
        }

        public static AgcTuple nodeVector(double inc, string direction)
        {
            var b = Math.Tan(90 - inc) *
                       Math.Tan(Globals.KrpConnection.SpaceCenter().ActiveVessel.Flight().Latitude);
            
            b = Math.Asin(Math.Min(Math.Max(-1, b), 1));
            var yVec = new AgcTuple(0, 1, 0);

            var longitudeVector = AgcMath.map(
                yVec,
                AgcMath.normalize(Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.Position(
                    Globals.KrpConnection.SpaceCenter().ActiveVessel.ReferenceFrame
                ))
            );

            switch (direction)
            {
                case "north":
                    return rodrigues(longitudeVector, yVec, b);
                case "south":
                    return rodrigues(longitudeVector, yVec, 180 - b);
                default:
                    return nodeVector(inc, "north");
            }
        }

        public static AgcTuple rodrigues(AgcTuple inVector, AgcTuple axis, double angle)
        {
            axis = AgcMath.normalize(axis);
            var outVector = inVector * Math.Cos(angle);
            outVector += AgcMath.cross(axis, inVector) * Math.Sin(angle);
            outVector += axis * AgcMath.dot(axis, inVector) * (1 - Math.Cos(angle));

            return outVector;
        }

        public static double orbitInterceptTime(string direction)
        {
            var targetInc = Globals.MissionData.Inclination;
            var targetLan = Globals.MissionData.Lan;

            if (direction == "nearest")
            {
                var timeToNortherly = orbitInterceptTime("north");
                var timeToSoutherly = orbitInterceptTime("south");
                double time;

                if (timeToSoutherly < timeToNortherly)
                {
                    Globals.MissionData.Direction = "south";
                    time = timeToSoutherly;
                }
                else
                {
                    Globals.MissionData.Direction = "north";
                    time = timeToNortherly;
                }
                return time;
            }

            Globals.CurrentNode = nodeVector(targetInc, direction);
            AgcTuple targetNode = rodrigues(Globals.VehicleData.getSolarPrimeVector(), new AgcTuple(0, 1, 0), -targetLan);
            var nodeDelta = AgcMath.angle(Globals.CurrentNode, targetNode);
            var deltaDir = AgcMath.dot(new AgcTuple(0, 1, 0), AgcMath.cross(targetNode, Globals.CurrentNode));

            if (deltaDir < 0)
            {
                nodeDelta = 360 - nodeDelta;
            }

            var deltatime = Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.RotationalPeriod *
                               nodeDelta / 360;

            return deltatime;
        }

        public static AgcTuple vecYZ(AgcTuple input)
        {
            return new AgcTuple(input.X, input.Z, input.Y);
        }

        public static double launchAzimuth()
        {
            var siteLat = Globals.KrpConnection.SpaceCenter().ActiveVessel.Flight().Latitude;

            var bInertial = Math.Cos(Globals.MissionData.Inclination) / Math.Cos(siteLat);

            if (bInertial < -1)
            {
                bInertial = -1;
            }

            if (bInertial > 1)
            {
                bInertial = 1;
            }

            bInertial = Math.Asin(bInertial);
            var vOrbit = Globals.TargetData.Velocity * Math.Cos(Globals.TargetData.Angle);
            var vBody =
                (2 * Math.PI * Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.EquatorialRadius /
                 Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.RotationalPeriod) * Math.Cos(siteLat);
            var vrotx = vOrbit * Math.Sin(bInertial) - vBody;
            var vroty = vOrbit * Math.Cos(bInertial);
            var azimuth = Math.Atan2(vroty, vrotx);

            switch (Globals.MissionData.Direction)
            {
                case "north":
                    return 90 - azimuth;
                case "south":
                    return 90 + azimuth;
                default:
                    return 90 - azimuth;
            }

        }
    }
}