using System;
using System.Collections.Generic;

namespace AGC.Events
{
    public class Stage : Event
    {
        public Stage(IReadOnlyDictionary<string, string> data) : base(data)
        {
        }

        public override void execute()
        {
            Console.WriteLine("!!!!STAGE!!!!");
        }
    }
}