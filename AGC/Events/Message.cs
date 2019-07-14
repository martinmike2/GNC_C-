using System;
using System.Collections.Generic;

namespace AGC.Events
{
    public class Message : Event
    {
        public Message(IReadOnlyDictionary<string, string> data) : base(data)
        {
        }

        public override void execute()
        {
            Console.WriteLine(EventMessage);
        }
    }
}