using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesAnalysis;
using System.Data;
using System.Collections.ObjectModel;

namespace SalesAnalysisTest
{
    [TestClass]
    public class CarSalesAnalysisTest
    {
        List<SalesModel> testSales;
        [TestInitialize]
        public void Initialize()
        {
            testSales = new List<SalesModel>
                {
                    new SalesModel{State="Idaho",Country="United States",Year=2021,Month="January",TotalSales=2020,City="ABC" },
                    new SalesModel{State="Alabama",Country="United States",Year=2021,Month="January",TotalSales=2020 ,City="CDE"},
                    new SalesModel{State="Alabama",Country="United States",Year=2021,Month="February",TotalSales=2020,City="EFG" }
                };
        }

        [TestMethod]
        public void Given_State_Is_Selected_Return_SelectedState()
        {
            CarSalesViewModel salViewUnderTest = new CarSalesViewModel
            {
                SelectedState = new State { StateAbbreviation = "IL", StateName = "Illinois", StateId = 1 }
            };
            List<string> expectedResult = new List<string> { "Illinois" };

            salViewUnderTest.GetAllStatesForAnalysis();
            CollectionAssert.AreEqual(expectedResult, salViewUnderTest.SelectedStateCollection);
        }

        [TestMethod]
        public void Given_No_State_Is_Selected_And_MultipleStatesLoaded_Return_All_Distinct_States()
        {
            CarSalesViewModel salViewUnderTest = new CarSalesViewModel
            {
                AllSales = testSales
            };
            List<string> expectedResult = new List<string> { "Idaho", "Alabama" };

            salViewUnderTest.GetAllStatesForAnalysis();

            CollectionAssert.AreEqual(expectedResult, salViewUnderTest.SelectedStateCollection);
        }

        [TestMethod]
        public void Given_Select_A_State_And_Data_Contain_MultipleState_When_Get_State_Return_SelectedState()
        {
            CarSalesViewModel salViewUnderTest = new CarSalesViewModel
            {
                AllSales = testSales,
                SelectedState = new State { StateAbbreviation = "IL", StateName = "Illinois", StateId = 1 }
            };
            List<string> expectedResult = new List<string> { "Illinois" };

            salViewUnderTest.GetAllStatesForAnalysis();

            CollectionAssert.AreEqual(expectedResult, salViewUnderTest.SelectedStateCollection);
        }

        [TestMethod]
        public void Given_No_Country_Is_Given_Return_False()
        {
            CarSalesViewModel underTest = new CarSalesViewModel();
            Assert.IsFalse(underTest.IsAllRequiredInputGiven());
        }

        [TestMethod]
        public void Given_Country_Is_Given_But_No_Year_Return_False()
        {
            CarSalesViewModel underTest = new CarSalesViewModel
            {
                SelectedCountry = new Country { CountryAbbreviation = "abc", CountryName = "Testing", CountryId = 1 }
            };
            Assert.IsFalse(underTest.IsAllRequiredInputGiven());
        }
        [TestMethod]
        public void Given_Country_And_Year_ButFileNotLoaded_When_CheckAllRequiredInput_Return_False()
        {
            CarSalesViewModel underTest = new CarSalesViewModel
            {
                SelectedCountry = new Country { CountryAbbreviation = "abc", CountryName = "Testing", CountryId = 1 },
                SelectedYear = 2021,
            };
            Assert.IsFalse(underTest.IsAllRequiredInputGiven());
        }

        [TestMethod]
        public void Given_Country_And_Year_ButFileLoaded_When_CheckAllRequiredInput_Return_False()
        {
            CarSalesViewModel underTest = new CarSalesViewModel
            {
                SelectedCountry = new Country { CountryAbbreviation = "abc", CountryName = "Testing", CountryId = 1 },
                SelectedYear = 2021,
                FileName="abc.txt"
            };
            Assert.IsTrue(underTest.IsAllRequiredInputGiven());
        }

        [TestMethod]
        public void Given_Year_But_No_Country_When_CheckAllRequiredInput_Return_False()
        {
            CarSalesViewModel underTest = new CarSalesViewModel
            {
                SelectedYear = 2021
            };
            Assert.IsFalse(underTest.IsAllRequiredInputGiven());
        }

        [TestMethod]
        public void Given_SalesContainMultipleStates_And_Year_NotSelected_When_FormulateResult_Then_NoResult()
        {
            CarSalesViewModel SalesFromMultipleStates = new CarSalesViewModel
            {
                SelectedCountry = new Country() { CountryName = "United States", CountryAbbreviation = "USA", CountryId = 1 },
                AllSales = testSales
            };
            SalesFromMultipleStates.GetAllStatesForAnalysis();

            SalesFromMultipleStates.FormulateResultTable();

            Assert.IsNull(SalesFromMultipleStates.SalesResultTable);
        }

