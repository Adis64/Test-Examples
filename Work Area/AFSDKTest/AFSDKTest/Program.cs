using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Data;
using OSIsoft.AF.Time;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Search;
using OSIsoft.AF.UnitsOfMeasure;

namespace AFSDKTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AFDatabase database = GetDatabase("PU-PIAFAPPDEV", "DevTest"); //GetDatabase("pisrv01", "Green Power Company"); 
            //PrintRootElements(database);
            //PrintElementTemplate(database);
            //PrintSearchResults(database,"Meter");
            //PrintSearchByTemplate(database, "MeterBasic");
            //FindMetersAboveAverage(database, 300);
            //PrintHistorical(database, "Meter001", "*-2h", "*");
            //PrintInterpolated(database, "Meter001", "*-2h", "*", TimeSpan.FromSeconds(10));
            //PrintHourlyAverage(database, "Meter001", "*-2h", "*");
            //CreateFeederRootElement(database);
            //CreateFeederElements(database,"Feeder005");
            CreateWeakReference(database);

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
            string templateName = "MeterBasic";
            string attributeName = "Energy Usage";


            AFElementSearch elesearch = new AFElementSearch(database, "Find Average Above Limit", string.Format("template:\"{0}\" \"|{1}\":>{2}", templateName, attributeName, AverageVal));

            foreach(AFElement ele in elesearch.FindElements())
            {
                Console.WriteLine("Element Name: {0}, Template: {1} , Categories: {2}", ele.Name, ele.Template, ele.CategoriesString);
            }
        }

        static void PrintHistorical(AFDatabase database, string meterName, string startTime, string endTime)
        {
            Console.WriteLine("Print Interpolated Values - Meter: {0}, Start: {1}, End: {2}", meterName, startTime, endTime);

            AFAttribute attr = AFAttribute.FindAttribute(@"\Meters\" + meterName + @"|Energy Usage", database);

            AFTime start = new AFTime(startTime);
            AFTime end = new AFTime(endTime);
            AFTimeRange timeRange = new AFTimeRange(start, end);

            AFValues vals = attr.Data.RecordedValues(timeRange: timeRange, boundaryType: AFBoundaryType.Inside, desiredUOM: null, filterExpression: null, includeFilteredValues: false);

            foreach (AFValue val in vals)
            {
                Console.WriteLine("Timestamp (UTC): {0}, value (kJ): {1}", val.Timestamp.UtcTime, val.Value);
            }
        }

        static void PrintInterpolated(AFDatabase database, string meterName, string start, string end, TimeSpan interval)
        {
            AFTime startTime = new AFTime(start);
            AFTime endTime = new AFTime(end);
            AFTimeRange timeRange = new AFTimeRange(startTime, endTime);

            AFAttribute att = AFAttribute.FindAttribute(@"\Meters\" + meterName + @"|Energy Usage", database);
            AFTimeSpan intervalNew = new AFTimeSpan(interval);
            AFValues values = att.Data.InterpolatedValues(timeRange: timeRange, interval: intervalNew, desiredUOM: null, filterExpression: null, includeFilteredValues: false);

        }

        static void PrintHourlyAverage(AFDatabase database, string meterName, string start, string end)
        {
            Console.WriteLine("Print Interpolated Values - Meter: {0}, Start: {1}, End: {2}", meterName, start, end);

            string abc = @"\Meters\" + meterName + @"|Energy Usage";

            AFAttribute attr = AFAttribute.FindAttribute(@"\Meters\" + meterName + @"|Energy Usage", database);

            AFTime startTime = new AFTime(start);
            AFTime endTime = new AFTime(end);
            AFTimeRange timeRange = new AFTimeRange(startTime, endTime);


            IDictionary<AFSummaryTypes, AFValues> vals = attr.Data.Summaries(timeRange: timeRange, summaryDuration: new AFTimeSpan(TimeSpan.FromHours(1)), summaryType: AFSummaryTypes.Average, calcBasis: AFCalculationBasis.TimeWeighted, timeType: AFTimestampCalculation.EarliestTime);


            foreach (AFValue val in vals[AFSummaryTypes.Average])
            {
                Console.WriteLine("Timestamp: {0:yyyy-MM-dd HH\\h}, Value: {1:0.00}  {2}", val.Timestamp.LocalTime, val.Value, val.UOM.Abbreviation);
            }
        }

        static void CreateFeederRootElement(AFDatabase database)
        {
            Console.WriteLine("Creating the Feeders root element");
            if (database.Elements.Contains("Feeders"))
                return;

            database.Elements.Add("Feeders");
            database.CheckIn();
        }

        static void CreateFeederElements(AFDatabase database, String FeederName)
        {
            Console.WriteLine("Creating new element using feeder template..");

            AFElementTemplate template = database.ElementTemplates["Feeders"];

            AFElement feeder = database.Elements["Feeders"];


            if (template == null || feeder == null)
            {
                Console.WriteLine("Feeder template and parent missing");
                return;
            }

            if (feeder.Elements.Contains(FeederName)) return;
            AFElement feeder01 = feeder.Elements.Add(FeederName, template);

            AFAttribute city = feeder01.Attributes["City"];
            if (city != null) city.SetValue(new AFValue("London"));

            AFAttribute power = feeder01.Attributes["Power"];
            power.ConfigString = @"\\PU-PIDARCDEV01\SINUSOID";

            if (database.IsDirty)
                database.CheckIn();
        }

        static void CreateWeakReference(AFDatabase database)
        {
            Console.WriteLine("Creating a weak referenc of the Feeder01 under London");

            AFReferenceType weakRefType = database.ReferenceTypes["Weak Reference"];

            AFElement london = database.Elements["Geographical Locations"].Elements["London"];
            AFElement feeder = database.Elements["Feeders"].Elements["Feeder005"];

            if (feeder != null || london != null)
            {
                if (!london.Elements.Contains(feeder))
                    london.Elements.Add(feeder, weakRefType);
            }


            if (london.IsDirty)
                london.CheckIn();

        }
    }
}
