![](./assets/construction-resized.jpeg)

# Commander
Commander is a simple, extensible, and configurable command framework for C# developers.

It's based on a simple idea of giving string input to the program and returning a string containing the result of the execution of the command based on the input.

# Features
- Automatic command naming based on method name (overridable by attribute).
- Automatic conversion and piping of string input to relevant data type(s) based on the command's method parameter.
- Automatic usage example generation.
- Automatic command documentation based on method attributes and the method itself.
- Automatic input splitting based on whitespace, preserves single and double quoted strings.
- Automatic command dispatching.
- Includes a default help command.
- Add custom parameter convertors.
- Add custom input splitters.
- Add custom help command.
- Detailed command documentation with special character sequences.
- Command groups and nestable commands.
- Required and optional command parameters, with optional parameters having default value(s).
- Organizational unit 'Cog' which is a class that contains commands and it can optionally group commands together.
- Dead simple execution through the program Run method.
- Error handling.

# Installation
TODO

# Documentation
TODO

[Generated Code Docs](./documentation/html/index.html)

# Applications
Commander can be used to implement command systems for games, command line tools, chat bots etc.

It accomplishes this goal by enforcing the input of a command string to the program and forcing all commands to return a string containing the result of the commands' execution.

# Quickstart
Below is a simple example of how to use this framework.
```c#
/* ExampleCog.cs */
using Commander;

namespace Quickstart {
    public class ExampleCog : Commander.Cog {
        public ExampleCog(Program program) : base(program) {
            /* SETUP CODE HERE */
        }
        
        [Commander.Command(Description="Repeats the given message.")]
        [Commander.Example("@c hello")]
        [Commander.Example("@c 'hello world!'")]
        public string Echo(string message) {
            return message;
        }
        
        [Commander.Command(Description="Adds numbers together.")]
        [Commander.Example("@c 10")]
        [Commander.Example("@c 5 16")]
        public string Add(int a, int b = 10) {
            return (a + b).ToString();
        }
    }
}
```

```c#
/* ExampleProgram.cs */
using Commander;

namespace Quickstart {
    public class ExampleProgram : Commander.Program
    {
        public ExampleProgram() : base("ExampleProgram")
        {
            /* SETUP CODE HERE */
            
            Register(new ExampleCog(this));
        }
        
        public static void Main(string[] args) {
            var program = new ExampleProgram();
            Console.WriteLn(program.Run("ExampleProgram echo 'hello world!'"));
            Console.WriteLn(program.Run("ExampleProgram add 400 20"));
            Console.WriteLn(program.Run("ExampleProgram help"));
            Console.WriteLn(program.Run("ExampleProgram help add"));
        }
    }
}
```

# Examples
TODO

#### TODO
- ~~[ ] Finish Unit tests.~~
- ~~[ ] Add flag parser.~~
- [x] Add argument preprocessor for commands ala discordpy ?
      E.g.: myFunc(string arg1, int arg2)
- [x] Add code comments
- [x] Complete unit tests (current: 85% Coverage)
- [x] Add code docs
- [ ] Add examples
- [ ] Prettify README.md
- [x] Add command usage info generation from metadata
- [x] Add special sequence parser
- [x] Add input splitter to program
- [x] Add default help command
- ~~This all could have gone in a project right?~~