using System.Collections.Generic;
using AGC.Data;

namespace AGC.Events
{
    public class Handler
    {
        private List<Event> Events { get; }

        public Handler()
        {
            Events = new List<Event>();
        }

        public void addEvent(Event e)
        {
            Events.Add(e);
        }

        private void removeEvent(Event e)
        {
            Events.Remove(e);
        }

        private Event getNextEvent()
        {
            return Events.Find(x => x.TimeAfterLiftOff <= Globals.CurrentTime);
        }

        public void execute()
        {
            var toRun = getNextEvent();

            while (toRun != null)
            {
                toRun.execute();
                removeEvent(toRun);
                toRun = getNextEvent();
            }
            
        }
    }
}