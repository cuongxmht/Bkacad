using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Employee : Person
    {
        public string EmployeeId { get; set; }
        public string Age { get; set; }
    }
}