using System;
namespace NewApp.Models
{
    public class Employee{
        public String HoTen { get; set; }
        public string  DiaChi { get; set; }
        public int NamSinh { get; set; }
public Employee(string ht,string dc)
{
    HoTen=ht;
    DiaChi=dc;
}
public Employee()
{
    
}
        public void NhapThongTin()
        {
            System.Console.Write("Ho ten: ");
            this.HoTen = Console.ReadLine();

            System.Console.Write("Dia chi: ");
            this.DiaChi = Console.ReadLine();
            System.Console.Write("Nam: ");
            this.NamSinh = int.Parse(Console.ReadLine());
        }

        public void HienThi()
        {
            System.Console.WriteLine("------");
            System.Console.WriteLine($"Nhan vien: {HoTen} - {DiaChi}");
            System.Console.WriteLine($"Tuoi: {TinhTuoi()}");
        }
        public int TinhTuoi()
        {
            return DateTime.Now.Year - NamSinh;
        }
    }
}