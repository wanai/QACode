using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Data;
using System.Windows;

namespace SalesAnalysis
{
    public class CarSalesViewModel : BaseViewModel, INotifyPropertyChanged
    {
        SalesModel _sales;
        private List<SalesModel> _allSales;
        private bool _isWarningVisible;
        private List<SalesModel> _selectedSales;
              
        private void AnalyzeResult()
        {
            LoadSalesResult();
        }       

        private void LoadSalesResult()
        {
            if (!IsAllRequiredInputGiven())
            {
                IsWarningVisible = true;
                return;
            }
            LoadAllSales();
            GetAllStatesForAnalysis();

            IsWarningVisible = false;
            FormulateResultTable();
            if (SalesResultTable != null)
                SalesAnalyzerWindow.Instance.dgSales.ItemsSource = SalesResultTable.DefaultView;
        }

       
        private void AddCalculation()
        {
            DataRow average = GetCalculationRow("Avg.");
            DataRow sum = GetCalculationRow("Total");
            DataRow med = GetCalculationRow("Med.");
            if (average != null)
                SalesResultTable.Rows.Add(average);
            if (med != null)
                SalesResultTable.Rows.Add(med);
            if (sum != null)
                SalesResultTable.Rows.Add(sum);
        }    

        private void ConvertResultBasedOnState()
        {
            Dictionary<int, double> totalByState = new Dictionary<int, double>();
            foreach (string m in SelectedMonthCollection)
            {
                DataRow r = SalesResultTable.NewRow();
                r[0] = m;
                for (int i = 0; i < SelectedStateCollection.Count; i++)
                {
                    double totalSales = SelectedSales.Where(x => x.Month.Equals(m) && x.State.Equals(SelectedStateCollection[i]) && x.Year.Equals(SelectedYear))
                           .Select(selectItem => selectItem.TotalSales).FirstOrDefault();
                    r[i + 1] = totalSales;
                    if (totalByState.ContainsKey(i))
                    {
                        totalByState[i] += totalSales;
                    }
                    else
                    {
                        totalByState.Add(i, totalSales);
                    }
                }
                SalesResultTable.Rows.Add(r);
            }
        }

        private string GetTotalByColumnName(DataTable table, string columnName)
        {
            return String.Format("{0:0.00}", table.AsEnumerable()
                        .Select(r => r.Field<string>(columnName))
                        .Sum(x => Convert.ToInt32(x)));
        }

        private string GetAverageByColumnName(DataTable table, string columnName)
        {
            return String.Format("{0:0.00}", table.AsEnumerable()
                          .Select(r => r.Field<string>(columnName))
                          .Average(x => Convert.ToDouble(x)));
        }

        private string GetMedianByColumName(DataTable table, string columnName)
        {
            var orderedData = table.AsEnumerable()
                        .Select(r => r.Field<string>(columnName))
                        .OrderBy(x => Convert.ToDouble(x));

            int totalRecord = table.Rows.Count;

            int medianPos = totalRecord / 2;
            if (totalRecord > 0 && totalRecord % 2 == 0)
            {
                return string.Format("{0:0.00}", ((Double.Parse(orderedData.ElementAt(medianPos - 1))
                                         + Double.Parse((orderedData.ElementAt(medianPos)))) / 2));
            }
            return string.Format("{0:0.00}", ((Double.Parse(orderedData.ElementAt(medianPos)))));
        }

        private void FormulateColumns()
        {
            SalesResultTable = new DataTable();
            SalesResultTable.Columns.Add("Month", typeof(string));
            foreach (string s in SelectedStateCollection)
            {
                SalesResultTable.Columns.Add(s, typeof(string));
            }
        }

        private string[] GetDataFromFile(string fileName)
        {
            return File.ReadAllLines(fileName);
        }
        public bool CanUpdateCities
        {
            get { return SelectedState != null || SelectedStateCollection != null; }
        }

        public CarSalesViewModel()
        {
            CountriesCollection = new List<Country>();
            StateCollection = new ObservableCollection<State>();
            IsWarningVisible = false;
            AnalyzeResultCommand = new DelegateCommand(AnalyzeResult, ()=>true);
            LoadSalesCommand = new DelegateCommand(OpenSalesData, () => true);

            CountriesCollection = GetCountriesData().Countries;
        }

