// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string strA="123", strB="432";

int.TryParse(strA, out int a);

int.TryParse(strB, out int b);

System.Console.WriteLine($"{strA} + {strB} = {a+b}");