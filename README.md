# DownloadPokemonImages
Automatically downloads all thumbnails of http://archives.bulbagarden.net/wiki/Category:Ken_Sugimori_Pok%C3%A9mon_artwork and the following pages.

## Dependencies
Makes use of System packages and [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack).

## Usage
Compile and run.

It creates a folder 'img' next to the .exe file and places all image files under it. It creates subfolders based on the filename, which has some flaws. See the next table to know which pokémon are placed in which wrong folder. The foldername should be appended to the pokemon name, originally with a space in between.

| ID  | Pokémon                | Folder |
|-----|------------------------|--------|
| 006 | Charizard-Mega X       | X      |
| 006 | Charizard-Mega Y       | Y      |
| 025 | Pikachu (Pop Star)     | Star   |
| 025 | Pikachu (Rock Star)    | Star   |
| 122 | Mr. Mime               | Mime   |
| 150 | Mewtwo-Mega X          | X      |
| 150 | Mewtwo-Mega Y          | Y      |
| 439 | Mime Jr                | Jr     |
| 666 | Vivillon (Poké Ball)   | Ball   |
| 666 | Vivillon (High Plains) | Plains |
| 666 | Vivillon (Icy Snow)    | Snow   |
