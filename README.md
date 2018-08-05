# MergeCoberturaXml
This tool merges multiple Cobertura XML files into one single file.

The tool was written because Visual Studio Team Services only takes one Cobertura file as coverage result. Multiple test projects would probably result in multiple files.

## Installation

The package is published on [nuget.org](https://www.nuget.org/packages/MergeCoberturaXml).

```
dotnet tool install  MergeCoberturaXml -g
```

## Usage

```
MergeCoberturaXml -i Project1.xml;Project2.xml;Project3.xml -o Merged.xml
```
