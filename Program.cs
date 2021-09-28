using System;
using System.Collections.Generic;
using System.IO;
/*Дан файл с информацией о родственниках.
* на первой строке задан порядок полей. Однако порядок может меняться Id;LastName;FirstName;BirthDate так и FirstName;Id;BirthDate;LastName
* далее идет неизвестное кол-во строк, в которых задаются данные людей в порядке, описанном в первой строке файла.
* после пустой строки идет описание отношений между людьми: spouse - супруг/супруга, parent - родитель, sibling - брат/сестра.
Формат строки с отшениями: по обе стороны от оператор "<->" задаются люди, которые состоят в отношениях, после знака равенства идет тип отношений, в которых эти люди состоят.
Задание:
1) разработать объектную модель по описанию выше
2) считать данные из файла и сохранить в объекты
3) разработать метод, чтобы для каждого человека можно было узнать в каком типе отношений он состоит с другим человеком. 
Например, у файла ниже для людей с Id 1 и 2 должен вернуться тип "sibling", хотя этого не было указано в файле явно.

Пример файла:
Id;LastName;FirstName;BirthDate
1;Иванов;Иван;01.01.1990
2;Иванова;Мария;01.05.1998
3;Иванов;Петр;01.01.1980
4;Иванова;Ольга;01.01.1971
5;Петрова;Маргарита;01.01.1972
6;Иванов;Олег;02.03.1995

3<->4=spouse
3<->1=parent
4<->1=parent
5<->3=sibling
6<->4=sibling*/
namespace relatives_lab
{
    class Person
    {
        public int Id;
        public string LastName;
        public string FirstName;
        public DateTime BirthDate;
        public static Dictionary<int, int> OrderTranslator = new Dictionary<int, int>();

        public Person Spouse; //супруг/супруга
        public Person[] Parents = new Person[2]; //родитель
        public List<Person> Sibling = new List<Person>(); //брат/сестра

        public Person(string[] str)
        {
            Id = Convert.ToInt32(str[OrderTranslator[0]]);
            LastName = str[OrderTranslator[1]];
            FirstName = str[OrderTranslator[2]];
            BirthDate = Convert.ToDateTime(str[OrderTranslator[3]]);
            Program.AllPeople.Add(Id, this);
        }
    }

    static class Program
    {
        public static Dictionary<int, Person> AllPeople = new Dictionary<int, Person>();

        static void Main(string[] args)
        {
            var file = new StreamReader("file.txt");
            ReadingFirstString(file);
            CreatePersons(file);
            FindRelatives(file);

        }
        public static void FindRelatives(StreamReader file)
        {
            string str;
            while ((str = file.ReadLine()) != string.Empty)
            {
                var array = str.Replace("<->", "=").Split('=');
                Person first = AllPeople[Convert.ToInt32(array[0])];
                Person second = AllPeople[Convert.ToInt32(array[1])];
                
                switch (array[2])
                {
                    case "spouse":
                        AddSpouse(first, second);
                        break;
                    case "parents":
                        AddParents(first, second);
                        break;
                    case "sibling":
                        AddSibling(first, second);
                        break;
                }
            }
        }

        public static void AddSpouse(Person first, Person second)
        {
            first.Spouse = second;
            second.Spouse = first;
        }

        public static void AddParents(Person first, Person second)
        {
            if (second.Parents[0] == null || second.Parents[0] != first)
                second.Parents[0] = first;
            else if(second.Parents[1] != first)
                second.Parents[1] = first;
        }

        public static void AddSibling(Person first, Person second)
        {
            if(!first.Sibling.Contains(second))
                first.Sibling.Add(second);
            if (second.Sibling.Contains(first))
                second.Sibling.Add(first);
        }

        public static void CreatePersons(StreamReader file)
        {
            string str;
            while ((str = file.ReadLine()) != string.Empty)
            {
                new Person(str.Split(';'));
            }
        }

        public static void ReadingFirstString(StreamReader file)
        {
            var firstString = file.ReadLine().Split(';');

            for (int i = 0; i < 4; i++)
            {
                switch (firstString[i])
                {
                    case "Id":
                        Person.OrderTranslator.Add(0, i);
                        break;
                    case "LastName":
                        Person.OrderTranslator.Add(1, i);
                        break;
                    case "FirstName":
                        Person.OrderTranslator.Add(2, i);
                        break;
                    case "BirthDate":
                        Person.OrderTranslator.Add(3, i);
                        break;
                }
            }
        }

    }
}
