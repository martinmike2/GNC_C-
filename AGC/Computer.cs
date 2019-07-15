using System;
using AGC.Data;
using AGC.Events;
using AGC.Utilities;
using KRPC.Client.Services.SpaceCenter;
using ArgumentException = KRPC.Client.Services.KRPC.ArgumentException;


namespace AGC
{
    public class Computer
    {
        private static Handler _eventHandler;

        public Computer(Handler eventHandler)
        {
            if (!preflight())
            {
                throw new ArgumentException();
            }

            _eventHandler = eventHandler;
        }
        
        private bool preflight()
        {
            try
            {
                var facing = Globals.KrpConnection.SpaceCenter().ActiveVessel.Direction(
                    Globals.KrpConnection.SpaceCenter().ActiveVessel.SurfaceReferenceFrame
                );
                /*Globals.SteeringVector = AgcMath.cross(
                    Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body
                        .Position(Globals.KrpConnection.SpaceCenter().ActiveVessel.ReferenceFrame),
                    Globals.KrpConnection.SpaceCenter().ActiveVessel.Velocity(Globals.KrpConnection.SpaceCenter().ActiveVessel.ReferenceFrame)
                );*/
                Globals.SteeringVector = facing;
                Globals.CurrentTime = Globals.KrpConnection.SpaceCenter().UT;
                Globals.TimeToOrbitIntercept = Utilities.Utilities.orbitInterceptTime(Globals.MissionData.Direction);
                Globals.LiftOffTime = Globals.CurrentTime + Globals.TimeToOrbitIntercept - Globals.ControlData.LaunchTimeAdvance;

                if (Globals.TimeToOrbitIntercept < Globals.ControlData.LaunchTimeAdvance)
                {
                    Globals.LiftOffTime += Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.RotationalPeriod;
                }
            
                setupTarget();

                if (Math.Abs(Globals.MissionData.LaunchAzimuth) < 0.0000001)
                {
                    Globals.ControlData.LaunchAzimuth = Utilities.Utilities.launchAzimuth();
                }
                if (Globals.ControlData.InitialRoll > 0)
                {
                    Globals.KrpConnection.SpaceCenter().ActiveVessel.AutoPilot.TargetRoll = (float) Globals.ControlData.InitialRoll;
                }
                else
                {
                    Globals.KrpConnection.SpaceCenter().ActiveVessel.AutoPilot.TargetRoll = 0f;
                }
            
                Globals.KrpConnection.SpaceCenter().ActiveVessel.AutoPilot.Engage();
                Globals.KrpConnection.SpaceCenter().ActiveVessel.Control.Throttle = (float)Globals.ThrottleSetting;
                Globals.KrpConnection.SpaceCenter().ActiveVessel.AutoPilot.TargetDirection = Globals.SteeringVector;
                var activeVessel = Globals.KrpConnection.SpaceCenter().ActiveVessel;


                Console.WriteLine("Preflight Completed");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
         
        public static void onTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            Globals.CurrentTime = Globals.KrpConnection.SpaceCenter().UT;
            _eventHandler.execute();

            if (!Globals.LiftOffFlag)
            {
                var t = TimeSpan.FromSeconds( Globals.LiftOffTime - Globals.CurrentTime );

                var answer = $"{t.Hours:D2}h:{t.Minutes:D2}m:{t.Seconds:D2}s";
         
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearCurrentConsoleLine();
                Console.WriteLine("Time to Launch: {0}", answer);
            }
            
            void ClearCurrentConsoleLine()
            {
                var currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth)); 
                Console.SetCursorPosition(0, currentLineCursor);
            }
        }

        private static void setupTarget()
        {
            var pe = Globals.MissionData.Periapsis * 1000 +
                        Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.EquatorialRadius;
             
            var ap = Globals.MissionData.Apoapsis * 1000 +
                        Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.EquatorialRadius;
             
            var targetAltitude = Globals.MissionData.Altitude * 1000 +
                                    Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.EquatorialRadius;
             
            var sma = (pe + ap) / 2;
            var vpe = Math.Sqrt(
                Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.GravitationalParameter * (2 / pe - 1 / sma)
            );
            var srm = pe * vpe;
            var targetVelocity = Math.Sqrt(
                Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.GravitationalParameter *
                (2 / targetAltitude - 1 / sma)
            );
            var flightPathAngle = Math.Acos(srm / (targetVelocity * targetAltitude));
            
            
            Globals.TargetData = new Target(targetAltitude, targetVelocity, flightPathAngle, new AgcTuple(0, 0, 0));
        }
    }
}
