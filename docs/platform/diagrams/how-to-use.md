# How to use .puml files

## Requirements
* Plant UML Version: 2018.14 https://sourceforge.net/projects/plantuml/files/1.2018.14/

## Recomended reading???
* Get familiar with the C4 plugin for Plant UML https://github.com/RicardoNiepel/C4-PlantUML
## Set up for editing
* 

## How to create C4 diagrams in Plant UML
* Include the C4 plugin on the line directly after the @startuml tag
```
!includeurl https://raw.githubusercontent.com/RicardoNiepel/C4-PlantUML/release/1-0/C4_Container.puml
```

## How to generate images
* To see all possible tags check: TODOURL
* Generate images with this command
```
java -jar {path to plantum.jar} {path to drawing} -{tsvg/tjpg/tpng}
```

* Example: Generate all .puml in directory into .svg format
```
Assumes .jar is in directory of the drawings
java -jar plantuml.1.2018.14.jar *.puml -tsvg
```

