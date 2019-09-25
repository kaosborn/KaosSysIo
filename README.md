# KaosSysIo

<a href="https://github.com/kaosborn/KaosSysIo/blob/master/.github/workflows/Build.yml">
<img src="https://github.com/kaosborn/KaosSysIo/workflows/Build/badge.svg"></a>
<a href="https://github.com/kaosborn/KaosSysIo/blob/master/.github/workflows/Test.yml">
<img src="https://github.com/kaosborn/KaosSysIo/workflows/Test/badge.svg"></a>

KaosSysIo is a .NET codebase that provides classes for traversing directories.
Included is the `tree2.exe` utility program that is an improved version
of the Windows `tree.exe` program.

Primary types included in the codebase are:

* `Kaos.SysIo.Node` - represents a directory node
* `Kaos.SysIo.NodeVector` - represents a list of directory nodes
* `Kaos.SysIo.DirWalker` - a lite alternative to Node/NodeVector
* `Kaos.Collections.QueuedStack` - a stack with a queue of items to push

The provided `.nuget` library is built as a .NET Standard project with multitargeting.
This library does not include the `DirWalker` class which must be consumed via source code.

### Library installation

To install using a direct reference to a `.dll` binary:

1. Download the `.nuget` package:

   * https://github.com/kaosborn/KaosSysIo/releases/

2. As archives, individual binaries may be extracted from the `.nuget` package for specific platforms.
A project may then reference the extracted platform-specific `.dll` directly.

### `tree2.exe` installation

The provided utility program is a standalone executable that requires .NET 4.0.
Copy this program to a directory in PATH for an improved version of Microsoft's `tree.exe` program.

### Documentation

Installing as a NuGet package will provide IntelliSense and object browser documentation from the `.xml` file.

### Build

Complete source code with embedded XML documentation is hosted at GitHub:

* https://github.com/kaosborn/KaosSysIo/releases/

### Layout

This repository is a single Visual Studio solution with additional files in the root.

* The `Bench` folder contains console program projects that mostly target the .NET 4.61 library build.
These programs exist to:

  * Provide examples for documentation
  * Exercise classes in this library

* The `ConTree` folder contains the build for the `tree2.exe` .NET 4.0 console program.

* The `SysIo` folder contains the primary build of the class library.
Building the Release configuration of the project contained in this folder
will produce a `.nuget` file for distribution.

* The `SysIo461` folder contains a .NET 4.6.1 build of the class library.
This project is used for development and testing only.

* The `Source` folder contains all library source code.
All source is organized using shared projects which are referenced by the build projects.

* The `Test461` folder contains a few unit tests.