        [TestMethod]
        public void Given_SalesContainMultipleStates_And_TwoMatchFound_When_FormulateResult_Then_Return5Rows()
        {
            CarSalesViewModel SalesFromMultipleStates = new CarSalesViewModel
            {
                SelectedCountry = new Country() { CountryName = "United States", CountryAbbreviation = "USA", CountryId = 1 },
                AllSales = testSales,
                SelectedYear = 2021
            };           
            SalesFromMultipleStates.GetAllStatesForAnalysis();

            SalesFromMultipleStates.FormulateResultTable();

            Assert.AreEqual(5, SalesFromMultipleStates.SalesResultTable.Rows.Count);
        }

        [TestMethod]
        public void Given_SalesContainMultipleStates_And_NoMatch_When_FormulateResult_Then_ReturnNoResults()
        {
            CarSalesViewModel SalesFromMultipleStates = new CarSalesViewModel
            {
                SelectedCountry = new Country() { CountryName = "United States", CountryAbbreviation = "USA", CountryId = 1 },
                AllSales = testSales,
                SelectedYear = 2020,
                SelectedState = new State { StateAbbreviation = "AL", StateName = "Alabma", StateId = 1 }
            };    
            SalesFromMultipleStates.GetAllStatesForAnalysis();

            SalesFromMultipleStates.FormulateResultTable();

            Assert.IsNull(SalesFromMultipleStates.SalesResultTable);
        }

        [TestMethod]
        public void Given_EmptyDataTable_When_Calculate_ThenNoCalculation()
        {
            CarSalesViewModel salesUnderTest = new CarSalesViewModel
            {
                SalesResultTable = new DataTable()
            };
            Assert.IsNull(salesUnderTest.GetCalculationRow("Total"));
        }

        [TestMethod]
        public void Given_Year_And_Country_Match_WithSalesData_When_FilterResult__ThenReturnAllMatch()
        {
            CarSalesViewModel salesUnderTest = new CarSalesViewModel()
            {
                AllSales = testSales,
                SelectedYear = 2021,
                SelectedStateCollection = new ObservableCollection<string> { "Idaho" , "Alabama" } ,
                SelectedCountry = new Country { CountryName="United States" }
            };
            salesUnderTest.FilterResult();
            Assert.AreEqual(3, salesUnderTest.SelectedSales.Count);
        }

        [TestMethod]
        public void Given_Month_NoCity_AsInput_When_FilterResult_ThenFilterIncludeMonth()
        {
            CarSalesViewModel salesUnderTest = new CarSalesViewModel()
            {
                AllSales = testSales,
                SelectedYear = 2021,
                SelectedMonth="January",
                SelectedStateCollection = new ObservableCollection<string> { "Idaho", "Alabama" },
                SelectedCountry = new Country { CountryName = "United States" }
            };
            salesUnderTest.FilterResult();
            Assert.AreEqual(2, salesUnderTest.SelectedSales.Count);
        }

        [TestMethod]
        public void Given_City_ButNoMonth_AsInput_When_FilterResult_ThenFilterIncludeCity()
        {
            CarSalesViewModel salesUnderTest = new CarSalesViewModel()
            {
                AllSales = testSales,
                SelectedYear = 2021,
                SelectedCities = new ObservableCollection<City> { new City { Name= "ABC" } },
                SelectedStateCollection = new ObservableCollection<string> { "Idaho", "Alabama" },
                SelectedCountry = new Country { CountryName = "United States" }
            };
            salesUnderTest.FilterResult();
            Assert.AreEqual(1, salesUnderTest.SelectedSales.Count);
        }

        [TestMethod]
        public void Given_Both_Month_And_City_AsInput_When_FilterResult_ThenFilterIncludeCityAndMonth()
        {
            CarSalesViewModel salesUnderTest = new CarSalesViewModel()
            {
                AllSales = testSales,
                SelectedYear = 2021,
                SelectedCities = new ObservableCollection<City> { new City { Name = "ABC" } },//new List<string> {  "ABC" },
                SelectedMonth = "January",
                SelectedStateCollection = new ObservableCollection<string> { "Idaho", "Alabama" },
                SelectedCountry = new Country { CountryName = "United States" }
            };
            salesUnderTest.FilterResult();
            Assert.AreEqual(1, salesUnderTest.SelectedSales.Count);
        }

