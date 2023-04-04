﻿using System.Collections.Generic;

namespace TASI
{



    internal class Statement
    {
        public static string[] staticStatements = { "set" };

        public static Var StaticStatement(CommandLine commandLine, List<Var> accessableVars)
        {
            Var returnValue = new();
            if (commandLine.commands[0].commandType != Command.CommandTypes.Statement)
                throw new Exception("Internal: StaticStatements must start with a Statement");

            switch (commandLine.commands[0].commandText.ToLower())
            {

                case "return":
                    if (commandLine.commands.Count == 1) return new(new Var());
                    if (commandLine.commands.Count < 2) throw new Exception("Invalid return statement usage; Right usage: return <value>;");
                    return new(GetVarOfCommandLine(new(commandLine.commands.GetRange(1, commandLine.commands.Count - 1), -1), accessableVars));

                case "set":
                    //Validate syntax
                    StaticStatementSet(commandLine, accessableVars);
                    return new();
                case "while":
                    CommandLine checkStatement = new(new(), -1);
                    for (int i = 1; i < commandLine.commands.Count; i++)
                    {
                        if (commandLine.commands[i].commandType == Command.CommandTypes.CodeContainer)
                            break;
                        checkStatement.commands.Add(commandLine.commands[i]);
                    }
                    if (commandLine.commands.Count != checkStatement.commands.Count + 2)
                        if (commandLine.commands.Count > checkStatement.commands.Count + 2)
                            throw new Exception("Missing statement (code container)");
                        else
                            throw new Exception($"Unexpected {commandLine.commands[checkStatement.commands.Count + 1].commandType} in while loop.");
                    if (commandLine.commands[checkStatement.commands.Count + 1].commandType != Command.CommandTypes.CodeContainer)
                        throw new Exception("Invalid stuff in while loop I hate writeing these messages pls kill me");
                    List<Command> code = commandLine.commands[checkStatement.commands.Count + 1].codeContainerCommands ?? throw new Exception("Internal: Code container was not converted to a command list.");
                    
                    while (GetVarOfCommandLine(checkStatement, accessableVars).GetBoolValue)
                    {
                        returnValue = InterpretMain.InterpretNormalMode(code, accessableVars);
                        if (returnValue.varDef.varType == VarDef.EvarType.@return) return returnValue;
                    }
                    

                    return new();
                case "if":
                    if (commandLine.commands.Count < 3) throw new Exception("Invalid if statement syntax. Example for right syntax:\nif <bool> <code container>;\nor:\nif <bool> <code container> else <code container>;");
                    if (commandLine.commands[2].commandType != Command.CommandTypes.CodeContainer)
                        throw new Exception("Invalid if statement syntax. Example for right syntax:\nif <bool> <code container>;\nor:\nif <bool> <code container> else <code container>;");

                    if (commandLine.commands.Count == 3)
                    {
                        if (GetVarOfCommandLine(new(new List<Command> { commandLine.commands[1] }, -1), accessableVars).GetBoolValue)
                        {
                            returnValue = InterpretMain.InterpretNormalMode(commandLine.commands[2].codeContainerCommands ?? throw new Exception("Internal: Code container was not converted to a command list."), accessableVars);
                            if (returnValue.varDef.varType == VarDef.EvarType.@return) return returnValue;
                        }

                    }
                    else if (commandLine.commands.Count == 5)
                    {
                        if (commandLine.commands[3].commandType != Command.CommandTypes.Statement || commandLine.commands[3].commandText != "else")
                            throw new Exception("Invalid if statement syntax. Example for right syntax:\nif <bool> <code container>;\nor:\nif <bool> <code container> else <code container>;");
                        if (commandLine.commands[4].commandType != Command.CommandTypes.CodeContainer)
                            throw new Exception("Invalid if statement syntax. Example for right syntax:\nif <bool> <code container>;\nor:\nif <bool> <code container> else <code container>;");
                        if (GetVarOfCommandLine(new(new List<Command> { commandLine.commands[1] }, -1 ), accessableVars).GetBoolValue)
                        {
                            returnValue = InterpretMain.InterpretNormalMode(commandLine.commands[2].codeContainerCommands ?? throw new Exception("Internal: Code container was not converted to a command list."), accessableVars);
                            if (returnValue.varDef.varType == VarDef.EvarType.@return) return returnValue;
                        }
                        else
                        {
                            returnValue = InterpretMain.InterpretNormalMode(commandLine.commands[4].codeContainerCommands ?? throw new Exception("Internal: Code container was not converted to a command list."), accessableVars);
                            if (returnValue.varDef.varType == VarDef.EvarType.@return) return returnValue;
                        }


                    }
                    return new();
                case "helpm":
                    if (commandLine.commands.Count != 2) throw new Exception("Invalid helpm statement syntax. Example for right syntax:\nhelpm <method call>;");
                    if (commandLine.commands[1].commandType != Command.CommandTypes.MethodCall) throw new Exception("Invalid helpm statement syntax. Example for right syntax:\nhelpm <method call>;");
                    MethodCall helpCall = commandLine.commands[1].methodCall ?? throw new Exception("Internal: Method call was not converted to a method call.");
                    ErrorHelp.ListMethodArguments(helpCall.callMethod);
                    return new();
                case "listm":
                    if (commandLine.commands.Count != 2) throw new Exception("Invalid listm statement syntax. Example for right syntax:\nhelpm <string location>;");
                    if (commandLine.commands[1].commandType != Command.CommandTypes.String) throw new Exception("Invalid listm statement syntax. Example for right syntax:\nhelpm <string location>;");
                    ErrorHelp.ListLocation(commandLine.commands[1].commandText);
                    return new();
                case "rootm":
                    if (commandLine.commands.Count != 1) throw new Exception("Invalid rootm statement syntax. Example for right syntax:\nhelpm; (It's that simple)");
                    Console.WriteLine("All registered namespaces are:");
                    ErrorHelp.ListNamespaces(Global.Namespaces);
                    return new();



                default:
                    throw new Exception($"Unknown statement: \"{commandLine.commands[0].commandText}\"");
            }
        }
        public static Var GetVarOfCommandLine(CommandLine commandLine, VarDef.EvarType expectedType, List<Var> accessableVars)
        {

            switch (commandLine.commands[0].commandType)//Check var type thats provided
            {
                case Command.CommandTypes.MethodCall:
                    MethodCall methodCall = commandLine.commands[0].methodCall ?? throw new Exception("Internal: Method call was not converted to a method call.");
                    if (commandLine.commands.Count != 1) //There shouldnt be anything after a method call
                        throw new Exception($"Unexpected {commandLine.commands[1].commandType} after Methodcall.");
                    if (methodCall.callMethod.returnType != expectedType) //Find out if Method returns desired type
                        throw new Exception($"The method {methodCall.callMethod.methodLocation} does not return the expected {expectedType} type.");
                    return methodCall.DoMethodCall(accessableVars);

                case Command.CommandTypes.NumCalculation:
                    if (commandLine.commands.Count != 1) //There shouldnt be anything after a calculation
                        throw new Exception($"Unexpected {commandLine.commands[1].commandType} after Num calculation.");
                    Var numCalcRet = NumCalculation.DoNumCalculation(commandLine.commands[0], accessableVars);
                    if (numCalcRet.varDef.varType != expectedType) throw new Exception($"The num calculation does not return the expected {expectedType} type.");
                    return numCalcRet;

                case Command.CommandTypes.Statement:
                    Var returnStatementCall = ReturnStatement(commandLine.commands, accessableVars);
                    if (returnStatementCall.varDef.varType != expectedType)
                        throw new Exception($"The ReturnStatement \"{commandLine.commands[0].commandText}\" does not return the expected {expectedType} value at all or in the given configuation.");
                    return returnStatementCall;

                case Command.CommandTypes.String:
                    if (expectedType != VarDef.EvarType.@string) throw new Exception($"String is not the expected {expectedType} type.");
                    if (commandLine.commands.Count != 1) //There shouldnt be anything after a string
                        throw new Exception($"Unexpected {commandLine.commands[1].commandType} after Num calculation.");
                    return new Var(new(VarDef.EvarType.@string, "", false), true, commandLine.commands[0].commandText);

                default:
                    throw new Exception($"Unexpected type ({commandLine.commands[0].commandType})");
            }
        }
        public static Var GetVarOfCommandLine(CommandLine commandLine, List<Var> accessableVars)
        {

            switch (commandLine.commands[0].commandType)//Check var type thats provided
            {
                case Command.CommandTypes.MethodCall:
                    MethodCall methodCall = commandLine.commands[0].methodCall ?? throw new Exception("Internal: Method call was not converted to a method call.");
                    if (commandLine.commands.Count != 1) //There shouldnt be anything after a method call
                        throw new Exception($"Unexpected {commandLine.commands[1].commandType} after Methodcall.");

                    return methodCall.DoMethodCall(accessableVars);

                case Command.CommandTypes.NumCalculation:
                    if (commandLine.commands.Count != 1) //There shouldnt be anything after a calculation
                        throw new Exception($"Unexpected {commandLine.commands[1].commandType} after Num calculation.");
                    Var numCalcRet = NumCalculation.DoNumCalculation(commandLine.commands[0], accessableVars);

                    return numCalcRet;

                case Command.CommandTypes.Statement:
                    Var returnStatementCall = ReturnStatement(commandLine.commands, accessableVars);
                    return returnStatementCall;

                case Command.CommandTypes.String:
                    if (commandLine.commands.Count != 1) //There shouldnt be anything after a string
                        throw new Exception($"Unexpected {commandLine.commands[1].commandType} after Num calculation.");
                    return new Var(new(VarDef.EvarType.@string, "", false), true, commandLine.commands[0].commandText);

                default:
                    throw new Exception($"Unexpected type ({commandLine.commands[0].commandType})");
            }
        }


