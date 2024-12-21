using Sardine.Core.DataModel.Abstractions;

namespace Sardine.Core.DataModel
{
    internal sealed class SentDataCounter
    {
        private readonly object _lock = new();
        private Dictionary<IDataProvider, long> _data;


        public long this[IDataProvider index]
        {
            get => _data.TryGetValue(index, out long value) ? value : 0;

            set
            {
                lock (_lock)
                {
                    if (!_data.TryAdd(index, value))
                        _data[index] = value;
                }
            }
        }

        public SentDataCounter()
        {
            _data = [];
        }

        public void Reset() => _data = [];

        public void Reset(IDataProvider t)
        {
            if (_data.ContainsKey(t))
                _data[t] = 0;
        }
    }
}