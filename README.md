# PADscript
an open source scripting language built upon PAD technology and Lua. From Jarvis home automation inc.

# Pre-requirements
1. .NET Core 3.0 Runtime 
3. For the usage of speech as of 2019-11-24 only windows users will need .NET 4.5+ for speech usage.
2. Thats it!

# Usage & Features
PADscript like other scripting languages allow for developers to write scripts to perform tasks. 
You can open the console to enter script commands or run a script file using `-f` of `--file`
to execute a PADScript File.

The file type is filename.pad or filename.lua since PADScript is built upon lua.

### plugins
To add additional commands in PADScript, you will need to make a C# .net Core library class project.
Once the project is made, add what ever functions you want in it and the name of each function will be
the same as the command name in PADScript. Within your AppData directory look for PADScript folder, then under
directory /plugins/Library/lib here just place the .dll file and it will be added automatically.

## Featues
1. The ability to add additional functions in the scripting language with C# .NET core 3.0 dll libraries.
  a. The directory is in the root of the program called Library.
2. With the syntax of lua, have access to additional features programmed into PADScript.


# Future Plans
- add a complete core of all fundamental functions towards each operating system.
- a UI generator for more complex applications
- auto-gen documentation of the commands inside padScript (can sorta do this with PrintCommands())
- add Speech for windows
- add GPIO support for raspberry pi
- add library version to integrate into .NET projects for developers.


