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
            System.Console.Write("Ten: ");            
            this.Name=Console.ReadLine();
            System.Console.Write("Dia chi: ");            
            this.Address=Console.ReadLine();
            System.Console.Write("Nam sinh: ");            
            this.YearOfBirth=int.Parse(Console.ReadLine());
            System.Console.Write("Ma SV: ");            
            this.StudentCode=Console.ReadLine();
            System.Console.Write("Khoa: ");            
            this.Department=Console.ReadLine();
            this.Id=Guid.NewGuid();
        }

        public void ShowStudent()
        {
            string info=@$"Id: {Id}, 
            Ten: {Name}, 
            dia chi: {Address}, 
            MaSV: {StudentCode}, 
            Khoa: {Department},   
            Diem: {Point}, 
            Tuoi: {GetAge()}";
            
            System.Console.WriteLine(info);
        }
    }
}