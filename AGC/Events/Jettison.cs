using System.Collections.Generic;

namespace AGC.Events
{
    public class Jettison : Event
    {
        public Jettison(IReadOnlyDictionary<string, string> data) : base(data)
        {
        }

        public override void execute()
        {
            throw new System.NotImplementedException();
        }
    }
}