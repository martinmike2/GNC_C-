
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using AGC.Data;
using AGC.Events;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;
using Engine = AGC.Data.Engine;

namespace AGC
{

    internal static class Program
    {
        private static Handler _eventHandler;
        private static string uri = "test_vehicle.xml";

        static void Main()
        {
            Globals.KrpConnection = new Connection(
                name: "My Example",
                address: IPAddress.Parse("192.168.1.105"),
                rpcPort: 1000,
                streamPort: 1001
            );

            loadData();
            setupGlobals();
            _eventHandler = new Handler();
            var _computer = new Computer(_eventHandler);
            setSystemEvents();
            setupSequence();
            startTimer();
            
            do {
                while (! Console.KeyAvailable) {
                    // Do something
                }       
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            
        }

        private static void startTimer()
        {
            System.Timers.Timer loopTimer = new System.Timers.Timer();
            loopTimer.Elapsed += Computer.onTimedEvent;
            loopTimer.Interval = 1000;
            loopTimer.Enabled = true;
        }

        private static void setupGlobals()
        {
            
            Globals.G0 = Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.SurfaceGravity;
            Globals.UpfgStage = -1;
            Globals.ThrottleSetting = 1;
            Globals.ThrottleDisplay = 1;
            Globals.SteeringRoll = 0;
            Globals.UpfgConverged = false;
            Globals.StagingInProgress = false;
            Globals.LiftOffFlag = false;

        }

        private static void setSystemEvents()
        {
            var timeToLaunch = Globals.LiftOffTime - Globals.CurrentTime;

            if (timeToLaunch > 18000)
            {
                _eventHandler.addEvent(new Message(new Dictionary<string, string>()
                {
                    {"type", "print"},
                    {"time", "-18000"},
                    {"message", "5 Hours to launch"}
                }));
            }

            if (timeToLaunch > 3600)
            {
                _eventHandler.addEvent(new Message(new Dictionary<string, string>()
                {
                    {"type", "print"},
                    {"time", "-3600"},
                    {"message", "1 Hour to launch"}
                }));
            }

            if (timeToLaunch > 1800)
            {
                _eventHandler.addEvent(new Message(new Dictionary<string, string>()
                {
                    {"type", "print"},
                    {"time", "-1800"},
                    {"message", "30 Minutes to launch"}
                }));
            }

            if (timeToLaunch > 600)
            {

                _eventHandler.addEvent(new Message(new Dictionary<string, string>()
                {
                    {"type", "print"},
                    {"time", "-600"},
                    {"message", "10 Minutes to launch"}
                }));
            }

            if (timeToLaunch > 300)
            {

                _eventHandler.addEvent(new Message(new Dictionary<string, string>()
                {
                    {"type", "print"},
                    {"time", "-300"},
                    {"message", "5 Minutes to launch"}
                }));
            }

            if (timeToLaunch > 60)
            {

                _eventHandler.addEvent(new Message(new Dictionary<string, string>()
                {
                    {"type", "print"},
                    {"time", "-60"},
                    {"message", "1 Minute to launch"}
                }));
            }

            if (timeToLaunch > 30)
            {

                _eventHandler.addEvent(new Message(new Dictionary<string, string>()
                {
                    {"type", "print"},
                    {"time", "-30"},
                    {"message", "30 Seconds to launch"}
                }));
            }

            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-10"},
                {"message", "10 Seconds to launch"}
            }));
            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-9"},
                {"message", "9 Seconds to launch"}
            }));
            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-8"},
                {"message", "8 Seconds to launch"}
            }));
            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-7"},
                {"message", "7 Seconds to launch"}
            }));
            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-6"},
                {"message", "6 Seconds to launch"}
            }));
            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-5"},
                {"message", "5 Seconds to launch"}
            }));
            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-4"},
                {"message", "4 Seconds to launch"}
            }));
            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-3"},
                {"message", "3 Seconds to launch"}
            }));
            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-2"},
                {"message", "2 Seconds to launch"}
            }));
            _eventHandler.addEvent(new Message(new Dictionary<string, string>()
            {
                {"type", "print"},
                {"time", "-1"},
                {"message", "1 Second to launch"}
            }));
        }

        private static void loadData()
        {
            var doc = getDataPackage();

            var controlsNode = doc.DocumentElement?.SelectSingleNode("/data/controls");
            var vehicleNode = doc.DocumentElement?.SelectSingleNode("/data/vehicle");
            var missionNode = doc.DocumentElement?.SelectSingleNode("/data/mission");

            buildControls(controlsNode);
            buildMission(missionNode);
            buildVehicle(vehicleNode);
        }

        private static XmlDocument getDataPackage()
        {
            var data = new XmlDocument();
            data.Load(uri);
            return data;
        }

        private static void buildControls(XmlNode controlNode)
        {
            double.TryParse(controlNode.Attributes?.GetNamedItem("launchAzimuth").Value, out var launchAzimuth);
            double.TryParse(controlNode.Attributes?.GetNamedItem("initialRoll").Value, out var initialRoll);
            double.TryParse(controlNode.Attributes?.GetNamedItem("launchTimeAdvance").Value, out var launchTimeAdvance);
            double.TryParse(controlNode.Attributes?.GetNamedItem("verticalAscentTime").Value, out var verticalAscentTime);
            double.TryParse(controlNode.Attributes?.GetNamedItem("pitchOverAngle").Value, out var pitchOverAngle);
            double.TryParse(controlNode.Attributes?.GetNamedItem("upfgActivation").Value, out var upfgActivation);

            Globals.ControlData = new Controls(launchTimeAdvance, verticalAscentTime, pitchOverAngle, upfgActivation, launchAzimuth,
                initialRoll);
        }

        private static void buildMission(XmlNode missionNode)
        {
            var direction = missionNode.Attributes?.GetNamedItem("direction").Value;

            double.TryParse(missionNode.Attributes?.GetNamedItem("payload").Value, out var payload);
            double.TryParse(missionNode.Attributes?.GetNamedItem("apoapsis").Value, out var apoapsis);
            double.TryParse(missionNode.Attributes?.GetNamedItem("periapsis").Value, out var periapsis);
            double.TryParse(missionNode.Attributes?.GetNamedItem("altitude").Value, out var altitude);
            double.TryParse(missionNode.Attributes?.GetNamedItem("inclination").Value, out var inclination);
            double.TryParse(missionNode.Attributes?.GetNamedItem("LAN").Value, out var lan);

            if (Globals.ControlData.LaunchAzimuth > 0) { direction = "nearest"; }
            if (direction == "") { direction = "north"; }
            if (Math.Abs(lan) < 0.0000001 && direction == "nearest") { direction = "north"; }
            if (Math.Abs(inclination) < 0.0000001)
            {
                inclination = Globals.KrpConnection.SpaceCenter().ActiveVessel.Flight().Latitude;
            }

            if (altitude < periapsis || altitude > apoapsis)
            {
                altitude = periapsis;}

            if (lan > 0) { lan += 360; }

            if (lan > 360) { lan = lan % 360; }

            Globals.Solarprimevector = Utilities.Utilities.solarPrimeVector(
                Globals.KrpConnection.SpaceCenter().ActiveVessel.OrbitalReferenceFrame);

            Globals.CurrentNode = Utilities.Utilities.nodeVector(inclination, direction);
            var currentLan = Utilities.AgcMath.angle(Globals.CurrentNode, Globals.Solarprimevector);

            if (Utilities.AgcMath.dot(new Utilities.AgcTuple(0,1,0), Utilities.AgcMath.cross(Globals.CurrentNode, Globals.Solarprimevector)) < 0)
            {
                currentLan = 360 - currentLan;
            }

            lan = currentLan + (Globals.ControlData.LaunchTimeAdvance + 30) /
                  Globals.KrpConnection.SpaceCenter().ActiveVessel.Orbit.Body.RotationalPeriod * 360;


            Globals.MissionData = new Mission(payload, apoapsis, periapsis, altitude, inclination, lan, direction);
        }

        private static void buildVehicle(XmlNode vehicleNode)
        {
            var vehicle = new Vehicle();
            var stages = vehicleNode.ChildNodes;

            foreach (XmlNode stage in stages)
            {
                var name = stage.Attributes?.GetNamedItem("name").Value;

                double.TryParse(stage.Attributes?.GetNamedItem("massTotal").Value, out var massTotal);
                double.TryParse(stage.Attributes?.GetNamedItem("massFuel").Value, out var massFuel);
                double.TryParse(stage.Attributes?.GetNamedItem("massDry").Value, out var massDry);
                double.TryParse(stage.Attributes?.GetNamedItem("gLim").Value, out var gLim);
                double.TryParse(stage.Attributes?.GetNamedItem("minThrottle").Value, out var minThrottle);
                double.TryParse(stage.Attributes?.GetNamedItem("throttle").Value, out var throttle);
                bool.TryParse(stage.Attributes?.GetNamedItem("shutdownRequired").Value, out var shutdownRequired);

                var stg = new VehicleStage(name, massTotal, massFuel, massDry, gLim, minThrottle, throttle, shutdownRequired, Globals.MissionData.Payload);

                var children = stage.ChildNodes;

                var engines = new List<XmlNode>();

                foreach (XmlNode child in children)
                {
                    if (child.LocalName != "engines") continue;
                    
                    foreach (XmlNode engine in child.ChildNodes)
                    {
                        engines.Add(engine);
                    }
                }

                foreach (var engine in engines)
                {
                    double.TryParse(engine.Attributes?.GetNamedItem("isp").Value, out var isp);
                    double.TryParse(engine.Attributes?.GetNamedItem("thrust").Value, out var thrust);
                    double.TryParse(engine.Attributes?.GetNamedItem("flow").Value, out var flow);

                    var eng = new Engine(Globals.G0, stg.Throttle, isp, thrust, flow);

                    stg.addEngine(eng);
                }
                
                vehicle.addVehicleStage(stg);
            }

            Globals.VehicleData = vehicle;
        }
        

        private static void setupSequence()
        {
            var doc = getDataPackage();
            var sequencingNode = doc.DocumentElement?.SelectSingleNode("/data/sequences");
            var factory = new EventFactory();

            var typename = "message";

            if (sequencingNode?.ChildNodes == null) return;
            
            foreach (XmlNode sequence in sequencingNode.ChildNodes)
            {
                var data = new Dictionary<string, string>();

                if (sequence.Attributes != null)
                    foreach (XmlAttribute attrib in sequence.Attributes)
                    {
                        if (attrib.LocalName == "type")
                        {
                            typename = attrib.Value;
                        }

                        data[attrib.LocalName] = attrib.Value;
                    }

                _eventHandler.addEvent(factory.makeEvent(typename, data));
            }
        }
    }
}
