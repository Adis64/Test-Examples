using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.UnitsOfMeasure;

namespace AFSDKTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AFDatabase database = GetDatabase("PU-PIAFAPPDEV", "Green Power Company");


        }

        static AFDatabase GetDatabase(string server, string database)
        {
            PISystems piSystems = new PISystems();
            PISystem assetServer = piSystems[server];
            AFDatabase afDatabase = assetServer.Databases[database];
            return afDatabase;
        }

        static void PrintRootElements(AFDatabase database)
        {
            Console.WriteLine("Print Root Elements: {0}", database.Elements.Count);
            foreach (AFElement element in database.Elements)
            {
                Console.WriteLine("  {0}", element.Name);
            }

            Console.WriteLine();
        }
    }
}
