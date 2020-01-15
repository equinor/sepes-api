# How to use .puml files

## Requirements
* [Plant UML Version: 2018.14](https://sourceforge.net/projects/plantuml/files/1.2018.14/)

## Recomended tools
* [PlantUML preview for VSCode](https://github.com/qjebbs/vscode-plantuml)
    * Its important to note you need to select the 2018.14 version of PlantUML as the renderer.

## Setup 
* 

## How to create C4 diagrams in Plant UML
* Get familiar with the [C4 plugin for Plant UML](https://github.com/RicardoNiepel/C4-PlantUML)
* Include the C4 plugin on the line directly after the @startuml tag
```
!includeurl https://raw.githubusercontent.com/RicardoNiepel/C4-PlantUML/release/1-0/C4_Container.puml
```


## How to generate images
* To see all possible flags check [the command line guide](https://plantuml.com/command-line)
* Generate images with this command
```
java -jar {path to plantum.jar} {path to drawing} -{tsvg/tjpg/tpng}
```

* Example: Generate all .puml in directory into .svg format
```
Assumes .jar is in directory of the drawings
java -jar plantuml.1.2018.14.jar *.puml -tsvg
```

