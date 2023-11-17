# LuaDeobfuscator

## Description

A command line tool to deobfuscate .lua files from various mobile games by Level5.

## Usage

Various options have to be set to properly use the command line tool.

| Option | Description |
| - | - |
| -f | The file or directory to deobfuscate. |

## Features

It can deobfuscate 3 common obfuscation methods employed in the lua scripts.
- Strings converted from decimal notation to proper UTF8
- Resolve function names
- Inline function invocation names and parameters

## Examples

To deobfuscate a .lua script:<br>
```LuaDeobfuscator.exe -f Path/To/File.lua```
