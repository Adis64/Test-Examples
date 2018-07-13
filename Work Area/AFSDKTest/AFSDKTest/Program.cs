using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Search;
using OSIsoft.AF.UnitsOfMeasure;

namespace AFSDKTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AFDatabase database = GetDatabase("PU-PIAFAPPDEV", "DevTest");
            //PrintRootElements(database);
            //PrintElementTemplate(database);
            //PrintSearchResults(database,"Meter");
            //PrintSearchByTemplate(database, "MeterBasic");
            FindMetersAboveAverage(database, 300);

            Console.WriteLine("Press ENTER key to close");
            Console.ReadLine();
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
            AFElements afEl = database.Elements;
            foreach (AFElement element in afEl)
            {
                Console.WriteLine("  {0}", element.Name);
            }

            Console.WriteLine();
        }

        static void PrintElementTemplate(AFDatabase database)
        {
            Console.WriteLine("Print Elements: {0}", database.ElementTemplates.Count);
            foreach (AFElementTemplate elementtemplate in database.ElementTemplates)
            {
                Console.WriteLine(" {0}", elementtemplate.Name);
            }
        }


        static void PrintSearchResults(AFDatabase database, string SearchString)
        {
            Console.WriteLine("Query String is : {0}", SearchString);

            var QueryString = string.Format("*{0}*", SearchString);
            //var QueryString = SearchString;

            using (AFElementSearch search = new AFElementSearch(database, "My Search", QueryString))
            {
                search.CacheTimeout = TimeSpan.FromMinutes(5);
                foreach (AFElement element in search.FindElements())
                {
                    Console.WriteLine("Element Name: {0}, Template: {1} , Categories: {2}",element.Name,element.Template,element.CategoriesString);
                }
            }

        }

        static void PrintSearchByTemplate(AFDatabase database, string SearchTemplate)
        {
            Console.WriteLine("Query String is : {0}", SearchTemplate);

            var QueryString = string.Format("TemplateName: {0}", SearchTemplate);
            //var QueryString = SearchString;

            using (AFElementSearch search = new AFElementSearch(database, "My Template Search", QueryString))
            {
                search.CacheTimeout = TimeSpan.FromMinutes(5);
                foreach (AFElement element in search.FindElements())
                {
                    Console.WriteLine("Element Name: {0}, Template: {1} , Categories: {2}", element.Name, element.Template, element.CategoriesString);
                }
            }

        }

        static void FindMetersAboveAverage(AFDatabase database, double AverageVal)
        {
            Console.WriteLine("Average limit is : {0}", AverageVal);

            AFElementSearch elesearch = new AFElementSearch(database, "Find Average Above Limit", "*Meter*");

            foreach(AFElement ele in elesearch.FindElements()
            {   

            }
        }

    }
}
