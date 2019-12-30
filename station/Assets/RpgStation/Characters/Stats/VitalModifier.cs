namespace Station
{
    public class VitalModifier
    {
        public string UniqueId;
        public int VitalId;
        public int Amount;
        public float TimeLeft;

        public VitalModifier(VitalModifier modifier)
        {
            UniqueId = modifier.UniqueId;
            VitalId = modifier.VitalId;
            Amount = modifier.Amount;
            TimeLeft = modifier.TimeLeft;
        }
    }

}
