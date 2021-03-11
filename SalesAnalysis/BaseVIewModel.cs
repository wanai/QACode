
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Input;

namespace SalesAnalysis
{
    public class BaseViewModel
    {
        private ICommand _analyzeResultCommand;
        private ICommand _loadSalesCommand;
        private ObservableCollection<City> _selectedCities;
        private DataTable _salesResultTable;
        private Country _selectedCountry;
        private string _selectedMonth;
        private int _selectedYear;
        private List<string> _years;
        private State _selectedState;
        private List<Country> _countriesCollection;
        private ObservableCollection<State> _statesCollection;
        private ObservableCollection<string> _selectedMonthCollection;
        private ObservableCollection<City> _citiesCollection;
        private readonly List<string> _monthsCollection = new List<string>
                                        {"January","February","March","April","May","June","July","August","September","October","November","December"};
        private string _fileName;
        private DelegateCommand <City> _citiesItemChangedCommand;

        public CountriesStates GetCountriesData()
        {
            return DeserializeToObject<CountriesStates>(@"CountriesStatesCities.xml");            
        }

        public List<string> YearsCollection
        {
            get
            {
                if (_years == null)
                {
                    _years = new List<string>
                    {
                        DateTime.Now.Year.ToString()
                    };
                    for (int i = 1; i < 11; i++)
                    {
                        _years.Add(DateTime.Now.AddYears(-1 * i).Year.ToString());
                    }
                }
                return _years;
            }
        }

        public List<Country> CountriesCollection
        {
            get
            {
                return _countriesCollection;
            }
            set
            {
                _countriesCollection = value;
                RaisePropertyChanged("CountriesCollection");
            }
        }

        public ObservableCollection<City> CitiesCollection
        {
            get { return _citiesCollection; }
            set
            {
                _citiesCollection = value;
                RaisePropertyChanged("CitiesCollection");
            }
        }

        public Country SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                _selectedCountry = value;
                if (_selectedCountry != null && _selectedCountry.States != null)
                {
                    StateCollection = new ObservableCollection<State>(_selectedCountry.States);

                }
                RaisePropertyChanged("SelectedCountry");
            }
        }

        public ObservableCollection<string> SelectedMonthCollection
        {
            get { return _selectedMonthCollection; }
            set
            {
                _selectedMonthCollection = value;
                RaisePropertyChanged("SelectedMonthCollection");
            }
        }
        public State SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                CitiesCollection = new ObservableCollection<City>();
                if (_selectedState != null && _selectedState.Cities != null)
                {
                    CitiesCollection = new ObservableCollection<City>(_selectedState.Cities);
                    SelectedCities = new ObservableCollection<City>();
                }
                RaisePropertyChanged("SelectedState");
            }
        }
     
        public string SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                _selectedMonth = value;
                RaisePropertyChanged("SelectedMonth");
            }
        }
        public int SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                if (_selectedYear != value)
                {
                    _selectedYear = value;
                    RaisePropertyChanged("SelectedYear");
                }
            }
        }

        public T DeserializeToObject<T>(string filepath) where T : class
        {
            if (File.Exists(filepath))
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            
                using (StreamReader sr = new StreamReader(filepath))
                {
                    return (T)ser.Deserialize(sr);
                }
            }
            return null;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<State> StateCollection
        {
            get { return _statesCollection; }
            set
            {
                _statesCollection = value;
                RaisePropertyChanged("StateCollection");
            }
        }

        private ObservableCollection<string> _selectedStateCollection;

        public ObservableCollection<string> SelectedStateCollection
        {
            get { return _selectedStateCollection; }
            set
            {
                _selectedStateCollection = value;
                RaisePropertyChanged("SelectedStateCollection");
            }
        }      

        public ObservableCollection<City> SelectedCities
        {
            get { return _selectedCities; }
            set
            {
                _selectedCities=value;
                RaisePropertyChanged("SelectedCities");
            }
        }

        public virtual void OpenSalesData()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "SalesData",
                DefaultExt = ".csv",
                Filter = "SalesData (.csv)|*.csv"
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                FileName = dlg.FileName;
            }
        }

        public ICommand AnalyzeResultCommand
        {
            get { return _analyzeResultCommand; }

            set { _analyzeResultCommand = value; }
        }

        public DelegateCommand <City> CitiesItemChangedCommand
        {
            get { return _citiesItemChangedCommand; }

            set { _citiesItemChangedCommand = value; }
        }
        public ICommand LoadSalesCommand
        {
            get { return _loadSalesCommand; }
            set {  _loadSalesCommand = value; }
        }        

        public List<string> MonthsCollection
        {
            get { return _monthsCollection; }
        }

        public DataTable SalesResultTable
        {
            get { return _salesResultTable; }
            set
            {
                _salesResultTable = value;
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged("FileName");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
