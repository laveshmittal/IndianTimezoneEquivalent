using System;
using System.IO;
using System.Text;

namespace IndianTimezoneEquivalent
{
    class Program
    {
        //Path of CSV file containing Data
        const string PATH =
            @"..\..\..\time.csv";


        public static DateTime GetIndianTimeEquivalent(string dateTime,string cityName)
        {
            //Parsing given dateTime string into dateTime object
            var givenDateTime = DateTime.Parse(dateTime);
            var flag = false;
            TimeZoneInfo cityTimeZoneinfo = TimeZoneInfo.Utc;
            foreach (var systemTimeZone in TimeZoneInfo.GetSystemTimeZones())
            {
                if (systemTimeZone.DisplayName.ToLower().Contains(cityName.ToLower()))
                {
                    flag = true;
                    cityTimeZoneinfo = systemTimeZone;
                    break;
                }
            }

            Console.WriteLine($"{cityName}:{flag}");
            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(givenDateTime, cityTimeZoneinfo.Id, indiaTimeZone.Id);
        }

        
        public static string[] GetRowsFromCsvFile(string path)
        {
            var fileData = File.ReadAllText(path).Trim();
            return fileData.Split("\n");
        }

        public static string GetTimeZoneAppendedRow(string row)
        {
            var data = row.Split(",", 2);
            //Converting data {"Wed, 15 Sep 2021 19:04:22 GMT",} to {Wed, 15 Sep 2021 19:04:22 GMT}
            var temp = data[1].Split(",",2);
            var timeString = temp[0]
                .Replace("\"", "")
                .Replace(",", "")
                .Trim();

            var indianTimeEquivalent = GetIndianTimeEquivalent(timeString,data[0])
                .ToString("F");
            
            //Writing the row with time equivalent back to new file
            //Remove the last empty block and append new time
            return String.Format($"{data[0]},{timeString},\"{indianTimeEquivalent}\"\n");

        }
        static void Main(string[] args)
        {
            //Creating a result csv in same directory of csv and opening in write mode
            var resultCsvPath = $"{Directory.GetParent(PATH)}\\result.csv";
            var resultCsvFile = new FileInfo(resultCsvPath).Open(FileMode.Create, FileAccess.ReadWrite);


            //Reading data of given file
            var rows = GetRowsFromCsvFile(PATH);

            //Writing the headers into result csv from given csv
            var columns = rows[0].Replace("\n","").Split(',');
            resultCsvFile.Write(Encoding.ASCII.GetBytes($"{columns[0]},{columns[1]},Indian Time Equivalent\n" ));


            //traverse rows of csv to write back modified row into resultant csv
            for (int i = 1; i < rows.Length; i++)
            {   
                if (string.IsNullOrWhiteSpace(rows[i])) continue;
                var row = GetTimeZoneAppendedRow(rows[i]);
                resultCsvFile.Write(Encoding.ASCII.GetBytes(row));
            }
            resultCsvFile.Close();
            File.Delete(PATH);
            File.Move(resultCsvPath,PATH);



        }
    }
}
