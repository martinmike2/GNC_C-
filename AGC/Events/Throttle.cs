using System.Collections.Generic;

namespace AGC.Events
{
    public class Throttle : Event
    {
        

        public override void execute()
        {
            throw new System.NotImplementedException();
        }

        public Throttle(IReadOnlyDictionary<string, string> data) : base(data)
        {
        }
    }
}