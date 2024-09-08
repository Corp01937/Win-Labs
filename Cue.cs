using System;
using System.ComponentModel;

namespace Win_Labs
{
    public class Cue : INotifyPropertyChanged
    {
        private float cueNumber; // Field for storing CueNumber
        private string duration;
        private string preWait;
        private string postWait;
        private bool autoFollow;
        private string fileName;
        private string targetFile;
        private string notes;

        public float CueNumber
        {
            get => cueNumber;
            set
            {
                if (cueNumber != value)
                {
                    cueNumber = value;
                    OnPropertyChanged(nameof(CueNumber));
                    Console.WriteLine("PropertyChange.CueNumber");
                }
            }
        }

        private string _CueName;
        public string CueName
        {
            get => _CueName;
            set
            {
                if (_CueName != value)
                {
                    _CueName = value;
                    OnPropertyChanged(nameof(CueName));
                }
            }
        }


        public string Duration
        {
            get => duration;
            set
            {
                if (duration != value)
                {
                    duration = value;
                    OnPropertyChanged(nameof(Duration));
                    Console.WriteLine("PropertyChange.Duration");
                }
            }
        }

        public string PreWait
        {
            get => preWait;
            set
            {
                if (preWait != value)
                {
                    preWait = value;
                    OnPropertyChanged(nameof(PreWait));
                    Console.WriteLine("PropertyChange.PreWait");
                }
            }
        }

        public string PostWait
        {
            get => postWait;
            set
            {
                if (postWait != value)
                {
                    postWait = value;
                    OnPropertyChanged(nameof(PostWait));
                    Console.WriteLine("PropertyChange.PostWait");
                }
            }
        }

        public bool AutoFollow
        {
            get => autoFollow;
            set
            {
                if (autoFollow != value)
                {
                    autoFollow = value;
                    OnPropertyChanged(nameof(AutoFollow));
                    Console.WriteLine("PropertyChange.AutoFollow");
                }
            }
        }

        public string FileName
        {
            get => fileName;
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged(nameof(FileName));
                    Console.WriteLine("PropertyChange.FileName");
                }
            }
        }

        public string TargetFile
        {
            get => targetFile;
            set
            {
                if (targetFile != value)
                {
                    targetFile = value;
                    OnPropertyChanged(nameof(TargetFile));
                    Console.WriteLine("PropertyChange.TargetFile");
                }
            }
        }

        public string Notes
        {
            get => notes;
            set
            {
                if (notes != value)
                {
                    notes = value;
                    OnPropertyChanged(nameof(Notes));
                    Console.WriteLine("PropertyChange.Notes");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Console.WriteLine("PropertyChange.Detected");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}