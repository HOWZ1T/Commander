Program (Class):
    Cog (Class):
        Can be a Command Group...
        contains Commands (functions)
            commands can have subcommands from same cog only


[CommandGroup("test")]
class MyCog:
    [Command(name="basic", description="desc")]
    func DoThing(a int, b string) string # test basic 10 "hello"

    [SubCommand(parent="basic", name="com", description="com dec"]
    func DoOtherThing(a int, b int) string # test basic com 10 11
Command name optional, if not given, use function name
