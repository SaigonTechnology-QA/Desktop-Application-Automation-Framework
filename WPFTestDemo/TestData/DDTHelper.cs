using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopAppAuto.TestData
{
    internal class DDTHelper
    {
        // A function that returns an IEnumerable of string arrays from a CSV file
        public static IEnumerable<string[]> DDTGetData(string filePath, string[] columns)
        {
            // Open the CSV file
            using var reader = new StreamReader(filePath);

            // Read the first line as the header
            var header = reader.ReadLine();

            // Split the header by pipe
            var columnsName = header.Split('|');

            // Create a dictionary that maps column names to indexes
            var headerIndex = columnsName.Select((name, index) => (name, index)).ToDictionary(pair => pair.name, pair => pair.index);

            // Read the rest of the lines
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Split the line by pipe
                var split = line.Split('|', StringSplitOptions.None);

                // Get the values by column names
                string desc = split[headerIndex["desc"]];
                string price = split[headerIndex["price"]];
                string date = split[headerIndex["date"]];
                string expectedName = split[headerIndex["expectedName"]];

                List<string> list = new List<string>();
                foreach (var column in columns)
                {
                    list.Add(split[headerIndex[column]]);
                }

                // Yield return an array of values
                yield return list.ToArray();
            }
        }
    }
}
