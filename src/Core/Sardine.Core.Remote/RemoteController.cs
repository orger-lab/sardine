[assembly: CLSCompliant(true)]
namespace Sardine.Core.Remote
{
    public delegate void RemoteControlAction<T>(T handle, params object[] options);

    public sealed class RemoteController
    {
        readonly Dictionary<string, Dictionary<string, (Vessel Vessel, Delegate Action)>> registeredHandles = [];


        public void Register<T>(Vessel<T> vessel, string registrationName, IEnumerable<(string CallName, RemoteControlAction<T> Action)> actionNamePairs)
        {
            // TODO fix, not going into registeredHandles
            if (actionNamePairs is null)
                return;

            if (!registeredHandles.TryGetValue(registrationName, out Dictionary<string, (Vessel Vessel, Delegate Action)>? value))
                value = [];

            foreach((string CallName, RemoteControlAction<T> Action) actionNamePair in actionNamePairs)
                value[actionNamePair.CallName] = (vessel, actionNamePair.Action);
        }

        public void Unregister(string registrationName, string? callName = null)
        {
            if (callName is null)
            {
                registeredHandles.Remove(registrationName);
                return;
            }

            if (registeredHandles.TryGetValue(registrationName, out Dictionary<string, (Vessel Vessel, Delegate Action)>? value))
                value?.Remove(callName);
        }

        public bool ExecuteCall(string registrationName, string callName, params object[] options)
        {
            if (!registeredHandles.TryGetValue(registrationName, out Dictionary<string, (Vessel Vessel, Delegate Action)>? rco))
                return false;

            if (rco is null)
                return false;

            if (!rco.TryGetValue(callName, out (Vessel Vessel, Delegate Action) vesselDelegatePair))
                return false;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                vesselDelegatePair.Action.DynamicInvoke(vesselDelegatePair.Vessel.ObjectHandle, options);
            }
            catch
            {
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return true;
        }
    }
}
