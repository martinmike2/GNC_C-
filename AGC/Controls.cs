namespace AGC
{
    public class Controls
    {
        public double LaunchTimeAdvance { get; set; }
        public double VerticalAscentTime { get; set; }
        public double PitchOverAngle { get; set; }
        public double UPFGActivation { get; set; }
        public double LaunchAzimuth { get; set; }
        public double InitialRoll { get; set; }

        public Controls(double launchTimeAdvance, double verticalAscentTime, double pitchOverAngle, double upfgActivation, double launchAzimuth = 0, double initialRoll = 0)
        {
            LaunchTimeAdvance = launchTimeAdvance;
            VerticalAscentTime = verticalAscentTime;
            PitchOverAngle = pitchOverAngle;
            UPFGActivation = upfgActivation;
            LaunchAzimuth = launchAzimuth;
            InitialRoll = initialRoll;
        }
    }
}
