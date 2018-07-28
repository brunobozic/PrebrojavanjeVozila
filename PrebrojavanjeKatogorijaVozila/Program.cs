using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PrebrojavanjeKatogorijaVozila
{
    public class Vehicle
    {
        private int _id;
        private double _length;
        private CategoryEnum _category;

        public Vehicle(int id, double length)
        {
            if (id < 0) throw new ArithmeticException("Id must be number larger than -1");
            if (length < 0) throw new ArithmeticException("Id must be number greater or equal than 0");

            _category = DecideCategoryByLength(length);
            Id = id;
            Length = length;
        }

        private static CategoryEnum DecideCategoryByLength(double length)
        {
            var retVal = CategoryEnum.Unknown;

            if (length > 0 && length < 4.5)
            {
                retVal = CategoryEnum.Kategorija1;
            }
            else if (length > 4.5 && length < 6.4)
            {
                retVal = CategoryEnum.Kategorija2;
            }
            else if (length > 6.4 && length < 14.6)
            {
                retVal = CategoryEnum.Kategorija3;
            }
            else if (length > 14.6)
            {
                retVal = CategoryEnum.Kategorija4;
            }

            return retVal;
        }

        public enum CategoryEnum
        {
            Kategorija1,
            Kategorija2,
            Kategorija3,
            Kategorija4,
            Unknown
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public double Length
        {
            get => _length;
            set => _length = value;
        }

        public CategoryEnum Category
        {
            get => _category;
            set => _category = value;
        }
    }

    public class DataParser
    {
        private readonly string[] _dataReadFromFileRaw;

        public DataParser(string[] listOfLinesReadFromFile)
        {
            _dataReadFromFileRaw = listOfLinesReadFromFile;
        }

        public IEnumerable<Vehicle> Parse()
        {
            List<Vehicle> listOfVehicles = new List<Vehicle>();
            if (_dataReadFromFileRaw.Length > 0)
            {
                foreach (var line in _dataReadFromFileRaw)
                {
                    var vehicleId = -1;
                    double vehicleLength = -1;

                    if (!string.IsNullOrEmpty(line) && line.Contains(":"))
                    {
                        var parsedForVehicleId = int.TryParse(line.Split(Convert.ToChar(":"))[1], out var parsedIntegerForVehicleId);
                        if (parsedForVehicleId)
                        {
                            vehicleId = parsedIntegerForVehicleId;
                        }

                        bool parsedForVehicleLength = double.TryParse(line.Split(Convert.ToChar(":"))[0], out double parsedIntegerForVehicleLength);
                        if (parsedForVehicleLength)
                        {
                            vehicleLength = parsedIntegerForVehicleLength;
                        }

                        var vehicle = new Vehicle(vehicleId, vehicleLength);

                        listOfVehicles.Add(vehicle);
                    }
                }
            }
            return listOfVehicles;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            WriteToConsole("Program koji prebrojava kategorije vozila!");

            var locationOfExecutingAssembly = System.Reflection.Assembly.GetAssembly(typeof(Program)).Location;
            var fileSystemPath = Path.GetDirectoryName(locationOfExecutingAssembly);
            const string subFolder = "\\SourceData\\";
            const string fileName = "PodaciC.txt";

            var fullPathToDate = string.Format("{0}{1}{2}", fileSystemPath, subFolder, fileName);

            if (!File.Exists(fullPathToDate))
            {
                WriteToConsole(string.Format(
                    "The file we were looking ([ {0} ]) for does not exist at the target location ([ {1} ])", fileName,
                    fullPathToDate));
            }

            WriteToConsole(Environment.NewLine + " Učitavanje podataka! ");
            WriteToConsole(" Otvaranje datoteke! ");
            var listOfLinesReadFromFile = File.ReadAllLines(fullPathToDate);



            var prepareData = new DataParser(listOfLinesReadFromFile);
            var vehicles = prepareData.Parse();


            WriteToConsole(" Ispis prebrojanih vozila! ");
            var enumerable = (IEnumerable<Vehicle>)vehicles;

            WriteToConsole(string.Format(" {0} | {1} ", "1:", enumerable.Count(v => v.Category.Equals(Vehicle.CategoryEnum.Kategorija1))));
            WriteToConsole(string.Format(" {0} | {1} ", "2:", enumerable.Count(v => v.Category.Equals(Vehicle.CategoryEnum.Kategorija2))));
            WriteToConsole(string.Format(" {0} | {1} ", "3:", enumerable.Count(v => v.Category.Equals(Vehicle.CategoryEnum.Kategorija3))));
            WriteToConsole(string.Format(" {0} | {1} ", "4:", enumerable.Count(v => v.Category.Equals(Vehicle.CategoryEnum.Kategorija4))));

            var prosjecnaDuljinaVozila = enumerable.Average(v => v.Length);
            var standardnaDevijacija = enumerable.Select(v => v.Length).StandardDeviation();

            WriteToConsole(string.Format("Prosječna duljina vozila iznosi {0} m, s standardnom devijacijom u iznosu od {1} m!", Math.Round(prosjecnaDuljinaVozila, 2), Math.Round(standardnaDevijacija, 2)));

            Console.ReadLine();
        }

        private static void WriteToConsole(string format)
        {
            Console.WriteLine(format);
        }
    }

    public static class StDev
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
    }
}
