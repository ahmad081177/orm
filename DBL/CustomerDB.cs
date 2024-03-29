﻿using Models;

namespace DBL
{
    public class CustomerDB : BaseDB<Customer>
    {
       
        protected override string GetTableName()
        {
            return "Customers";
        }
        public override string GetPKColumnName()
        {
            return "Id";
        }
        protected override Customer CreateModel(object[] row)
        {
            if (row?.Length == 5)
            {
                Customer c = new Customer();
                c.Id = int.Parse(row[0].ToString());
                c.Name = row[1].ToString();
                c.Email = row[2].ToString();
                c.IsAdmin = bool.Parse(row[4].ToString());
                return c;
            }
            throw new Exception($"Invalid table {GetTableName()} configuration - should have at leadt 5 columns.");
        }
        protected override async Task<Customer> CreateModelAsync(object[] row)
        {
            if (row?.Length == 5)
            {
                Customer c = new()
                {
                    Id = int.Parse(row[0].ToString()),
                    Name = row[1].ToString(),
                    Email = row[2].ToString(),
                    IsAdmin = bool.Parse(row[4].ToString())
                };
                return c;
            }
            throw new Exception($"Invalid table {GetTableName()} configuration - should have at leadt 5 columns.");
        }
       
        public async Task<bool> InsertAsync(Customer customer,string password)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>
            {
                { "Name", customer.Name },
                { "Email", customer.Email },
                { "CustomerPassword", password }
            };
            return await base.InsertAsync(fillValues) == 1;
        }

        public bool Insert(Customer customer, string password)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>
            {
                { "Name", customer.Name },
                { "Email", customer.Email },
                { "CustomerPassword", password }
            };
            return base.Insert(fillValues) == 1;
        }

        public async Task<Customer?> InsertGetObjAsync(Customer customer, string password)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>()
            {
                { "Name", customer.Name },
                { "Email", customer.Email },
                { "CustomerPassword", password }
            };
            return await base.InsertGetObjAsync(fillValues);
        }

        public Customer? InsertGetObj(Customer customer, string password)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>()
            {
                { "Name", customer.Name },
                { "Email", customer.Email },
                { "CustomerPassword", password }
            };
            return base.InsertGetObj(fillValues);
        }

        public async Task<int> UpdateAsync(Customer customer)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>();
            Dictionary<string, string> filterValues = new Dictionary<string, string>();
            fillValues.Add("Name", customer.Name);
            fillValues.Add("Email", customer.Email);
            filterValues.Add(GetPKColumnName(), customer.Id.ToString());
            return await base.UpdateAsync(fillValues, filterValues);
        }

        public int Update(Customer customer)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>();
            Dictionary<string, string> filterValues = new Dictionary<string, string>();
            fillValues.Add("Name", customer.Name);
            fillValues.Add("Email", customer.Email);
            filterValues.Add(GetPKColumnName(), customer.Id.ToString());
            return base.Update(fillValues, filterValues);
        }

        public async Task<int> UpdatePasswordAsync(Customer customer,string password)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>();
            Dictionary<string, string> filterValues = new Dictionary<string, string>();
            fillValues.Add("Name", customer.Name);
            fillValues.Add("Email", customer.Email);
            fillValues.Add("CustomerPassword", password);
            filterValues.Add(GetPKColumnName(), customer.Id.ToString());
            return await base.UpdateAsync(fillValues, filterValues);
        }

        public int UpdatePassword(Customer customer, string password)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>();
            Dictionary<string, string> filterValues = new Dictionary<string, string>();
            fillValues.Add("Name", customer.Name);
            fillValues.Add("Email", customer.Email);
            fillValues.Add("CustomerPassword", password);
            filterValues.Add(GetPKColumnName(), customer.Id.ToString());
            return base.Update(fillValues, filterValues);
        }


        // specific queries
        public async Task<string> GetPasswordAsync(int id)
        {
            string sql = @$"SELECT {GetTableName()}.CustomerPassword FROM {GetTableName()} WHERE ({GetPKColumnName()} = @id)";
            AddParameterToCommand("@id", id);
            string oldPassword = (string)await ExecScalarAsync(sql);
            return oldPassword;
        }

        public string GetPassword(int id)
        {
            string sql = @$"SELECT {GetTableName()}.CustomerPassword FROM {GetTableName()} WHERE ({GetPKColumnName()} = @id)";
            AddParameterToCommand("@id", id);
            string oldPassword = (string)ExecScalar(sql);
            return oldPassword;
        }

        public async Task<List<Customer>> GetNonAdminsAsync()
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("IsAdmin", "0");
            return await SelectAllAsync(parameters: p);
        }

        public List<Customer> GetNonAdmins()
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("IsAdmin", "0");
            return SelectAll(parameters:p);
        }

        public async Task<List<(string, string)>> GetName_Email4NonAdminsAsync()
        {
            List<(string, string)> returnList = new List<(string, string)>();
            string sql = $"select Name, Email from {GetTableName()}";
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("IsAdmin", "0");
            List<object[]> list = await base.StingListSelectAllAsync(sql, p);
            foreach (object[] item in list)
            {
                string name = item[0].ToString();
                string email = item[1].ToString();
                returnList.Add((name, email));
            }
            return returnList;
        }

        public List<(string, string)> GetName_Email4NonAdmins()
        {
            List<(string, string)> returnList = new List<(string, string)>();
            string sql = $"select Name, Email from {GetTableName()}";
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("IsAdmin", "0");
            List<object[]> list = base.StingListSelectAll(sql, p);
            foreach (object[] item in list)
            {
                string name = item[0].ToString();
                string email = item[1].ToString();
                returnList.Add((name, email));
            }
            return returnList;
        }

        public async Task<Customer?> GetCustomerByOrderIDAsync(int orderID)
        {
            string sql = @$"Select {DbName}.{GetTableName()}.*
                           From {DbName}.{GetTableName()} Inner Join {DbName}.orders 
                           On {DbName}.orders.{GetPKColumnName()} = {DbName}.{GetTableName()}.{GetPKColumnName()}";
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add(DbName+".orders.OrderID", orderID.ToString());
            List<Customer> list = await SelectAllAsync(sql, p);
            if (list.Count == 1)
                return list[0];
            else
                return null;
        }

        public Customer? GetCustomerByOrderID(int orderID)
        {
            string sql = @$"Select {DbName}.{GetTableName()}.*
                           From {DbName}.{GetTableName()} Inner Join {DbName}.orders 
                           On {DbName}.orders.{GetPKColumnName()} = {DbName}.{GetTableName()}.{GetPKColumnName()}";
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add(DbName+".orders.OrderID", orderID.ToString());
            List<Customer> list = SelectAll(sql, p);
            if (list.Count == 1)
                return list[0];
            else
                return null;
        }
    }
}
