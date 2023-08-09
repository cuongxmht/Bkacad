using System.Collections;
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
        
        
        public void ChangeName(ArrayList arrayList)
        {
            System.Console.Write("\nNhap ten user can sua: ");
            string updUser=Console.ReadLine();

            System.Console.Write("\nTen moi: ");
            string newName=Console.ReadLine();
            if(arrayList?.Count<=0)return;
            foreach(var item in arrayList)
            {
                var ps=(Person)item;
                if(ps.Name !=updUser ) continue;
                ps.Name=newName;
            }
        }
        public void RemoveByName(ArrayList arrayList)
        {
            if(arrayList?.Count<=0)return;
            System.Console.Write("\nNhap ten user can xoa: ");
            string updUser=Console.ReadLine();


            ArrayList delList=new ArrayList();
            foreach(var item in arrayList)
            {
                var ps=(Person)item;
                if(ps.Name !=updUser) continue;
                delList.Add(ps);
            }
            if(delList.Count >0) 
                foreach(var item in delList)    
                    arrayList.Remove(item);

        }
        public void AddNew(ArrayList psList)
        {
            Person ps = new Person();
            ps.CreateNew();
            psList.Add(ps);
        }
    public void ShowInfoAll(ArrayList psList)
        {
            foreach(var item in psList) 
            {
                Person ps=(Person)item;
                string info=@$"Id: {ps.Id}, 
                Ten: {ps.Name}, 
                dia chi: {ps.Address},             
                Tuoi: {ps.GetAge()}";
                
                System.Console.WriteLine(info);
            }
        }
    }
}