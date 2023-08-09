using System.ComponentModel.DataAnnotations;

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
        public void CreateNew()
        {
            System.Console.Write("Ten: ");            
            this.Name=Console.ReadLine();
            System.Console.Write("Dia chi: ");            
            this.Address=Console.ReadLine();
            System.Console.Write("Nam sinh: ");            
            this.YearOfBirth=int.Parse(Console.ReadLine());
            
            this.Id=Guid.NewGuid();
        }
        public int GetAge()
        {
            if(YearOfBirth>DateTime.Now.Year) System.Console.WriteLine("Chua nhap ngay sinh, hoac nhap khong dung");
            int age=DateTime.Now.Year-YearOfBirth;
            return age;
        }

        public void ShowInfo()
        {
            string info=@$"Id: {Id}, 
            Ten: {Name}, 
            dia chi: {Address},             
            Tuoi: {GetAge()}";
            
            System.Console.WriteLine(info);
        }
    }
}