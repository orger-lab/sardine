namespace Sardine.Core
{
    public sealed class VesselReloadedEventArgs(bool isOnline) : EventArgs
    {
        public bool IsOnline { get; } = isOnline;
    }
}