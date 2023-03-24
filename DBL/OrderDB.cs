using Models;

namespace DBL
{
    public class OrderDB : BaseDB<Order>
    {
        public override string GetPKColumnName()
        {
            return "Id";
        }
        protected override string GetTableName()
        {
            return "Orders";
        }
        protected override Order CreateModel(object[] row)
        {
            Order o = new Order();
            o.Id = int.Parse(row[0].ToString());
            o.OrderDateTime = DateTime.Parse(row[1].ToString());
            int custID = int.Parse(row[2].ToString());
            o.Customer = new CustomerDB().GetModelByPk(custID.ToString());
            //TODO List of products
            return o;
        }
        protected override async Task<Order> CreateModelAsync(object[] row)
        {
            Order o = new Order();
            o.Id = int.Parse(row[0].ToString());
            o.OrderDateTime = DateTime.Parse(row[1].ToString());
            int custID = int.Parse(row[2].ToString());
            CustomerDB customerDB = new CustomerDB();
            Customer customer = await customerDB.GetModelByPkAsync(custID.ToString());
            o.Customer = customer;
            //TODO List of products
            return o;
        }

        public async Task<List<Order>> GetAllOrdersByCustomerIDAsync(int customerID)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add(new CustomerDB().GetPKColumnName(), customerID.ToString());
            return await SelectAllAsync(parameters:p);
        }

        public async Task<bool> InsertAsync(Order order)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>();
            DateTime d = DateTime.Now;
            string dts = $"'{d.Year}-{d.Month}-{d.Day} {d.Hour}:{d.Minute}:{d.Second}'";
            fillValues.Add("OrderDateTime", dts);
            fillValues.Add(new CustomerDB().GetPKColumnName(), order.Customer.Id.ToString());
            return await base.InsertAsync(fillValues) == 1;
        }

        public async Task<Order?> InsertGetObjAsync(Order order)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>();
            DateTime d = DateTime.Now;
            string dts = $"'{d.Year}-{d.Month}-{d.Day} {d.Hour}:{d.Minute}:{d.Second}'";
            fillValues.Add("OrderDateTime", dts);
            fillValues.Add(new CustomerDB().GetPKColumnName(), order.Customer.Id.ToString());
            return await base.InsertGetObjAsync(fillValues);
        }

        public async Task<int> UpdateAsync(Order order)
        {
            Dictionary<string, string> fillValues = new Dictionary<string, string>();
            Dictionary<string, string> filterValues = new Dictionary<string, string>();
            DateTime d = DateTime.Now;
            string dts = $"'{d.Year}-{d.Month}-{d.Day} {d.Hour}:{d.Minute}:{d.Second}'";
            fillValues.Add("OrderDateTime", dts);
            filterValues.Add(GetPKColumnName(), order.Id.ToString());
            return await base.UpdateAsync(fillValues, filterValues);
        }
    }
}
