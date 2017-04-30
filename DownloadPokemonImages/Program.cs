using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DownloadPokemonImages {
    class Program {
        /// <summary>
        ///     Base folder to store the images to
        /// </summary>
        private static string rootFolder = "Images";

        /// <summary>
        ///     Base URL to get images from
        /// </summary>
        private static string baseUrl = "http://archives.bulbagarden.net";

        /// <summary>
        ///     Together with <see cref="baseUrl"/> this forms the page to download the pokemons from
        /// </summary>
        private static string subUrl =
                "/w/index.php?title=Category:Ken_Sugimori_Pokémon_artwork&fileuntil=%2A060%0A060Poliwag+RG.png#mw-category-media"
            ;

        /// <summary>
        ///     Allows to fetch the contents of the site
        /// </summary>
        private static WebRequest web;

        /// <summary>
        ///     Allows for easy parsing through the html document
        /// </summary>
        private static HtmlDocument htmlDoc = new HtmlDocument();

        /// <summary>
        ///     Allows to request files
        /// </summary>
        private static WebClient client = new WebClient();

        /// <summary>
        ///     Regex to match whether or not a game name is specified in the image name
        /// </summary>
        private static string regexMatch = @"\d{3}\D+ ";

        /// <summary>
        ///     Entry point of the application
        /// </summary>
        /// <param name="args"></param>
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
        }

        /// <summary>
        ///     Downloads all the images of the current page
        /// </summary>
        private static void DownloadImages() {
            // Find matching elements
            var imgNodes =
                htmlDoc.DocumentNode
                       .SelectNodes("//ul[@class='gallery mw-gallery-traditional']/li/div/div/div/a/img[@alt and @src and @srcset]");

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

        /// <summary>
        ///     Fetches the next page to download more images
        /// </summary>
        private static void GetNextPage() {
            subUrl = null;
            // Find matching nodes
            var pageNodes =
                htmlDoc.DocumentNode
                       .SelectNodes("//div[@class='mw-category-generated']/div[@id='mw-category-media']/a");
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

        /// <summary>
        ///     Some pokemomens have a special name, do not consider them as erronous. A switch clausule is in essence a hash table
        ///     in C#.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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
