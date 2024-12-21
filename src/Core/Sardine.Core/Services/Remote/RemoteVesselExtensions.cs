namespace Sardine.Core.Remote
{
    public static class RemoteVesselExtensions
    {
        public static void RegisterRemoteControl<THandle>(this Vessel<THandle> vessel, string registrationName, string callName, RemoteControlAction<THandle> action)
        {
            ArgumentNullException.ThrowIfNull(vessel);
            Fleet.Current.Get<RemoteController>().Register(vessel, registrationName, [(callName, action)]);
        }

        public static void RegisterRemoteControl<THandle>(this Vessel<THandle> vessel, string registrationName, IEnumerable<(string, RemoteControlAction<THandle>)> actions)
        {
            ArgumentNullException.ThrowIfNull(vessel);
            Fleet.Current.Get<RemoteController>().Register(vessel, registrationName, actions);
        }
    }
}
