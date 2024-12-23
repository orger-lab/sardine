namespace Sardine.Utils
{
    public class RingBuffer<T>
    {
        private readonly T[] _buffer;
        public int BufferLength { get; }
        protected T this[int index]
        {
            get => _buffer[index];
            set => _buffer[index] = value;
        }

        protected int ReadCaret { get; set; }
        protected int WriteCaret { get; set; }

        private readonly object _lock = new();

        public bool IsSkipping { get; private set; } = false;
        protected bool IsFilled { get; private set; } = false;

        public T Last => this[(WriteCaret == 0 ? BufferLength : WriteCaret) - 1];


        public delegate T2 CalculateMean<T2>(T[] tArray);

        public T2 Mean<T2>(CalculateMean<T2> calculateMean)
        {
            ArgumentNullException.ThrowIfNull(calculateMean);

            List<T> list = [];
            for (int i = 0; i < BufferLength; i++)
            {
                if (_buffer[i] is not null)
                {
                    list.Add(this[i]);
                }
            }
            return calculateMean([.. list]);
        }

        public T NextValue()
        {
            T returnValue;
            lock (_lock)
            {
                if (WriteCaret == ReadCaret && !IsSkipping)
                {
                    throw new InvalidOperationException();
                }

                returnValue = this[ReadCaret++];

                if (ReadCaret == BufferLength)
                {
                    ReadCaret = 0;
                }
            }
            return returnValue;
        }

        public void Add(T item)
        {
            lock (_lock)
            {
                this[WriteCaret++] = item;
                if (WriteCaret == BufferLength)
                {
                    WriteCaret = 0;
                    IsFilled = true;
                }

                if (WriteCaret == ReadCaret)
                {
                    IsSkipping = true;
                }
            }
        }

        public RingBuffer(int bufferLength)
        {
            BufferLength = bufferLength;
            _buffer = new T[BufferLength];
        }
    }
}
