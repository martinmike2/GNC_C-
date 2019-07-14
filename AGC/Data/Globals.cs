using KRPC.Client;
using AGC.Utilities;

namespace AGC.Data
{
    public class Globals
    {
        public static Mission MissionData { get; set; }
        public static Controls ControlData { get; set; }
        public static Target TargetData { get; set; }
        public static Vehicle VehicleData { get; set; }
        public static VehicleState VehicleSate { get; set; }
        public static UpfgState CurrentUpfgState { get; set; }
        public static UpfgState PreviousUpfgState { get; set; }
        public static CserState CserState { get; set; }
        public static Guidance VehicleGuidance { get; set; }
        public static double G0 { get; set; }
        public static double MU { get; set; }
        public static double UpfgStage { get; set; }
       
        public static double ThrottleSetting { get; set; }
        public static double ThrottleDisplay { get; set; }
        public static AgcTuple SteeringVector { get; set; }
        public static double SteeringRoll { get; set; }
        public static bool UpfgConverged { get; set; }
        public static bool StagingInProgress { get; set; }
        
        public static double CurrentTime { get; set; }

        public static double TimeToOrbitIntercept { get; set; }

        public static AgcTuple CurrentNode { get; set; }
        
        public static Connection KrpConnection { get; set; }
        
        public static AgcTuple Solarprimevector { get; set; }
        
        public static double LiftOffTime { get; set; }
        
        public static Target UpfgTarget { get; set; }

        public static bool LiftOffFlag { get; set; }
        
        public static double PitchOverLimit = 20;
        public static double UpfgConvergenceDelay = 5;
        public static double UpfgFinalizationTime = 5;
        public static double StagingKillRotationTime = 5;
        public static double UpfgConvergenceCriterion = 0.1;
        public static double UpfgGoodSolutionCriterion = 15;
        
    }
}