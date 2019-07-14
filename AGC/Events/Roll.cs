using System.Collections.Generic;

namespace AGC.Events
{
    public class Roll : Event
    {
        

        public override void execute()
        {
            throw new System.NotImplementedException();
        }

        public Roll(IReadOnlyDictionary<string, string> data) : base(data)
        {
        }
    }
}