        private static void StaticStatementSet(CommandLine commandLine, List<Var> accessableVars)
        {
            if (commandLine.commands.Count < 3) throw new Exception("Invalid syntax for set command\nExpected: set <variable(Statement)> <value>;");
            if (commandLine.commands[1].commandType != Command.CommandTypes.Statement) throw new Exception("Invalid syntax for set command\nExpected: set <variable(Statement)> <value>;");

            Var? correctVar = null;
            commandLine.commands[1].commandText = commandLine.commands[1].commandText.ToLower();
            foreach (Var var in accessableVars) //Search for variable
            {
                if (var.varDef.varName == commandLine.commands[1].commandText)
                {
                    correctVar = var;
                    break;
                }
            }
            if (correctVar == null) throw new Exception($"The variable {commandLine.commands[1].commandText} cant be found.");

            switch (correctVar.varDef.varType) //Check var type thats needed
            {
                case VarDef.EvarType.num or VarDef.EvarType.@bool:
                    correctVar.numValue = GetVarOfCommandLine(new CommandLine(commandLine.commands.GetRange(2, commandLine.commands.Count - 2), commandLine.lineIDX), correctVar.varDef.varType, accessableVars).numValue;
                    break;
                case VarDef.EvarType.@string:
                    correctVar.stringValue = GetVarOfCommandLine(new CommandLine(commandLine.commands.GetRange(2, commandLine.commands.Count - 2), commandLine.lineIDX), correctVar.varDef.varType, accessableVars).stringValue;
                    break;
                default: throw new Exception("Internal: Unimplemented VarType");
            }
        }
        public static Var ReturnStatement(List<Command> commands, List<Var> accessableVars)
        {
            if (commands[0].commandType != Command.CommandTypes.Statement)
                throw new Exception("Internal: ReturnStatements must start with a Statement");

            switch (commands[0].commandText)
            {
                case "true":
                    if (commands.Count != 1) throw new Exception($"Unexpected {commands[1].commandType}");
                    return new Var(new(VarDef.EvarType.@bool, ""), true, true);
                case "false":
                    if (commands.Count != 1) throw new Exception($"Unexpected {commands[1].commandType}");
                    return new Var(new(VarDef.EvarType.@bool, ""), true, false);
                case "new":
                    if (commands[1].commandType != Command.CommandTypes.Statement)
                        throw new Exception($"Unexpected {commands[1].commandType} at argument 1 of new statement\nA statement would be expected at this point.");
                    throw new NotImplementedException("Internal: New statement is not fully implemented yet");
                case "void":
                    if (commands.Count != 1) throw new Exception($"Unexpected {commands[1].commandType}");
                    return new();

                case "nl":
                    return new Var(new(VarDef.EvarType.@string, ""), true, "\n");
                case "lineChar":
                    return new Var(new(VarDef.EvarType.@string, ""), true, "Ⅼ");

                case "if":
                    //Check if if statement usage is correct
                    Var? returnVar = null;
                    if (commands.Count != 5 || commands[2].commandType != Command.CommandTypes.CodeContainer || commands[3].commandType != Command.CommandTypes.Statement || commands[3].commandText.ToLower() != "else" || commands[4].commandType != Command.CommandTypes.CodeContainer)
                        throw new Exception("Invalid return-type if statement; Correct usage:\nif <code container> else <code container>");
                    if (GetVarOfCommandLine(new(new List<Command> { commands[1] }, -1), accessableVars).GetBoolValue)
                        returnVar = InterpretMain.InterpretNormalMode(commands[2].codeContainerCommands ?? throw new Exception("Internal: Code container was not converted to a command list."), accessableVars);
                    else
                        returnVar = InterpretMain.InterpretNormalMode(commands[4].codeContainerCommands ?? throw new Exception("Internal: Code container was not converted to a command list."), accessableVars);
                    if (returnVar.varDef.varType == VarDef.EvarType.@return)
                        return returnVar.returnStatementValue ?? throw new Exception("Internal: return-var var is null");
                    else
                        throw new Exception("The return-type if statemtent didn't return anything");


                default:
                    // Is probably var
                    
                    if (commands.Count != 1) throw new Exception("Unexpected syntax after varname.");
                    commands[0].commandText = commands[0].commandText.ToLower();
                    foreach (Var var in accessableVars)
                        if (var.varDef.varName == commands[0].commandText)
                            return var;
                    //Var not found


                    throw new Exception($"Unknown return statement \"{commands[0].commandText}\"");
            }

        }

    }
}
