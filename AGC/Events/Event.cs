using System.Collections.Generic;
using AGC.Data;

namespace AGC.Events
{
    public abstract class Event
    {
        public double TimeAfterLiftOff { get; }
        protected string EventMessage { get; }
        private string Type { get; }
        private double Throttle { get; }
        private double MassLost { get; }
        private double Angle { get; }

        protected Event(IReadOnlyDictionary<string, string> data)
        {
            if (data.ContainsKey("throttle"))
            {
                double.TryParse(data["throttle"], out var throttle);
                Throttle = throttle;
            }

            if (data.ContainsKey("massLost"))
            {
                double.TryParse(data["massLost"], out var massLost);
                MassLost = massLost;
            }

            if (data.ContainsKey("angle"))
            {
                double.TryParse(data["angle"], out var angle);
                Angle = angle;
            }

            double.TryParse(data["time"], out var time);
            time += Globals.LiftOffTime;
            TimeAfterLiftOff = time;
            
            EventMessage = data["message"];
            Type = data["type"];
        }

        public abstract void execute();
    }
}