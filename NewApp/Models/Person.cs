using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApp.Models
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int YearOfBirth { get; set; }
        public Person(Guid id, string name, string address, int yearOfBirth)
        {
            Id=id;
            Name=name;
            Address=address;
            YearOfBirth =yearOfBirth;
        }
        public Person()
        {
            
        }
        public int GetAge()
        {
            if(YearOfBirth<DateTime.Now.Year) System.Console.WriteLine("Chua nhap ngay sinh, hoac nhap khong dung");
            int age=YearOfBirth-DateTime.Now.Year;
            return age;
        }
    }
}