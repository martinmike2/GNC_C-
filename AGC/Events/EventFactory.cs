using System.Collections.Generic;

namespace AGC.Events
{
    public class EventFactory
    {
        public Event makeEvent(string type, Dictionary<string, string> data)
        {
            Event e;
            switch (type)
            {
                case "print":
                    e = new Message(data);
                    break;
                case "stage":
                    e = new Stage(data);
                    break;
                case "launch":
                    e = new Launch(data);
                    break;
                default:
                    e = new Message(data);
                    break;
            }

            return e;
        }
    }
}