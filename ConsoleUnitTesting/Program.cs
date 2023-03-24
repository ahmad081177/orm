using Models;
using DBL;
namespace ConsoleUnitTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TEST CustomerDB Get by id
            CustomerDB cdb = new CustomerDB();
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("CustomerID", "1");
            List<Customer> list = cdb.SelectAll(parameters: param);
            if (list?.Count>0)
            {
                Console.WriteLine($" name: {list[0].Name} email: {list[0].Email}\n\n");
            }
            else
            {
                Console.WriteLine("Get Customer by ID failed");
            }

            //TEST CustomerDB SelectAll
            list = cdb.SelectAll();
               
            foreach(Customer item in list)
            {
                Console.WriteLine(@$" id={item.Id} name={item.Name} email={item.Email}");
            }
            //Insert new customer
            Customer c1 = new()
            {
                IsAdmin = true,
                Email = "dummy@gmail.com",
                Name = "Someone",
            };
            bool f1 = cdb.Insert(c1, "123");
            if (f1)
            {
                Console.WriteLine($"Insert {c1.Name} to customers was successful");
            }
            else
            {
                Console.WriteLine($"Insert {c1.Name} to customers failed");
            }
            //Retrive the customer from the DB (since we dont her ID)
            Dictionary<string, string> param2 = new()
            {
                { "Email", c1.Email },
                { "Name", c1.Name }
            };
            var c1_list = cdb.SelectAll(parameters: param2);
            if (c1_list.Count == 1)
            {
                //Delet the newly created one
                int r = cdb.Delete(c1_list[0].Id.ToString());
                if (r == 1)
                {
                    Console.WriteLine($"Delete {c1.Name} from customers was successful");
                }
                else
                {
                    Console.WriteLine($"Delete {c1.Name} from customers failed");
                }
            }
            //Make sure our DB is back to its original
            var list2 = cdb.GetAll();
            if (list2.Count == list.Count)
            {
                Console.WriteLine("Insert after delete ends OK");
            }
            else
            {
                Console.WriteLine("Insert after delete had issues");
            }
        }
    }
}