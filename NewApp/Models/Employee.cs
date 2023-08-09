using System;
namespace NewApp.Models
{
    public class Employee:Person
    {
        public String EmployeeId { get; set; }
        
        public void CreateNew()
        {
            base.CreateNew();
            System.Console.Write("EmployeeId: ");
            this.EmployeeId = Console.ReadLine();
        }

        public void ShowInfo()
        {
            base.ShowInfo();
            
            string info=$"Ma NV: {EmployeeId}";
            
            System.Console.WriteLine(info);
        }
        
    }
}