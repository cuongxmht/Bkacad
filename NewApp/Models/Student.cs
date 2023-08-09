using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApp.Models
{
    public class Student:Person
    {
        public string StudentCode { get; set; }
        public string Department { get; set;}
        public int Point { get; set; }
        public void CreateNew()
        {
            base.CreateNew();       
            this.YearOfBirth=int.Parse(Console.ReadLine());
            System.Console.Write("Ma SV: ");            
            this.StudentCode=Console.ReadLine();
            System.Console.Write("Khoa: ");            
            this.Department=Console.ReadLine();
            
        }

        public void ShowInfo()
        {
            base.ShowInfo();

            string info=@$"
            MaSV: {StudentCode}, 
            Khoa: {Department},   
            Diem: {Point}, 
            ";
            
            System.Console.WriteLine(info);
        }
    }
}