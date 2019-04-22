namespace Wokarol.Clocks
{
    public interface IClock
    {
        /// <summary>
        /// Called every time clock tick, passes delta
        /// </summary>
        event System.Action<float> OnTick;
    } 
}
