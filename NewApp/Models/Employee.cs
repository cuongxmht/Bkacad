using System;
namespace NewApp.Models
{
    public class Employee{
        public String HoTen { get; set; }
        public string  DiaChi { get; set; }

        public void NhapThongTin()
        {
            System.Console.Write("Ho ten: ");
            this.HoTen = Console.ReadLine();

            System.Console.Write("Dia chi: ");
            this.DiaChi = Console.ReadLine();

        }

        public void HienThi()
        {
            System.Console.WriteLine("------");
            System.Console.WriteLine($"Nhan vien: {HoTen} - {DiaChi}");
        }
    }
}