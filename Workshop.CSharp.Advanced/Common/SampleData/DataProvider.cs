using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Globalization;

namespace Workshop.CSharp.Advanced
{
    public static class DataProvider
    {
        private static Product[] _products;
        public static Product[] Products
        {
            get { return _products ??= ReadXmlElements("Products.xml").Deserialize<Product>().ToArray(); }
        }

        private static Category[] _categories;
        public static Category[] Categories
        {
            get { return _categories ??= ReadXmlElements("Categories.xml").Deserialize<Category>().ToArray(); }
        }

        private static IEnumerable<XElement> ReadXmlElements(string fileName)
        {
            var filePath = Path.Combine(Consts.ProjectFolderPath, "Common/SampleData", fileName);
            var fileContent = File.ReadAllText(filePath);
            return XElement.Parse(fileContent).Elements();
        }

        private static IEnumerable<T> Deserialize<T>(this IEnumerable<XElement> elements)
            where T : new()
        {
            var propertyMap = typeof(T).GetProperties().ToDictionary(p => p.Name);

            foreach (var entityElement in elements)
            {
                var obj = new T();
                foreach (var fieldElement in entityElement.Elements())
                {
                    if (propertyMap.TryGetValue(fieldElement.Name.LocalName, out var property))
                    {
                        var value = Convert.ChangeType(fieldElement.Value, property.PropertyType, CultureInfo.InvariantCulture.NumberFormat);
                        property.SetValue(obj, value, null);
                    }
                }
                yield return obj;
            }
        }
    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int SupplierID { get; set; }
        public int CategoryID { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public short UnitsOnOrder { get; set; }
        public short ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
    }
}
