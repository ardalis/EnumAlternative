using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EnumReplacement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Roles:");
            foreach (var role in Role.List())
            {
                Console.WriteLine($"Role: {role.Name} ({role.Value})");
            }


            Console.WriteLine("Roles 2:");

            foreach (var role in Role.AllRoles)
            {
                Console.WriteLine($"Role: {role.Name} ({role.Value})");
            }

            Console.WriteLine("Job Titles:");

            foreach (var title in JobTitle.AllTitles)
            {
                Console.WriteLine($"Title: {title.Name} ({title.Value})");
            }

            Console.ReadLine();
        }
    }

    public class Role
    {

        public static Role Author { get; } = new Role(0, "Author");
        public static Role Editor { get; } = new Role(1, "Editor");
        public static Role Administrator { get; } = new Role(2, "Administrator");
        public static Role SalesRep { get; } = new Role(3, "Sales Representative");

        public static List<Role> AllRoles
        {
            get
            {
                return _allRoles;
            }
        }
        // if you move this above the static properties, it fails
        private static List<Role> _allRoles = ListRoles();

        private static List<Role> ListRoles()
        {
            return typeof(Role).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Role))
                .Select(pi => (Role)pi.GetValue(null, null))
                .OrderBy(p => p.Name)
                .ToList();
        }

        public string Name { get; private set; }
        public int Value { get; private set; }

        private Role(int val, string name)
        {
            Value = val;
            Name = name;
        }

        public static IEnumerable<Role> List()
        {
            // alternately, use a dictionary keyed by value
            return new[] { Author, Editor, Administrator, SalesRep };
        }


        public static Role FromString(string roleString)
        {
            return List().Single(r => String.Equals(r.Name, roleString, StringComparison.OrdinalIgnoreCase));
        }

        public static Role FromValue(int value)
        {
            return List().Single(r => r.Value == value);
        }
    }
    public class JobTitle
    {
        // this must appear before other static instance types.
        public static List<JobTitle> AllTitles { get; } = new List<JobTitle>();

        public static JobTitle Author { get; } = new JobTitle(0, "Author");
        public static JobTitle Editor { get; } = new JobTitle(1, "Editor");
        public static JobTitle Administrator { get; } = new JobTitle(2, "Administrator");
        public static JobTitle SalesRep { get; } = new JobTitle(3, "Sales Representative");

        public string Name { get; private set; }
        public int Value { get; private set; }

        private JobTitle(int val, string name)
        {
            Value = val;
            Name = name;
            AllTitles.Add(this);
        }



        public static JobTitle FromString(string roleString)
        {
            return AllTitles.Single(r => String.Equals(r.Name, roleString, StringComparison.OrdinalIgnoreCase));
        }

        public static JobTitle FromValue(int value)
        {
            return AllTitles.Single(r => r.Value == value);
        }
    }
}
