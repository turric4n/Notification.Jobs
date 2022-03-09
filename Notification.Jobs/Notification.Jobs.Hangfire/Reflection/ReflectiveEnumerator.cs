using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Notification.Jobs.Hangfire.Reflection
{
    public static class ReflectiveEnumerator
    {
        static ReflectiveEnumerator() { }

        public static List<Assembly> LoadAllBinDirectoryAssemblies(string pattern)
        {
            string binPath = AppDomain.CurrentDomain.BaseDirectory; // note: don't use CurrentEntryAssembly or anything like that.

            List<Assembly> listOfAssemblies = new List<Assembly>();

            foreach (string dll in Directory.GetFiles(binPath, $"{pattern}.dll", SearchOption.AllDirectories))
            {
                try
                {
                    listOfAssemblies.Add(Assembly.LoadFile(dll));
                }
                catch (FileLoadException loadEx)
                { } // The Assembly has already been loaded.
                catch (BadImageFormatException imgEx)
                { } // If a BadImageFormatException exception is thrown, the file is not an assembly.

            } // foreach dll

            return listOfAssemblies;
        }

        public static List<Assembly> GetListOfEntryAssemblyWithReferences()
        {
            List<Assembly> listOfAssemblies = new List<Assembly>();
            var mainAsm = Assembly.GetEntryAssembly();
            listOfAssemblies.Add(mainAsm);

            foreach (var refAsmName in mainAsm.GetReferencedAssemblies())
            {
                listOfAssemblies.Add(Assembly.Load(refAsmName));
            }
            return listOfAssemblies;
        }

        public static IEnumerable<Assembly> GetAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                    if (!list.Contains(reference.FullName))
                    {
                        try
                        {
                            stack.Push(Assembly.Load(reference));
                            list.Add(reference.FullName);
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e);
                        }

                    }

            }
            while (stack.Count > 0);

        }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            objects.Sort();
            return objects;
        }
    }
}
