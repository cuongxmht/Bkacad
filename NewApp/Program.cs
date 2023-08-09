using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NewApp.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NewApp.DB;

var configurations=new ConfigurationBuilder().AddJsonFile("appsettings.json");
var config=configurations.Build();
var connectionString=config.GetConnectionString("Database");

var services=new ServiceCollection();
services.AddDbContext<HumanDbContext>(option=>option.UseSqlite(connectionString));
 
// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

// string strA="123", strB="432";

// int.TryParse(strA, out int a);

// int.TryParse(strB, out int b);

// System.Console.WriteLine($"{strA} + {strB} = {a+b}");

/*
//Luong = LCB*HeSo + PC
int LCB=1790000, PC=2000000;
float HeSo=3.25F;
string hs;
while(true){
    System.Console.Write("Nhap he so luong: ");
    hs=Console.ReadLine();
    if(!float.TryParse(hs, out HeSo) )
    {
        System.Console.WriteLine("He so luong nhap khong dung.");
    } 
    else if(HeSo<=0)
        System.Console.WriteLine("He so luong phai >0.");
    else if(HeSo>0) break;
    System.Console.WriteLine("");
}

System.Console.WriteLine($"LCB={LCB}, HeSo={HeSo}, PC={PC}");
System.Console.WriteLine($"Luong = LCB*HeSo + PC");
System.Console.WriteLine($"Luong = {LCB*HeSo + PC}");
*/
/*
Employee employee=new Employee();
employee.NhapThongTin();
employee.HienThi();
*/
//Huong doi tuong, ke thua
/*
Student student=new Student();
student.CreateNew();
student.ShowInfo();
*/
//Mang, ds
System.Console.Write("Nhap so phan tu: ");
int n=Convert.ToInt32(Console.ReadLine());
Person[] arrPerson=new Person[n];
for(int i=0;i<n;i++)
{
    System.Console.WriteLine($"{i+1}");
    Person person=new Person();
    person.CreateNew();
    arrPerson[i]=person;
}
System.Console.WriteLine("\nHien thi thong tin");
for(int i=0;i<n;i++)
{
    System.Console.WriteLine($"-----{i+1}------");
    arrPerson[i].ShowInfo();
}





