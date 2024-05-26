using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Winery
{
    public class Container : INotifyPropertyChanged
    {
        #region Fields
        private string tankID;
        private string wineID;
        private int maxVolume;
        private ContainerType type;
        private ContainerStatus status;
        private ContainerLocation location;
        private int currentVolume;
        private DateTime lastEditDate;
        #endregion

        #region Enums
        public enum ContainerType
        {
            Tank,
            Barrel,
            Bottle,
            Keg,
            Vat
        }

        public enum ContainerStatus
        {
            Closed,
            Opened,
            InUse,
            Maintenance,
            Empty
        }

        public enum ContainerLocation
        {
            Basement,
            Hangar,
            Outside,
            Storage
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        [Key]
        [Required]
        public string TankID
        {
            get { return tankID; }
            set
            {
                if (tankID != value)
                {
                    tankID = value;
                    OnPropertyChanged("TankID");
                }
            }
        }

        [ForeignKey("Wine")]
        public string WineID
        {
            get { return wineID; }
            set
            {
                wineID = value;
                OnPropertyChanged("WineID");
            }
        }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MaxVolume must be a positive number.")]
        public int MaxVolume
        {
            get { return maxVolume; }
            set
            {
                if (maxVolume != value)
                {
                    maxVolume = value;
                    OnPropertyChanged("MaxVolume");
                }
            }
        }

        [Required]
        public ContainerType Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        [Required]
        public ContainerStatus Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        [Required]
        public ContainerLocation Location
        {
            get { return location; }
            set
            {
                if (location != value)
                {
                    location = value;
                    OnPropertyChanged("Location");
                }
            }
        }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "CurrentVolume must be a non-negative number.")]
        public int CurrentVolume
        {
            get { return currentVolume; }
            set
            {
                if (currentVolume != value)
                {
                    currentVolume = value;
                    OnPropertyChanged("CurrentVolume");
                }
            }
        }

        [Required]
        public DateTime LastEditDate
        {
            get { return lastEditDate; }
            set
            {
                if (lastEditDate != value)
                {
                    lastEditDate = value;
                    OnPropertyChanged("LastEditDate");
                }
            }
        }
        #endregion

        #region Methods
        public Container() { }
        public virtual Wine Wine { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Tank ID: {TankID}");
            sb.AppendLine($"Wine ID: {WineID}");
            sb.AppendLine($"Max Volume: {MaxVolume}");
            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Status: {Status}");
            sb.AppendLine($"Location: {Location}");
            sb.AppendLine($"Current Volume: {CurrentVolume}");
            sb.AppendLine($"Last Edit Date: {LastEditDate}");
            return sb.ToString();
        }
        #endregion
    }
}
