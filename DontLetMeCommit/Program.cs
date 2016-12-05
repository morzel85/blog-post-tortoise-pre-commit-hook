using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DontLetMeCommit
{
    class Program
    {
        const string NotForRepoMarker = "NOT_FOR_REPO";

        static void Main(string[] args)
        {
            string[] affectedPaths = File.ReadAllLines(args[0]);
                        
            foreach (string path in affectedPaths)
            {
                if (ShouldFileBeChecked(path) && HasNotForRepoMarker(path))
                {
                    string errorMessage = $"{NotForRepoMarker} marker found in {path}";
                    Console.Error.WriteLine(errorMessage); // Notice write to Error output stream!
                    Environment.Exit(1);
                }
            }
        }

        static bool ShouldFileBeChecked(string path)
        {
            // Here we are choosing to check only selected file types but you may want to check
            // all the files except specified types or skip filtering altogether...
            Regex filePattern = new Regex(@"^.*\.(cs|js|xml|config)$", RegexOptions.IgnoreCase);

            // List of files affected by the commit might include (re)moved files so we check if file exists...
            return File.Exists(path) && filePattern.IsMatch(path);
        }

        static bool HasNotForRepoMarker(string path)
        {
            using (StreamReader reader = File.OpenText(path))
            {
                string line = reader.ReadLine();

                while (line != null)
                {
                    if (line.Contains(NotForRepoMarker)) 
                        return true; // "Uncommittable" code marker found - let's block the commit!

                    line = reader.ReadLine();
                }
            }

            return false;
        }
    }
}
