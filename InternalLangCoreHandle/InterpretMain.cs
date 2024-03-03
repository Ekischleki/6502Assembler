using TASI.InterpretStartup;

using TASI.InitialisationObjects;

namespace TASI.InternalLangCoreHandle
{
    public static class InterpretMain
    {



        /*
        public static List<VarConstruct> InterpretCopyVar(List<Command> commands)
        {
            bool statementMode = false;
            CommandLine? commandStatement = new(new(), -1);
            List<VarConstruct> result = new();

            if (commands.Last().commandType != Command.CommandTypes.EndCommand) commands.Add(new(Command.CommandTypes.EndCommand, ";", commands.Last().commandStatement));

            foreach (Command command in commands)
            {

                if (statementMode)
                {
                    if (command.commandType != Command.CommandTypes.EndCommand)
                    {
                        commandStatement.commands.Add(command);
                        continue;
                    }

                    if (commandStatement.commands.Count != 2 && commandStatement.commands[0].commandType == Command.CommandTypes.Statement && commandStatement.commands[1].commandType == Command.CommandTypes.Statement) throw new CodeSyntaxException("Invalid VarCopy statement.\nRight way of using it:<statemt: var one> <statement: var two>;");
                    if (commandStatement.commands.Count == 3) // Is link
                    {
                        if (commandStatement.commands[1].commandType != Command.CommandTypes.Statement || commandStatement.commands[2].commandType != Command.CommandTypes.Statement) throw new CodeSyntaxException("Invalid VarConstruct link statement.\nRight way of using it:link <statemt: var type> <statement: var name>;");
                        if (!Enum.TryParse<VarConstruct.VarType>(commandStatement.commands[1].commandText.ToLower(), out VarConstruct.VarType varType)) throw new CodeSyntaxException($"The variable type \"{commandStatement.commands[0].commandText.ToLower()}\" is invalid.");
                        result.ForEach(x =>
                        {
                            if (x.name == commandStatement.commands[1].commandText.ToLower()) throw new CodeSyntaxException($"A variable with the name {commandStatement.commands[1].commandText.ToLower()} already exists. Keep in mind, that variable names are not case sensitive.");
                        });
                        result.Add(new(varType, commandStatement.commands[2].commandText.ToLower(), true));
                    }
                    else
                    {
                        if (commandStatement.commands[0].commandType != Command.CommandTypes.Statement || commandStatement.commands[1].commandType != Command.CommandTypes.Statement) throw new CodeSyntaxException("Invalid VarConstruct statement.\nRight way of using it:<statemt: var type> <statement: var name>;");
                        if (!Enum.TryParse<VarConstruct.VarType>(commandStatement.commands[0].commandText.ToLower(), out VarConstruct.VarType varType)) throw new CodeSyntaxException($"The variable type \"{commandStatement.commands[0].commandText.ToLower()}\" is invalid.");
                        result.ForEach(x =>
                        {
                            if (x.name == commandStatement.commands[1].commandText.ToLower()) throw new CodeSyntaxException($"A variable with the name {commandStatement.commands[1].commandText.ToLower()} already exists. Keep in mind, that variable names are not case sensitive.");
                        });

                        result.Add(new(varType, commandStatement.commands[1].commandText.ToLower()));

                    }
                    statementMode = false;
                    continue;
                }


                switch (command.commandType)
                {
                    case Command.CommandTypes.Statement:
                        Global.CurrentLine = command.commandStatement;
                        statementMode = true;
                        commandStatement = new(new List<Command> { command }, 1);
                        break;
                    default:
                        throw new NotImplementedException($"You can only use statements in VarConstruct-mode.");
                }
            }
            return result;
        }

        */


        

        


        internal static bool ComparePaths(string path1, string path2)
        {
            return Path.GetFullPath(path1).Replace('\\', '/').ToLower() == Path.GetFullPath(path2).Replace('\\', '/').ToLower();
        }

        /// <summary>
        /// Contains all statements in the interpret normal mode. The string is the statement name.
        /// </summary>
        

        public static void InterpretNormalMode(IEnumerable<Command> commands, AccessableObjects accessableObjects)
        {
            
            //More or less the core of the language. It uses a Command-List and loops over every command, it then checks the command type and calls the corrosponding internal functions to the code.
            bool statementMode = false;
            List<Command> commandStatement = new();
            foreach (Command command in commands)
            {
                accessableObjects.global.CurrentFile = command.commandFile;

                accessableObjects.cancellationTokenSource?.Token.ThrowIfCancellationRequested();
                if (statementMode)
                {
                    if (command.commandType == Command.CommandTypes.EndCommand)
                    {
                        if (accessableObjects.global.DebugMode)
                        {

                        }
                        
                        InterpretationHelp.HandleStatement(commandStatement, accessableObjects);
                        

                        statementMode = false;
                        continue;
                    }
                    commandStatement.Add(command);
                    continue;
                }


                switch (command.commandType)
                {
                    
                    case Command.CommandTypes.EndCommand:
                        //Just ignore it
                        break;
                    case Command.CommandTypes.Statement:
                        accessableObjects.global.CurrentLine = command.commandLine;
                        statementMode = true;
                        commandStatement = new List<Command> { command };
                        break;
                    default:
                        throw new NotImplementedException($"You can't use a {command.commandType}-type directly.");
                }
            }
            if (statementMode) throw new CodeSyntaxException("Seems like you forgot a semicolon (;)");

            return;
        }
    }
}
