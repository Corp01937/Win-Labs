<<<<<<< HEAD
﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
=======
﻿using System;
using System.ComponentModel;
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2

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
<<<<<<< HEAD
                    Save();
=======
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2
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
<<<<<<< HEAD
                    Save();
=======
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2
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
<<<<<<< HEAD
                    Save();
=======
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2
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
<<<<<<< HEAD
                    Save();
=======
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2
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
<<<<<<< HEAD
                    Save();
=======
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2
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
<<<<<<< HEAD
                    Save();
=======
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2
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
<<<<<<< HEAD
                    Save();
=======
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2
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
<<<<<<< HEAD
                    Save();
=======
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Console.WriteLine("PropertyChange.Detected");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
<<<<<<< HEAD

        public void Save() // Change the method to public
        {
            if (string.IsNullOrEmpty(_cueFilePath))
                return;

            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(_cueFilePath, json);
                Console.WriteLine($"Cue saved to {_cueFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving cue: {ex.Message}");
            }
        }
        private string _cueFilePath;

        public void SetFilePath(string filePath)
        {
            _cueFilePath = filePath;
        }
=======
>>>>>>> 593531934aef45b6890753e80268784cbfef1aa2
    }
}