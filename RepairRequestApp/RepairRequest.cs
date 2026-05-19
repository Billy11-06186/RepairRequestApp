using System;
using System.ComponentModel;

namespace RepairRequestApp
{
    public class RepairRequest : INotifyPropertyChanged
    {
        private int id;
        private string equipment;
        private string faultType;
        private string status;
        private string client;
        private string description;
        private DateTime createdDate;

        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Equipment
        {
            get => equipment;
            set { equipment = value; OnPropertyChanged(nameof(Equipment)); }
        }

        public string FaultType
        {
            get => faultType;
            set { faultType = value; OnPropertyChanged(nameof(FaultType)); }
        }

        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        public string Client
        {
            get => client;
            set { client = value; OnPropertyChanged(nameof(Client)); }
        }

        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(nameof(Description)); }
        }

        public DateTime CreatedDate
        {
            get => createdDate;
            set { createdDate = value; OnPropertyChanged(nameof(CreatedDate)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}