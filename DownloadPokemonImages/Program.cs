using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static string rootFolder = "Images";
        private static string baseUrl = "http://archives.bulbagarden.net";

        private static string subUrl =
            "/w/index.php?title=Category:Ken_Sugimori_Pokémon_artwork&fileuntil=%2A060%0A060Poliwag+RG.png#mw-category-media";

        private static WebRequest web;
        private static HtmlDocument htmlDoc = new HtmlDocument();
        private static WebClient client = new WebClient();

        private static string regexMatch = @"\d{3}\D+ ";

        private static List<int> counter = new List<int> { 1, 1, 2, 1, 1, 3, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 6, 2, 2, 2, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1,
                2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 2, 1, 2, 1, 2, 1, 1,
                1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 1, 1, 1, 2, 2, 2,
                2, 3, 1, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1,
                1, 1, 1, 2, 3, 3, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 7, 1, 1, 1, 1, 1, 1, 1, 2,
                1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 3,
                3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 7, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 21, 1, 3, 1, 1, 1, 1, 1, 1, 1, 4, 1, 3, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 5, 2, 2};

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
                var origFilename = node.GetAttributeValue("alt", null).Replace("Ã©", "é");
                var foldername = "";
                var newFilename = origFilename;
                // If card
                if (origFilename.EndsWith("card.jpg")) {
                    // Change folder- and filename
                    foldername = "Cards";
                    newFilename = origFilename.Replace(" card", "");
                }
                // If gamename is specified
                else if (!IsException(origFilename) && Regex.IsMatch(origFilename, regexMatch)) {
                    // Change folder- and filename
                    foldername = Regex.Replace(origFilename, regexMatch, "").Replace(".png", "");
                    if (foldername == "2" && Regex.IsMatch(origFilename, regexMatch + @"\D+ 2.png")) {
                        // Alternate of alternate art
                        var split = origFilename.Split(' ');
                        foldername = split[1];
                        newFilename = split[0] + split[2];
                    } else
                        newFilename = origFilename.Replace(" " + foldername, "");
                }
                // If non-standard name
                else if (!Regex.IsMatch(origFilename, @"\d{3}.+\.png")) {
                    // Change folder
                    foldername = "Other";
                }
                // Get url
                var fullpath = Path.Combine(rootFolder, foldername, newFilename);
                Console.WriteLine("Downloading " + origFilename + " to " + fullpath);
                // Make sure the directory exists
                Directory.CreateDirectory(Path.Combine(rootFolder, foldername));
                // Download the image
                var url = node.GetAttributeValue("src", null);
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
            int last = 1;
            int count = 0;
            bool issueFound = false;
            foreach (var file in files) {
                int fileNr;
                if (!int.TryParse(file.Substring(0, 3), out fileNr)) continue;
                if (fileNr > last) {
                    if (counter[last-1] != count) {
                        Console.WriteLine("Warning: Expected " + counter[last] + " but got " + count + " (id=" + fileNr +
                                          ")");
                        issueFound = true;
                    }
                    if (fileNr - last == 2) {
                        Console.WriteLine("Warning: Missing id=" + (last + 1));
                    } else if (fileNr - last > 2) {
                        Console.WriteLine("Warning: Missing id=" + (last + 1) + " to id=" + (fileNr - 1));
                    }
                    last = fileNr;
                    count = 0;
                }
                if (fileNr == last)
                    count++;
            }
            if (last == counter.Count && counter[last - 1] != count) {
                Console.WriteLine("Warning: Expected " + counter[last] + " but got " + count + " (id=" + counter.Count +
                                  ")");
                issueFound = true;
            }
            if (last != 721) {
                Console.WriteLine("Warning: Missing pokemons id=" + (last+1) + " to id=" + counter.Count);
                Console.WriteLine();
                Console.WriteLine("There is probably something wrong with your internet connection or the wiki page has drastically changed.");
            } else if (issueFound)
                Console.WriteLine("Some issues were found, check the folders for missing images and report it!");
            else
                Console.WriteLine("Download = correct!");
        }

        private static bool IsException(string filename) {
            switch (filename) {
            case "006Charizard-Mega X.png":
            case "006Charizard-Mega Y.png":
            case "025Pikachu-Pop Star.png":
            case "025Pikachu-Rock Star.png":
            case "122Mr. Mime.png":
            case "150Mewtwo-Mega X.png":
            case "150Mewtwo-Mega Y.png":
            case "439Mime Jr.png":
            case "666Vivillon-Poké Ball.png":
            case "666Vivillon-High Plains.png":
            case "666Vivillon-Icy Snow.png":
                return true;
            default:
                return false;
            }
        }
    }
}
