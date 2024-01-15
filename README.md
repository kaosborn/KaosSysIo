# KaosSysIo

<a href="https://github.com/kaosborn/KaosSysIo/actions/workflows/build.yml">
<img src="https://github.com/kaosborn/KaosSysIo/workflows/Build/badge.svg"></a>
<a href="https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml">
<img src="https://github.com/kaosborn/KaosSysIo/workflows/Test/badge.svg"></a>

<a href="https://github.com/kaosborn/KaosSysIo/actions/workflows/Test.yml">
<img src="https://github.com/kaosborn/KaosSysIo/workflows/Test1/badge.svg"></a>

<a href="https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml">
<img src="https://github.com/kaosborn/KaosSysIo/actions/workflows/Test/badge.svg"></a>

<a href="https://github.com/kaosborn/KaosSysIo/actions/workflows/Test.yml">
<img src="https://github.com/kaosborn/KaosSysIo/actions/workflows/Test1/badge.svg"></a>

<a href="https://github.com/kaosborn/KaosSysIo/actions/workflows/test">
<img src="https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml/badge.svg"></a>

<a href="https://github.com/kaosborn/KaosSysIo/actions/workflows/Test">
<img src="https://github.com/kaosborn/KaosSysIo/actions/workflows/Test.yml/badge.svg"></a>

XXX

[![Test](https://github.com/kaosborn/KaosSysIo/actions/workflows/Test/badge.svg)](https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml)
[![Test](https://github.com/kaosborn/KaosSysIo/actions/workflows/test/badge.svg)](https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml)
[![Test](https://github.com/kaosborn/KaosSysIo/actions/workflows/Test1/badge.svg)](https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml)
[![Test](https://github.com/kaosborn/KaosSysIo/actions/workflows/test1/badge.svg)](https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml)
[![Test](https://github.com/kaosborn/KaosSysIo/actions/workflows/Test.yml/badge.svg)](https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml)
[![Test](https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml/badge.svg)](https://github.com/kaosborn/KaosSysIo/actions/workflows/test.yml)


### Overview

The **KaosSysIo** repository contains a .NET codebase that provides classes for traversing directories.
Included is the `tree2.exe` utility program that is an improved version
of the Windows `tree.exe` console program.

Primary types included in this codebase are:

* `Kaos.SysIo.Node` - represents a directory node
* `Kaos.SysIo.NodeVector` - represents a list of directory nodes
* `Kaos.SysIo.DirWalker` - a lite alternative to Node/NodeVector
* `Kaos.Collections.QueuedStack` - a stack with a queue of items to push

### `tree2.exe`

The current build of `tree2.exe` targets .NET Framework 4.8.
This is the result of building the `ConTree` project.
Copy this program to a directory in PATH for an improved version of Microsoft's `tree.exe` program.

```
Usage:

tree2 [drive:][path] [/F] [/A] [/W] [/SL] [/SN] [/n]

/F   Display the names of the files in each folder.
/A   Use ASCII instead of extended characters.
/W   Produce output suitable for a static HTML web page.
/SL  Sort lexically.
/SN  Sort naturally.
/n   Indent by n where n is a number.
```

Not in Windows' `tree.exe` are the `/S`, `/W`, and `/n` switches.

#### Example session 1

The first example changes the indent from 4 to 2 spaces with the `/2` switch.
Files as well as folders are listed with the `/F` switch.
ASCII characters are used for the outline by supplying the `/A` switch.

````
T:\>tree2 T:\Unicode /A /F /2

T:\Unicode
| Unicode charts.url
|
+-Charts
|   U0000 - Controls and Basic Latin.pdf
|   U0080 - C1 Controls.pdf
|   U0100 - Latin Extended - A.pdf
|   U0180 - Latin Extended - B.pdf
|   U2500 - Box Drawing.pdf
|   U2C60 - Latin Extended - C.pdf
|   UFFF0 - Specials.pdf
|
\-Data
  | UCD-150209.zip
  | UCD-160620.zip
  |
  \-extracts
      UnicodeData-160620.txt
````

#### Example session 2

The original `tree.exe` program produces results that are sorted however the file system returns them.
The `tree2.exe` program emulates this behavior.
First, files on a FAT32 drive are listed with the `/F` switch:

```
T:\>tree2.exe T:\Numbers /F
T:\Numbers
    20 - twenty.txt
    5 - five.txt
    100 - hundred.txt
```

Without sorting, the files are listed in a seemingly random order.
Next, a natural sort is applied to the results with the `/SN` switch:

```
T:\>tree2.exe T:\Numbers /F /SN
T:\Numbers
    5 - five.txt
    20 - twenty.txt
    100 - hundred.txt
```

#### Example session 3

This example produces a static web page
after first setting the shell's code page to UTF-8 with the Windows `chcp` command.
Then a web page with fully collapsible folders is produced with the `/W` switch.
Lexical sorting is used for the hexidecimals with the `/SL` switch.

````
T:\>chcp 65001
Active code page: 65001

T:\>tree2 T:\Unicode /A /F /SL /W > MyPage.html
T:\>msedge.exe MyPage.html
````

Browser output:

![example 3](/Images/contree-example-unicode-1.png)  
(All subfolders are expanded in this example.)

### Top-level folders

This repository is a single Visual Studio solution with additional files in the root.

* `Bench` - Contains console programs that:

  * Provide examples for documentation.
  * Exercise classes in this library.

* `ConTree` - Contains the build for the `tree2.exe` .NET console program.
Targeting .NET Framework 4.8 produces an executable of only 24K.

* `SysIo` - Builds a class library in the form of a `.nuget` file.

* `Source` - All source code in shared projects by class.

* `TestCore` - Contains a few unit tests.

### License

All work here falls under the [MIT License](/LICENSE).