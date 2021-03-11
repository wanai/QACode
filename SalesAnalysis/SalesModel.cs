namespace SalesAnalysis
{  

    public class SalesModel
    {
        private string _Country;
        private string _City;
        private string _Month;
        private int _Year;       
        private double _totalSales;     
        private string _State;      
        public string Country
        {
            get
            {
                return _Country;
            }
            set { _Country = value; }
        }
        public string City
        {
            get
            {
                return _City;
            }
            set { _City = value; }
        }

        public string Month
        {
            get
            {
                return _Month;
            }
            set { _Month = value; }
        }
        public int Year
        {
            get
            {
                return _Year;
            }
            set { _Year = value; }
        }
        public string State
        {
            get
            {
                return _State;
            }
            set { _State = value; }
        }
        
       
        public double TotalSales
        {
            get
            {
                return _totalSales;
            }
            set { _totalSales = value; }
        }
    }
}
