using System.Collections.Generic;

namespace SpaceTaxi.Customers {
    public class CustomerCreator {

        public List<Customer> CreateCustomers(string[] customerdata) {
            var retval = new List<Customer>();
            foreach (var customer in customerdata) {
                var customerParams = customer.Split(null);
                retval.Add(new Customer(
                    customerParams[0],
                    customerParams[1],
                    customerParams[2][0],
                    customerParams[3],
                    customerParams[4],
                    customerParams[5]));
            }
            return retval;
        }
    }
}