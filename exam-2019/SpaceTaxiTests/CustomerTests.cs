using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SpaceTaxi;
using SpaceTaxi.Customers;

namespace SpaceTaxiTests {
    public class CustomerTests {
        
        private List<Customer> customers;
        private CustomerCreator customerCreator;
        private List<String> lst;
        private String[] arr;
        
        [SetUp]
        public void CreateObjects() {
            lst = new List<String>();
            customers = new List<Customer>();
        }

        
        /// <summary>
        /// Simple tests som virker åbenlyst sande, aborter på af muligvis evige loops(?)
        /// </summary>
        [Test]
        public void testCustomerAmount() {
            lst.Add("Bob 10 J r 10 100");
            lst.Add("Carol 30 r ^ 10 100");
            string[] arr = lst.ToArray();
            customerCreator = new CustomerCreator();
            customers = customerCreator.CreateCustomers(arr, 1);
            Assert.IsTrue(customers.Count == 2);
        }
    }    
}