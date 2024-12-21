namespace Sardine.Core.DataModel
{
    public sealed class MessageMetadata
    {
        public IReadOnlyList<Type> DataTypes { get; }
        public int Sender { get; }
        public string SenderName { get; }
        public int OriginalSender { get; }
        public double SourceRate { get; }
        public long SourceID { get; }


        internal MessageMetadata(Type[] dataTypes, int sender, string senderName, int originalSender, double sourceRate, long sourceID)
        {
            DataTypes = dataTypes;
            SourceRate = sourceRate;
            OriginalSender = originalSender;
            Sender = sender;
            SenderName = senderName;
            SourceID = sourceID;
        }

    }
}