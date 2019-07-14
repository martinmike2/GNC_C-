namespace AGC.Data
{
    public class Staging
    {
        private bool Jettison { get; set; }
        private double WaitBeforeJettison { get; set; }
        private bool Ignition { get; set; }
        private double WaitBeforeIgnition { get; set; }
        private string Ullage { get; set; }
        private double UllageBurnDuration { get; set; }
        private double PostUllageBurn { get; set; }
        
        public Staging(bool jettison, double waitBeforeJettison, bool ignition, double waitBeforeIgnition, string ullage, double ullageBurnDuration = 0, double postUllageBurn = 0)
        {
            Jettison = jettison;
            WaitBeforeJettison = waitBeforeJettison;
            Ignition = ignition;
            WaitBeforeIgnition = waitBeforeIgnition;
            Ullage = ullage;
            UllageBurnDuration = ullageBurnDuration;
            PostUllageBurn = postUllageBurn;
        }
    }
}
