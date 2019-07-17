using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
                
                Globals.SteeringVector = new AgcTuple(1, 0, 0);
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

                Globals.KrpConnection.SpaceCenter().ActiveVessel.AutoPilot.ReferenceFrame =
                    Globals.KrpConnection.SpaceCenter().ActiveVessel.SurfaceReferenceFrame;
                Globals.KrpConnection.SpaceCenter().ActiveVessel.AutoPilot.Engage();
                Globals.KrpConnection.SpaceCenter().ActiveVessel.Control.Throttle = (float)Globals.ThrottleSetting;
                Globals.KrpConnection.SpaceCenter().ActiveVessel.AutoPilot.TargetDirection = Globals.SteeringVector;

                
                
                Console.WriteLine("Preflight Completed");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // Execute a Loop
        public void execute()
        {
            if (Globals.AscentFlag == 0)
            {
                if (Globals.CurrentTime > Globals.LiftOffTime + Globals.ControlData.VerticalAscentTime)
                {
                    pointTo(
                         Globals.MissionData.LaunchAzimuth,
                         90 - Globals.ControlData.PitchOverAngle
                    );
                    Globals.AscentFlag = 1;
                    _eventHandler.addEvent(
                        new Message(
                            new Dictionary<string, string>
                            {
                                {"type", "print"},
                                {"message", $"Pitching over by {Math.Round(Globals.ControlData.PitchOverAngle, 1)} degrees."},
                                {"time", "0"}
                            })
                        );
                    _eventHandler.addEvent(
                        new Message(
                            new Dictionary<string, string>
                            {
                                {"type", "print"},
                                {"message", $"Burning on heading {Math.Round(Globals.MissionData.LaunchAzimuth, 1)} degrees."},
                                {"time", "0"}
                            })
                        );
                }
            } else if (Globals.AscentFlag == 1)
            {
                if (Globals.CurrentTime > Globals.LiftOffTime + Globals.ControlData.VerticalAscentTime + 3)
                {
                    if (Globals.ControlData.PitchOverAngle - getVelocityAngle() < 0.1)
                    {
                        Globals.AscentFlag = 2;
                    }

                    if (Globals.CurrentTime >= Globals.LiftOffTime + Globals.ControlData.VerticalAscentTime +
                        Globals.PitchOverLimit)
                    {
                        Globals.AscentFlag = 2;
                        _eventHandler.addEvent(new Message(new Dictionary<string, string>
                        {
                            {"type", "print"},
                            {"message", "Pitchover time limit exceeded!"},
                            {"time", "0"}
                        }));
                    }
                }
            } else if (Globals.AscentFlag == 2)
            {
                pointTo(Globals.MissionData.LaunchAzimuth, getVelocityAngle());
                _eventHandler.addEvent(new Message(new Dictionary<string, string>
                {
                    {"type", "print"},
                    {"message", $"Holding prograde at {Math.Round(Globals.MissionData.LaunchAzimuth, 1)} deg azimuth."},
                    {"time", "0"}
                }));

                Globals.AscentFlag = 3;
            } else
            {
                pointTo(Globals.MissionData.LaunchAzimuth, getVelocityAngle());
            }

            if (Globals.CurrentTime >=
                Globals.LiftOffTime + Globals.ControlData.UPFGActivation - Globals.UpfgConvergenceDelay)
            {
                _eventHandler.addEvent(new Message(new Dictionary<string, string>
                {
                    {"type", "print"},
                    {"message", "Initiating UPFG!"},
                    {"time", "0"}
                }));
                return;
            }

            Globals.UpfgTarget.Normal =
                Globals.UpfgTarget.calculateNormal(Globals.MissionData.Inclination, Globals.MissionData.Lan);
        }

        private static double getVelocityAngle()
        {
            AgcTuple up = Globals.KrpConnection.SpaceCenter().TransformDirection(
                new AgcTuple(0, 0, -1),
                Globals.KrpConnection.SpaceCenter().ActiveVessel.ReferenceFrame,
                Globals.KrpConnection.SpaceCenter().ActiveVessel.SurfaceReferenceFrame
            );
            var surfaceVel = Globals.KrpConnection.SpaceCenter().ActiveVessel.Velocity(
                Globals.KrpConnection.SpaceCenter().ActiveVessel.SurfaceVelocityReferenceFrame
            );

            var velocityAngle = AgcMath.angle(up, surfaceVel);
            return velocityAngle;
        }


        private static void pointTo(double heading, double pitch)
        {
            Globals.KrpConnection.SpaceCenter().ActiveVessel.AutoPilot.TargetPitchAndHeading((float)pitch, (float)heading);
        }
        private static void pointTo(AgcTuple direction)
        {
            Globals.KrpConnection.SpaceCenter().ActiveVessel.AutoPilot.TargetDirection = direction;
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

            AgcTuple normal = new AgcTuple(0, 0, 0);
            if (Globals.TargetData != null)
            {
                normal = Globals.TargetData.Normal;
            }

            Globals.TargetData = new Target(targetAltitude, targetVelocity, flightPathAngle, normal);
        }
    }
}
