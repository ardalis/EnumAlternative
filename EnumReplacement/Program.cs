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

            Console.WriteLine("--Smart Enums--");
            Console.WriteLine("Smart Foo:");
            foreach (var smartFoo in SmartFoo.List)
            {
                Console.WriteLine($"Foo: {smartFoo.Name} ({smartFoo.Value})");
            }
            Console.WriteLine("Smart Bar:");
            foreach (var smartBar in SmartBar.List)
            {
                Console.WriteLine($"Bar: {smartBar.Name} ({smartBar.Value})");
            }
            Console.WriteLine("Smart Baz:");
            foreach (var smartBaz in SmartBaz.List)
            {
                Console.WriteLine($"Baz: {smartBaz.Name} ({smartBaz.Value})");
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

    public abstract class SmartEnum<TEnum, TValue>
        where TEnum : SmartEnum<TEnum, TValue>
    {
        private static readonly List<TEnum> _list = new List<TEnum>();

        // Despite analysis tool warnings, we want this static bool to be on this generic type (so that each TEnum has its own bool).
        private static bool _invoked;

        public static List<TEnum> List
        {
            get
            {
                if (!_invoked)
                {
                    _invoked = true;
                    // Force invocaiton/initialization by calling one of the derived members.
                    typeof(TEnum).GetProperties(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(p => p.PropertyType == typeof(TEnum))?.GetValue(null, null);
                }

                return _list;
            }
        }

        public string Name { get; }
        public TValue Value { get; }

        protected SmartEnum(string name, TValue value)
        {
            Name = name;
            Value = value;

            TEnum item = this as TEnum;
            List.Add(item);
        }

        public static TEnum FromName(string name)
        {
            return List.Single(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public static TEnum FromValue(TValue value)
        {
            // Can't use == to compare generics unless we constrain TValue to "class", which we don't want because then we couldn't use int.
            return List.Single(item => EqualityComparer<TValue>.Default.Equals(item.Value, value));
        }

        public override string ToString() => Name;
    }

    public class SmartFoo : SmartEnum<SmartFoo, int>
    {
        public static SmartFoo A { get; } = new SmartFoo("Foo A", 1);
        public static SmartFoo B { get; } = new SmartFoo("Foo B", 2);

        private SmartFoo(string name, int value)
            : base(name, value)
        { }
    }

    public class SmartBar : SmartEnum<SmartBar, int>
    {
        public static SmartBar A { get; } = new SmartBar("Bar A", 1);
        public static SmartBar B { get; } = new SmartBar("Bar B", 2);
        public static SmartBar C { get; } = new SmartBar("Bar C", 3);

        private SmartBar(string name, int value)
            : base(name, value)
        { }
    }

    public class SmartBaz : SmartEnum<SmartBaz, string>
    {
        public static SmartBaz A { get; } = new SmartBaz("Baz A", "Baz Val 1");
        public static SmartBaz B { get; } = new SmartBaz("Baz B", "Baz Val 2");

        private SmartBaz(string name, string value)
            : base(name, value)
        { }
    }

}
