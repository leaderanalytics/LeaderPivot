using LeaderAnalytics.LeaderPivot;
using System.Collections.Generic;
using System.Linq;

namespace LeaderAnalytics.LeaderPivot.TestData
{

    public class SalesData
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }

        public string Year { get; set; }
        public string Quarter { get; set; }
        public string Month { get; set; }
    }

    public class SalesDataService
    {
        private string[] Months = new string[12] {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec",};

        public List<Dimension<SalesData>> LoadDimensions()
        {
            // Must have at least one row and at least one column dimension

            List<Dimension<SalesData>> dimensions = new List<Dimension<SalesData>>
            {
                // Rows
                new Dimension<SalesData>
                {
                    DisplayValue = "Product Name",
                    GroupValue = x => x.Product,
                    HeaderValue = x => x.Product,
                    IsRow = true,
                    IsExpanded = true,
                    Sequence = 3,
                    IsAscending = true,
                    IsEnabled = true
                },

                new Dimension<SalesData>
                {
                    DisplayValue = "Country",
                    GroupValue = x => x.Country,
                    HeaderValue = x => x.Country,
                    IsRow = true,
                    IsExpanded = true,
                    Sequence = 0,
                    IsAscending = true,
                    IsEnabled = true
                },


                new Dimension<SalesData>
                {
                    DisplayValue = "State",
                    GroupValue = x => x.State,
                    HeaderValue = x => x.State,
                    IsRow = true,
                    IsExpanded = true,
                    Sequence = 1,
                    IsAscending = true,
                    IsEnabled = true
                },


                new Dimension<SalesData>
                {
                    DisplayValue = "City",
                    GroupValue = x => x.City,
                    HeaderValue = x => x.City,
                    IsRow = true,
                    IsExpanded = true,
                    Sequence = 2,
                    IsAscending = true,
                    IsEnabled = true
                },

                // Columns
                new Dimension<SalesData>
                {
                    DisplayValue = "Year",
                    GroupValue = x => x.Year,
                    HeaderValue = x => $"Year: {x.Year}",
                    IsRow = false,
                    IsExpanded = true,
                    Sequence = 0,
                    IsAscending = true,
                    IsEnabled = true
                },

                new Dimension<SalesData>
                {
                    DisplayValue = "Quarter",
                    GroupValue = x => x.Quarter,
                    HeaderValue = x => $"Quarter: {x.Quarter}",
                    IsRow = false,
                    IsExpanded = true,
                    Sequence = 1,
                    IsAscending = true,
                    IsEnabled = true
                },

                new Dimension<SalesData>
                {
                    DisplayValue = "Month",
                    GroupValue = x => x.Month,
                    HeaderValue = x => x.Month,
                    SortValue = x =>  System.Array.IndexOf(Months, x).ToString().PadLeft(2,'0'),
                    IsRow = false,
                    IsExpanded = true,
                    Sequence = 2,
                    IsAscending = true,
                    IsEnabled = true
                }
            };

            return dimensions;
        }

        public List<Measure<SalesData>> LoadMeasures()
        {
            List<Measure<SalesData>> measures = new List<Measure<SalesData>>
            {
                new Measure<SalesData> { Aggragate = x => x.Measure.Sum(y => y.Quantity), DisplayValue = "Quantity", Format="{0:n0}", Sequence = 1, IsEnabled = true },
                
                new Measure<SalesData> { Aggragate = x => 
                {
                    return ((x.ColumnGroup?.Sum(y => y.Quantity) ?? 0) == 0) ? 1m :  x.Measure.Sum(y => y.Quantity) / (decimal)x.ColumnGroup.Sum(z => z.Quantity);
                    
                }, DisplayValue = "Quantity % of Column", Format="{0:P}", Sequence = 4, IsEnabled = true
                
                },


                new Measure<SalesData> 
                { 
                    Aggragate = x => 
                    {
                        decimal result = (x.RowGroup?.Sum(y => y.Quantity * y.UnitPrice) ?? 0) == 0 ? 1m :  x.Measure.Sum(y => y.Quantity * y.UnitPrice) / ((decimal)x.RowGroup.Sum(y => y.Quantity * y.UnitPrice));
                        return result;
                    
                    } , DisplayValue = "Revenue % of Row" , Format="{0:P}", Sequence = 5, IsEnabled = true
                },
               
                new Measure<SalesData> { Aggragate = x => x.Measure.Sum(y => y.Quantity * y.UnitPrice), DisplayValue = "Revenue", Format="{0:C}", Sequence = 2, IsEnabled = true },
                new Measure<SalesData> { Aggragate = x => x.Measure.Count(), DisplayValue = "Count", Format="{0:n0}", Sequence = 3, IsEnabled = true}
            };
            return measures;
        }

        public List<SalesData> GetSalesData()
        {
            return new List<SalesData>
            {

                // US

                // California
                
                // San Diego
                // Coffee Mug
                new SalesData {Product = "Coffee Mug", Quantity = 2, UnitPrice = 13, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "Coffee Mug", Quantity = 6, UnitPrice = 15, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "Coffee Mug", Quantity = 8, UnitPrice = 15, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "Coffee Mug", Quantity = 19, UnitPrice = 13, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "Coffee Mug", Quantity = 44, UnitPrice = 13, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "Coffee Mug", Quantity = 9, UnitPrice = 13, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "Coffee Mug", Quantity = 2, UnitPrice = 13, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "Coffee Mug", Quantity = 66, UnitPrice = 15, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "4", Month = "Dec" },

                // KVM Switch
                new SalesData {Product = "KVM Switch", Quantity = 38, UnitPrice = 13, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "KVM Switch", Quantity = 18, UnitPrice = 15, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "KVM Switch", Quantity = 25, UnitPrice = 13, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "KVM Switch", Quantity = 12, UnitPrice = 15, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "KVM Switch", Quantity = 78, UnitPrice = 15, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "KVM Switch", Quantity = 13, UnitPrice = 15, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "KVM Switch", Quantity = 6, UnitPrice = 13, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "KVM Switch", Quantity = 81, UnitPrice = 15, Country = "US", State = "CA", City = "San Diego", Year = "2020", Quarter = "4", Month = "Dec" },


                // Los Angeles
                // Coffee Mug
                new SalesData {Product = "Coffee Mug", Quantity = 42, UnitPrice = 15, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "Coffee Mug", Quantity = 39, UnitPrice = 13, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "1", Month = "Feb" },

                new SalesData {Product = "Coffee Mug", Quantity = 28, UnitPrice = 13, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "Coffee Mug", Quantity = 41, UnitPrice = 15, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "Coffee Mug", Quantity = 39, UnitPrice = 13, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "Coffee Mug", Quantity = 63, UnitPrice = 15, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "Coffee Mug", Quantity = 81, UnitPrice = 15, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "Coffee Mug", Quantity = 22, UnitPrice = 15, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "4", Month = "Dec" },

                // KVM Switch
                new SalesData {Product = "KVM Switch", Quantity = 9, UnitPrice = 13, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "KVM Switch", Quantity = 37, UnitPrice = 15, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "KVM Switch", Quantity = 29, UnitPrice = 13, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "KVM Switch", Quantity = 17, UnitPrice = 15, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "2", Month = "May" },

                new SalesData {Product = "KVM Switch", Quantity = 27, UnitPrice = 13, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "KVM Switch", Quantity = 6, UnitPrice = 15, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "KVM Switch", Quantity = 63, UnitPrice = 15, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "KVM Switch", Quantity = 57, UnitPrice = 13, Country = "US", State = "CA", City = "Los Angeles", Year = "2020", Quarter = "4", Month = "Dec" },



                // New York
                // Brooklyn
                // Coffee Mug
                new SalesData {Product = "Coffee Mug", Quantity = 86, UnitPrice = 13, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "Coffee Mug", Quantity = 18, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "Coffee Mug", Quantity = 63, UnitPrice = 13, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "Coffee Mug", Quantity = 98, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "Coffee Mug", Quantity = 28, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "Coffee Mug", Quantity = 64, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "Coffee Mug", Quantity = 78, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "Coffee Mug", Quantity = 23, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "4", Month = "Dec" },

                // KVM Switch
                new SalesData {Product = "KVM Switch", Quantity = 85, UnitPrice = 13, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "KVM Switch", Quantity = 16, UnitPrice = 13, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "KVM Switch", Quantity = 54, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "KVM Switch", Quantity = 77, UnitPrice = 13, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "KVM Switch", Quantity = 60, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "KVM Switch", Quantity = 99, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "KVM Switch", Quantity = 30, UnitPrice = 13, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "KVM Switch", Quantity = 36, UnitPrice = 15, Country = "US", State = "NY", City = "Brooklyn", Year = "2020", Quarter = "4", Month = "Dec" },


                // Manhattan
                // Coffee Mug
                new SalesData {Product = "Coffee Mug", Quantity = 74, UnitPrice = 15, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "Coffee Mug", Quantity = 86, UnitPrice = 13, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "1", Month = "Feb" },

                new SalesData {Product = "Coffee Mug", Quantity = 49, UnitPrice = 13, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "Coffee Mug", Quantity = 88, UnitPrice = 15, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "Coffee Mug", Quantity = 78, UnitPrice = 15, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "Coffee Mug", Quantity = 44, UnitPrice = 13, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "Coffee Mug", Quantity = 56, UnitPrice = 15, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "Coffee Mug", Quantity = 60, UnitPrice = 15, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "4", Month = "Dec" },

                // KVM Switch
                new SalesData {Product = "KVM Switch", Quantity = 29, UnitPrice = 15, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "KVM Switch", Quantity = 69, UnitPrice = 13, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "KVM Switch", Quantity = 17, UnitPrice = 13, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "KVM Switch", Quantity = 68, UnitPrice = 15, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "2", Month = "May" },

                new SalesData {Product = "KVM Switch", Quantity = 18, UnitPrice = 15, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "KVM Switch", Quantity = 80, UnitPrice = 13, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "KVM Switch", Quantity = 78, UnitPrice = 13, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "KVM Switch", Quantity = 72, UnitPrice = 15, Country = "US", State = "NY", City = "Manhattan", Year = "2020", Quarter = "4", Month = "Dec" },








                // Canada

                // British Columbia
                
                // Vancouver
                // Coffee Mug
                new SalesData {Product = "Coffee Mug", Quantity = 5, UnitPrice = 13, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "Coffee Mug", Quantity = 42, UnitPrice = 13, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "Coffee Mug", Quantity = 23, UnitPrice = 15, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "Coffee Mug", Quantity = 19, UnitPrice = 15, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "Coffee Mug", Quantity = 31, UnitPrice = 15, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "Coffee Mug", Quantity = 44, UnitPrice = 15, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "Coffee Mug", Quantity = 62, UnitPrice = 13, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "Coffee Mug", Quantity = 70, UnitPrice = 13, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "4", Month = "Dec" },

                // KVM Switch
                new SalesData {Product = "KVM Switch", Quantity = 15, UnitPrice = 15, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "KVM Switch", Quantity = 42, UnitPrice = 13, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "KVM Switch", Quantity = 95, UnitPrice = 15, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "KVM Switch", Quantity = 70, UnitPrice = 13, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "KVM Switch", Quantity = 93, UnitPrice = 15, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "KVM Switch", Quantity = 95, UnitPrice = 13, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "KVM Switch", Quantity = 5, UnitPrice = 13, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "KVM Switch", Quantity = 96, UnitPrice = 15, Country = "Canada", State = "BC", City = "Vancouver", Year = "2020", Quarter = "4", Month = "Dec" },


                // Prince George
                // Coffee Mug
                new SalesData {Product = "Coffee Mug", Quantity = 14, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "Coffee Mug", Quantity = 22, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "1", Month = "Feb" },

                new SalesData {Product = "Coffee Mug", Quantity = 40, UnitPrice = 13, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "Coffee Mug", Quantity = 38, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "Coffee Mug", Quantity = 54, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "Coffee Mug", Quantity = 3, UnitPrice = 13, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "Coffee Mug", Quantity = 60, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "Coffee Mug", Quantity = 91, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "4", Month = "Dec" },

                // KVM Switch
                new SalesData {Product = "KVM Switch", Quantity = 1, UnitPrice = 13, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "KVM Switch", Quantity = 34, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "KVM Switch", Quantity = 36, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "KVM Switch", Quantity = 9, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "2", Month = "May" },

                new SalesData {Product = "KVM Switch", Quantity = 81, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "KVM Switch", Quantity = 52, UnitPrice = 13, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "KVM Switch", Quantity = 60, UnitPrice = 15, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "KVM Switch", Quantity = 88, UnitPrice = 13, Country = "Canada", State = "BC", City = "Prince George", Year = "2020", Quarter = "4", Month = "Dec" },



                // Quebec
                // Dorval Lodge
                // Coffee Mug
                new SalesData {Product = "Coffee Mug", Quantity = 77, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "Coffee Mug", Quantity = 4, UnitPrice = 15, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "Coffee Mug", Quantity = 73, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "Coffee Mug", Quantity = 28, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "Coffee Mug", Quantity = 40, UnitPrice = 15, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "Coffee Mug", Quantity = 57, UnitPrice = 15, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "Coffee Mug", Quantity = 7, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "Coffee Mug", Quantity = 98, UnitPrice = 15, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "4", Month = "Dec" },

                // KVM Switch
                new SalesData {Product = "KVM Switch", Quantity = 57, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "KVM Switch", Quantity = 73, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "KVM Switch", Quantity = 17, UnitPrice = 15, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "KVM Switch", Quantity = 40, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "KVM Switch", Quantity = 87, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "KVM Switch", Quantity = 46, UnitPrice = 15, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "KVM Switch", Quantity = 83, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "KVM Switch", Quantity = 1, UnitPrice = 13, Country = "Canada", State = "QU", City = "Dorval Lodge", Year = "2020", Quarter = "4", Month = "Dec" },


                // Manhattan
                // Coffee Mug
                new SalesData {Product = "Coffee Mug", Quantity = 47, UnitPrice = 15, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "Coffee Mug", Quantity = 17, UnitPrice = 15, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "1", Month = "Feb" },

                new SalesData {Product = "Coffee Mug", Quantity = 58, UnitPrice = 13, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "Coffee Mug", Quantity = 9, UnitPrice = 13, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "2", Month = "Apr" },

                new SalesData {Product = "Coffee Mug", Quantity = 90, UnitPrice = 13, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "Coffee Mug", Quantity = 13, UnitPrice = 15, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "Coffee Mug", Quantity = 87, UnitPrice = 13, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "Coffee Mug", Quantity = 5, UnitPrice = 15, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "4", Month = "Dec" },

                // KVM Switch
                new SalesData {Product = "KVM Switch", Quantity = 63, UnitPrice = 15, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "1", Month = "Jan" },
                new SalesData {Product = "KVM Switch", Quantity = 77, UnitPrice = 13, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "1", Month = "Jan" },

                new SalesData {Product = "KVM Switch", Quantity = 96, UnitPrice = 13, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "2", Month = "Apr" },
                new SalesData {Product = "KVM Switch", Quantity = 78, UnitPrice = 15, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "2", Month = "May" },

                new SalesData {Product = "KVM Switch", Quantity = 89, UnitPrice = 13, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "3", Month = "Sept" },
                new SalesData {Product = "KVM Switch", Quantity = 33, UnitPrice = 15, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "3", Month = "Sept" },

                new SalesData {Product = "KVM Switch", Quantity = 75, UnitPrice = 13, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "4", Month = "Nov" },
                new SalesData {Product = "KVM Switch", Quantity = 92, UnitPrice = 15, Country = "Canada", State = "QU", City = "Ottawa", Year = "2020", Quarter = "4", Month = "Dec" }

            };
        }


        
    }
}
