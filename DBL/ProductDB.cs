using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBL
{
    internal class ProductDB : BaseDB<Product>
    {
        private const string ProductsTableName = "Products";

        public override string GetPKColumnName()
        {
            return "Id";
        }

        protected override Product CreateModel(object[] row)
        {
            int len = 5;
            if (row?.Length == len)
            {
                Product p = new Product();
                p.Id = int.Parse(row[0].ToString());
                p.Name = row[1].ToString();
                p.Description = row[2].ToString();
                p.Category = row[3].ToString();
                p.SubCategory = row[4].ToString();
                //TODO List of images
                return p;
            }
            throw new Exception($"Invalid table {GetTableName()} configuration - should have at leadt {len} columns.");
        }

        protected override async Task<Product> CreateModelAsync(object[] row)
        {
            int len = 5;
            if (row?.Length == len)
            {
                Product p = new Product();
                p.Id = int.Parse(row[0].ToString());
                p.Name = row[1].ToString();
                p.Description = row[2].ToString();
                p.Category = row[3].ToString();
                p.SubCategory = row[4].ToString();
                //TODO List of images
                return p;
            }
            throw new Exception($"Invalid table {GetTableName()} configuration - should have at leadt {len} columns.");
        }

        protected override string GetTableName()
        {
            return ProductsTableName;
        }
    }
}