        [TestMethod]
        public void Given_DataTable_HasMultipleSales_When_Calculate_Total_ReturnTotalForAllColumns()
        {
            CarSalesViewModel salesUnderTest = new CarSalesViewModel();
            DataTable tableUnderTest = new DataTable();
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test", ColumnName = "Month" });
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test", ColumnName = "test" });
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test2", ColumnName = "test2" });
            DataRow row = tableUnderTest.NewRow();
            row[0] = "January";
            row[1] = 200;
            row[2] = 100;
            DataRow row1 = tableUnderTest.NewRow();
            row1[0] = "February";
            row1[1] =300;
            row1[2] = 320;
            tableUnderTest.Rows.Add(row);
            tableUnderTest.Rows.Add(row1);
            salesUnderTest.SalesResultTable = tableUnderTest;
            salesUnderTest.SelectedStateCollection = new ObservableCollection<string> { "test", "test2" };

            DataRow calculatedResult = salesUnderTest.GetCalculationRow("Total");

            Assert.AreEqual("Total:", calculatedResult[0]);
            Assert.AreEqual("500.00",calculatedResult[1]);
            Assert.AreEqual("420.00", calculatedResult[2]);
        }

        [TestMethod]
        public void Given_DataTable_HasMultipleSales_When_Calculate_Avg_ReturnAverageForAllColumns()
        {
            CarSalesViewModel salesUnderTest = new CarSalesViewModel();
            DataTable tableUnderTest = new DataTable();
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test", ColumnName = "Month" });
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test", ColumnName = "test" });
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test2", ColumnName = "test2" });
            DataRow row = tableUnderTest.NewRow();
            row[0] = "January";
            row[1] = 200;
            row[2] = 100;
            DataRow row1 = tableUnderTest.NewRow();
            row1[0] = "February";
            row1[1] = 300;
            row1[2] = 320;
            tableUnderTest.Rows.Add(row);
            tableUnderTest.Rows.Add(row1);
            salesUnderTest.SalesResultTable = tableUnderTest;
            salesUnderTest.SelectedStateCollection = new ObservableCollection<string> { "test", "test2" };

            DataRow calculatedResult = salesUnderTest.GetCalculationRow("Avg.");

            Assert.AreEqual("Avg.:", calculatedResult[0]);
            Assert.AreEqual("250.00", calculatedResult[1]);
            Assert.AreEqual("210.00", calculatedResult[2]);
        }

        [TestMethod]
        public void Given_DataTable_HasTwoSalesPerState_When_Calculate_Median_ReturnMedianForAllColumns()
        {
            CarSalesViewModel salesUnderTest = new CarSalesViewModel();
            DataTable tableUnderTest = new DataTable();
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test", ColumnName = "Month" });
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test", ColumnName = "test" });
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test2", ColumnName = "test2" });
            DataRow row = tableUnderTest.NewRow();
            row[0] = "January";
            row[1] = 200;
            row[2] = 100;
            DataRow row1 = tableUnderTest.NewRow();
            row1[0] = "February";
            row1[1] = 300;
            row1[2] = 320;
            tableUnderTest.Rows.Add(row);
            tableUnderTest.Rows.Add(row1);
         
            salesUnderTest.SalesResultTable = tableUnderTest;
            salesUnderTest.SelectedStateCollection = new ObservableCollection<string> { "test", "test2" };

            DataRow calculatedResult = salesUnderTest.GetCalculationRow("Med.");

            Assert.AreEqual("Med.:", calculatedResult[0]);
            Assert.AreEqual("250.00", calculatedResult[1]);
            Assert.AreEqual("210.00", calculatedResult[2]);
        }

        [TestMethod]
        public void Given_DataTable_HasThreeSalesPerState_When_Calculate_Median_ReturnMedianForAllColumns()
        {
            CarSalesViewModel salesUnderTest = new CarSalesViewModel();
            DataTable tableUnderTest = new DataTable();
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test", ColumnName = "Month" });
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test", ColumnName = "test" });
            tableUnderTest.Columns.Add(new DataColumn() { Caption = "test2", ColumnName = "test2" });
            DataRow row = tableUnderTest.NewRow();
            row[0] = "January";
            row[1] = 356;
            row[2] = 100;
            DataRow row1 = tableUnderTest.NewRow();
            row1[0] = "February";
            row1[1] = 234;
            row1[2] = 123;
            DataRow row2 = tableUnderTest.NewRow();
            row2[0] = "March";
            row2[1] = 125;
            row2[2] = 316;
            tableUnderTest.Rows.Add(row);
            tableUnderTest.Rows.Add(row1);
            tableUnderTest.Rows.Add(row2);

            salesUnderTest.SalesResultTable = tableUnderTest;
            salesUnderTest.SelectedStateCollection = new ObservableCollection<string> { "test", "test2" };

            DataRow calculatedResult = salesUnderTest.GetCalculationRow("Med.");

            Assert.AreEqual("Med.:", calculatedResult[0]);
            Assert.AreEqual("234.00", calculatedResult[1]);
            Assert.AreEqual("123.00", calculatedResult[2]);
        }        
    }
}