        public System.Collections.IList SelectedItems
        {
            get
            {
                return SelectedCities;
            }
            set
            {
                SelectedCities.Clear();
                foreach (City city in value)
                {
                    SelectedCities.Add(city);
                }
            }
        }

        public void FilterResult()
        {
            SelectedMonthCollection = new ObservableCollection<string>();

            SelectedSales = AllSales.Where(x => SelectedStateCollection.Contains(x.State) && x.Country.Contains(SelectedCountry.CountryName)
                                            && x.Year.Equals(SelectedYear)).ToList();
            if (SelectedMonth != null)
            {
                SelectedSales = SelectedSales.Where(x=>x.Month.Equals(SelectedMonth)).ToList();                
            }
            if (SelectedCities != null)
            {
                SelectedSales = SelectedSales.Where(sale => SelectedCities.Select(r=>r.Name).Contains(sale.City)).ToList();
            }
        }

        public void SelectMonthsForResult()
        {
            if (SelectedMonth != null)
                SelectedMonthCollection.Add(SelectedMonth);
            else
            {
                foreach (var i in SelectedSales.Select(x => x.Month).Distinct().ToList())
                {
                    SelectedMonthCollection.Add(i);
                }
            }
        }

        public void FormulateResultTable()
        {
            FilterResult();
            SelectMonthsForResult();
            if (SelectedSales.Count > 0)
            {
                FormulateColumns();
                ConvertResultBasedOnState();
                AddCalculation();
            }
        }

        public void GetAllStatesForAnalysis()
        {
            SelectedStateCollection = new ObservableCollection<string>();
            if (SelectedState != null)
            {
                SelectedStateCollection.Add(SelectedState.StateName);
                return;
            }
            List<string> selectedStates = AllSales.Select(e => e.State).Distinct().ToList();
            foreach (string state in selectedStates)
            {
                SelectedStateCollection.Add(state);
            }
            SelectedStateCollection.OrderBy(s => s.ToString());
        }

        public List<SalesModel> AllSales
        {
            get
            {
                return _allSales;
            }
            set
            {
                _allSales = value;
            }
        }                

        public void  LoadAllSales()
        {
            if (AllSales == null)
            {
                string[] content = GetDataFromFile(FileName);
                AllSales = new List<SalesModel>();
                foreach (var line in content)
                {
                    string[] data = line.Split(';');
                    Sales = new SalesModel
                    {
                        Month = data[0],
                        Year = Convert.ToInt32(data[1]),
                        Country = data[2],
                        State = data[3],
                        City = data[4],
                        TotalSales = Convert.ToInt32(data[5])
                    };
                    AllSales.Add(Sales);
                }
            }
        }       

        public SalesModel Sales
        {
            get { return _sales; }
            set { _sales = value; }
        }     

        public List<SalesModel> SelectedSales
        {
            get { return _selectedSales; }
            set
            {
                _selectedSales = value;
                RaisePropertyChanged("SelectedState");
            }
        }     

        public bool IsAllRequiredInputGiven()
        {
            return SelectedCountry != null && SelectedYear > 1000 && !string.IsNullOrEmpty(FileName);
        }

        public bool IsWarningVisible
        {
            get { return _isWarningVisible; }
            set
            {
                _isWarningVisible = value;
                RaisePropertyChanged("IsWarningVisible");
                RaisePropertyChanged("IsResultVisible");
            }
        }

        public bool IsResultVisible
        {
            get { return !_isWarningVisible; }

        }

        public DataRow GetCalculationRow(string label)
        {
            if (SalesResultTable.Rows.Count <= 0)
            {
                return null;
            }
            DataRow calculationRow = SalesResultTable.NewRow();
            calculationRow[0] = label + ":";
            for (int i = 0; i < SelectedStateCollection.Count; i++)
            {
                if (label == "Total")
                {
                    calculationRow[i + 1] = GetTotalByColumnName(SalesResultTable, SalesResultTable.Columns[i + 1].ColumnName);
                }
                else if (label == "Avg.")
                {
                    calculationRow[i + 1] = GetAverageByColumnName(SalesResultTable, SalesResultTable.Columns[i + 1].ColumnName);
                }
                else if (label == "Med.")
                {
                    calculationRow[i + 1] = GetMedianByColumName(SalesResultTable, SalesResultTable.Columns[i + 1].ColumnName);
                }

            }
            return calculationRow;
        }       

    }


}
