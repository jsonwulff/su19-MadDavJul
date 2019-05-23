using System.Collections.Generic;

namespace SpaceTaxi.Customers {
    public class CustomerCreator {
        
        /// <summary>
        /// Instantiates customers according to given level-data
        /// </summary>
        /// <param name="customerdata"> Data to be split into each customer</param>
        /// <param name="levelNumber"> Current level number</param>
        /// <returns></returns>
        public List<Customer> CreateCustomers(string[] customerdata, int levelNumber) {
            var retval = new List<Customer>();
            foreach (var customer in customerdata) {
                var customerParams = customer.Split(null);
                retval.Add(new Customer(
                    customerParams[0],
                    customerParams[1],
                    customerParams[2][0],
                    customerParams[3],
                    customerParams[4],
                    customerParams[5],
                    levelNumber));
            }
            return retval;
        }
    }
}