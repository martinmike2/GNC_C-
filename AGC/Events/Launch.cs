using System.Collections.Generic;
using AGC.Data;
using KRPC.Client.Services.SpaceCenter;

namespace AGC.Events
{
    public class Launch : Event
    {
        public Launch(IReadOnlyDictionary<string, string> data) : base(data)
        {
        }

        public override void execute()
        {
            Globals.KrpConnection.SpaceCenter().ActiveVessel.Control.ActivateNextStage();
            Globals.LiftOffFlag = true;
        }
    }
}