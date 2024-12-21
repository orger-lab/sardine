namespace Sardine.Core.DataModel
{
    public sealed class OnSentDataEventArgs(object data, Type[] dataTypes, int sender, string senderName, int originalSender, double sourceRate, long sourceID) : EventArgs
    {
        public object? Data { get; private set; } = data;
        public MessageMetadata Metadata { get; } = new MessageMetadata(dataTypes, sender, senderName, originalSender, sourceRate, sourceID);
    }
}