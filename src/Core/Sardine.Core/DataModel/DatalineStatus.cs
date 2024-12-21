using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sardine.Core.DataModel
{
    public sealed class DatalineStatus : INotifyPropertyChanged
    {
        private IList<Type> mostRecentDataOut = [];
        private IList<Type> mostRecentDataIn = [];
        private long mostRecentIDOut;
        private long mostRecentIDIn;


        public event PropertyChangedEventHandler? PropertyChanged;


        public IList<Type> MostRecentDataTypesOut
        {
            get => mostRecentDataOut;

            internal set
            {
                if (mostRecentDataOut != value)
                {
                    mostRecentDataOut = value;
                    OnPropertyChanged();
                }
            }
        }

        public IList<Type> MostRecentDataTypesIn
        {
            get => mostRecentDataIn;

            internal set
            {
                if (mostRecentDataIn != value)
                {
                    mostRecentDataIn = value;
                    OnPropertyChanged();
                }
            }
        }

        public long MostRecentIDIn
        {
            get => mostRecentIDIn;

            internal set
            {
                if (mostRecentIDIn != value)
                {
                    mostRecentIDIn = value;
                    OnPropertyChanged();
                }
            }
        }

        public long MostRecentIDOut
        {
            get => mostRecentIDOut;

            internal set
            {
                if (mostRecentIDOut != value)
                {
                    mostRecentIDOut = value;
                    OnPropertyChanged();
                }
            }
        }


        internal DatalineStatus() { }


        private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}