
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;

namespace AGC
{

    internal static class Program
    {
        

        static void Main(string[] args)
        {
            var connection = new Connection(
                name: "My Example",
                address: IPAddress.Parse("127.0.0.1"),
                rpcPort: 1000,
                streamPort: 1001
            );
            var spaceCenter = connection.SpaceCenter();
            var vessel = spaceCenter.ActiveVessel;
            var refFram = vessel.Orbit.Body.ReferenceFrame;
            var posStream = connection.AddStream(() => vessel.Position(refFram));
            //posStream.Start();
            Settings settings = new Settings();
            loadData(settings);
            CSER cser = new CSER();

                System.Timers.Timer loopTimer = new System.Timers.Timer();
            loopTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e, posStream);
            loopTimer.Interval = 100;
            loopTimer.Enabled = true;

            //Console.WriteLine("Press \'q\' to quit.");
            while (Console.Read() != 'q') ;
        }

        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e, Stream<Tuple<double, double, double>> posStream)
        {
            //Console.WriteLine(posStream.Get());
        }

        private static bool loadData(Settings settings)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load("test_vehicle.xml");

            XmlNode controls = doc.DocumentElement.SelectSingleNode("/data/controls");
            XmlNode vehicle = doc.DocumentElement.SelectSingleNode("/data/vehicle");
            XmlNode sequencing = doc.DocumentElement.SelectSingleNode("/data/sequences");
            XmlNode mission = doc.DocumentElement.SelectSingleNode("/data/mission");

            return true;
        }

        private Vehicle buildVehicle(XmlNode vehicleNode)
        {
            Vehicle vehicle = new Vehicle();
            XmlNodeList stages = vehicleNode.SelectNodes("/stage/");

            foreach (XmlNode stage in stages)
            {

            }
        }

    }
}
