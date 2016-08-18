using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace DownloadPokemonImages {
    class Program {
        private static string rootFolder = "img";
        private static string baseUrl = "http://archives.bulbagarden.net";
        private static string subUrl = "/w/index.php?title=Category:Ken_Sugimori_Pokémon_artwork&fileuntil=%2A060%0A060Poliwag+RG.png#mw-category-media";

        private static WebRequest web;
        private static HtmlDocument htmlDoc = new HtmlDocument();
        private static WebClient client = new WebClient();

        private static string regexMatch = @"\d{3}\D+ ";

        static void Main(string[] args) {
            do {
                // Starting page to download
                Console.WriteLine("Requesting '" + baseUrl + subUrl + "'");
                web = WebRequest.CreateHttp(baseUrl + subUrl);
                var stream = web.GetResponse().GetResponseStream();
                if (stream == null) {
                    Console.WriteLine("Got empty stream");
                    return;
                }
                // Load the stream
                htmlDoc.Load(stream);
                // Download images on current page
                DownloadImages();
                // Find the next page
                GetNextPage();
                Console.WriteLine("============================================");
            } while (subUrl != null);
            // Check if all main pokemon where correctly downloaded
            Console.WriteLine(">>> Checking files...");
            CheckFolder();
        }

        private static void DownloadImages() {
            // Find matching elements
            var imgNodes =
                htmlDoc.DocumentNode.SelectNodes(
                    "//ul[@class='gallery mw-gallery-traditional']/li/div/div/div/a/img[@alt and @src and @srcset]");

            // Iterate through them
            foreach (var node in imgNodes) {
                // Get the original filename
                var origFilename = node.GetAttributeValue("alt", null);
                var foldername = "";
                var newFilename = origFilename;
                // If gamename is specified
                if (Regex.IsMatch(origFilename, regexMatch)) {
                    // Change folder and filename
                    foldername = Regex.Replace(origFilename, regexMatch, "").Replace(".png", "");
                    newFilename = origFilename.Replace(" " + foldername, "");
                }
                // If non-standard name
                if (!Regex.IsMatch(origFilename, @"\d{3}.+\.png")) {
                    // Change folder
                    foldername = "other";
                }
                // Get url
                var url = node.GetAttributeValue("src", null);
                var fullpath = Path.Combine(rootFolder, foldername, newFilename);
                Console.WriteLine("Downloading " + origFilename + " to " + fullpath);
                // Make sure the directory exists
                Directory.CreateDirectory(Path.Combine(rootFolder, foldername));
                // Download the image
                client.DownloadFile(url, fullpath);
            }

        }

        private static void GetNextPage() {
            subUrl = null;
            // Find matching nodes
            var pageNodes =
                htmlDoc.DocumentNode.SelectNodes("//div[@class='mw-category-generated']/div[@id='mw-category-media']/a");
            // Iterate through them
            foreach (var node in pageNodes) {
                // If text value is not 'next page'
                if (!node.InnerText.Contains("next page"))
                    continue; //skip
                // Assign new url
                subUrl = node.GetAttributeValue("href", null).Replace("amp;", "&");
                break;
            }
        }

        private static void CheckFolder() {
            // Get all files in the directory
            var files = Directory.GetFiles(rootFolder).Select(Path.GetFileNameWithoutExtension).ToList();
            // Make sure they are sorted
            files.Sort();
            int current = 1;
            foreach (var file in files) {
                int fileNr;
                if (!int.TryParse(file.Substring(0, 3), out fileNr)) continue;
                if (fileNr > current) {
                    Console.WriteLine("WARNING: Missing " + current);
                    current = fileNr+1;
                    continue;
                }
                if (fileNr == current)
                    current++;
            }
            Console.WriteLine("Known issues:");
            Console.WriteLine("  * Mr. Mime and Mime Jr are in the folders Mime and Jr respectively");
            Console.WriteLine("  * Vivilion is also put in some folders.");
            Console.WriteLine("    - Check the folders Plain, Snow and Ball. Append the folder names.");
            Console.WriteLine("  * Charizard-Mega and Mewtwo-Mega are put in the X and Y folders");
            Console.WriteLine("  * Pikachu Pop and Rock Star are put in the Star folder");
            Console.WriteLine("All other folders should now only contain alternate art");
        }
    }
}
