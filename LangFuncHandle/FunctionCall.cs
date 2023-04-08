﻿using DataTypeStore;

namespace TASI
{
    public class FunctionCall
    {
        public Function? callFunction;
        public List<Var> inputVars;
        public List<CommandLine> argumentCommands;
        private string functionName;

        public FunctionCall(Region region)
        {
            callFunction = FindFunctionByPath(region.FindDirectValue("mL").value, Global.Namespaces, true, null);
            argumentCommands = new List<CommandLine>();
            foreach (Region region1 in region.FindSubregionWithNameArray("CmdL"))
            {
                argumentCommands.Add(new(region));
            }
        }
        public Region Region
        {
            get
            {
                Region result = new("MC", new List<Region>(), new());
                result.directValues.Add(new("mL", callFunction.functionLocation, false));
                foreach (var arg in argumentCommands)
                    result.SubRegions.Add(arg.Region);
                return result;

            }
        }
        public FunctionCall(Function callFunction, List<Var> inputVars)
        {
            this.callFunction = callFunction;
            this.inputVars = new List<Var>(inputVars);
        }

        public FunctionCall(Command command)
        {
            functionName = "";
            List<string> functionArguments = new();
            string currentArgument = "";
            bool doingName = true;
            int squareDeph = 0;
            int braceDeph = 0;
            bool inString = false;
            bool lastBackslash = false;


            //split function name from arguments and arguments with comma
            foreach (char c in command.commandText)
            {
                if (doingName)
                {
                    if (c == ':')
                    {
                        if (functionName == "")
                            throw new Exception("Function call can't have an empty function path.");
                        doingName = false;
                        continue;
                    }
                    functionName += c;
                }
                else
                {
                    if (inString)
                    {

                        if (!lastBackslash && c == '\"')
                            inString = false;
                        if (c == '\\')
                            lastBackslash = true;
                        else
                            lastBackslash = false;
                        currentArgument += c;
                        continue;
                    }


                    switch (c)
                    {
                        case '\"':
                            inString = true;
                            lastBackslash = false;
                            break;

                        case '[':
                            squareDeph += 1;
                            break;
                        case ']':
                            squareDeph -= 1;
                            if (squareDeph < 0)
                                throw new Exception("Unexpected ']'");
                            break;
                        case '{':
                            braceDeph += 1;
                            break;
                        case '}':
                            braceDeph -= 1;
                            if (braceDeph < 0)
                                throw new Exception("Unexpected '}'");
                            break;
                        case ',':
                            if (braceDeph == 0 && squareDeph == 0 && !inString) // Only check for base layer commas
                            {
                                if (currentArgument.Replace(" ", "") == "") // If argument minus Space is nothing
                                    throw new Exception("Cant have an empty argument (Check for double commas like \"[Example.Function:test,,]\")");
                                functionArguments.Add(currentArgument);
                                currentArgument = "";
                                continue;
                            }
                            break;
                    }
                    currentArgument += c;

                }
            }
            //Check if syntax are valid
            if (inString)
                throw new Exception("Expected \"");
            if (braceDeph != 0)
                throw new Exception("Expected }");
            if (squareDeph != 0)
                throw new Exception("Expected ]");

            if (currentArgument.Replace(" ", "") == "" && functionArguments.Count != 0) // If argument minus Space is nothing
                throw new Exception("Cant have an empty argument (Check for double commas like \"[Example.Function:test,,]\")");
            if (functionArguments.Count != 0 || (currentArgument.Replace(" ", "") != ""))
                functionArguments.Add(currentArgument);
            argumentCommands = new(functionArguments.Count);
            foreach (string argument in functionArguments) //Convert string arguments to commands
                argumentCommands.Add(new(StringProcess.ConvertLineToCommand(argument, command.commandLine), -1));
            Global.allFunctionCalls.Add(this);






            return;
        }

        public void SearchCallFunction(NamespaceInfo currentNamespace) //This is there, so header analysis can be done, without any errors.
        {

            callFunction = FindFunctionByPath(functionName.ToLower(), Global.Namespaces, true, currentNamespace);
            foreach (CommandLine commandLine in argumentCommands)
                foreach (Command command in commandLine.commands)
                    if (command.commandType == Command.CommandTypes.FunctionCall) command.functionCall.SearchCallFunction(currentNamespace);
        }

        public FunctionCallInputHelp? CheckIfFunctionCallHasValidArgTypesAndReturnCode(List<Var> inputVars)
        {
            bool matching;
            for (int j = 0; j < callFunction.functionArguments.Count; j++)
            {
                List<VarDef> functionInputType = callFunction.functionArguments[j];
                if (functionInputType.Count == inputVars.Count)
                {
                    matching = true;
                    for (int i = 0; i < inputVars.Count; i++)
                    {
                        if (functionInputType[i].varType != inputVars[i].varDef.varType)
                        {
                            matching = false;
                            break;
                        }
                    }
                    if (matching)
                    {
                        if (callFunction.parentNamespace.namespaceIntend == NamespaceInfo.NamespaceIntend.@internal)
                            return new(new(), new());
                        return new(callFunction.functionCode[j], functionInputType);
                    }
                }
            }
            return null;
        }


        public static NamespaceInfo? FindNamespaceByName(string name, List<NamespaceInfo> namespaces, bool exceptionAtNotFound)
        {

            foreach (NamespaceInfo namespaceInfo in namespaces)
                if (namespaceInfo.Name == name)
                    return namespaceInfo;

            if (exceptionAtNotFound)
                throw new Exception($"The namespace \"{name}\" was not found.");
            else
                return null;
        }

        public static Function? FindFunctionByPath(string name, List<NamespaceInfo> parentNamespaces, bool exceptionAtNotFound, NamespaceInfo? currentNamespace)
        {

            string[] nameSplit = name.Split('.');
            if (nameSplit.Length < 2)
                throw new Exception($"Can't find Function \"{name}\" because there is no Function in the location.");
            NamespaceInfo? parentNamespace = FindNamespaceByName(nameSplit[0], parentNamespaces, exceptionAtNotFound);
            if (parentNamespace == null)
                return null;
            List<Function> functions = parentNamespace.namespaceFuncitons;
            Function? currentFunction = null;
            for (int i = 0; i < nameSplit.Length - 1; i++)
            {
                currentFunction = null;
                foreach (Function function in functions)
                {
                    if (function.funcName == nameSplit[i + 1])
                    {
                        functions = function.subFunctions;
                        currentFunction = function;
                        break;
                    }
                }
                if (currentFunction == null)
                {
                    if (exceptionAtNotFound)
                        throw new Exception($"Could not find function \"{name}\".");
                    else
                        return null;
                }

            }

            if (currentNamespace != null)
                if (!currentNamespace.accessableNamespaces.Contains(currentFunction.parentNamespace)) throw new Exception($"The function \"{currentFunction.functionLocation}\" can't be accessed in the \"{currentNamespace.Name}\" namespace, because the \"{currentFunction.parentNamespace.Name}\" namespace wasn't imported. ");

            return currentFunction;

        }

        public Var DoFunctionCall(AccessableObjects accessableObjects)
        {
            inputVars = new();
            foreach (CommandLine commandLine in argumentCommands) // Exicute arguments
            {
                inputVars.Add(Statement.GetVarOfCommandLine(commandLine, accessableObjects));
            }

            FunctionCallInputHelp? functionCallInputHelp = CheckIfFunctionCallHasValidArgTypesAndReturnCode(inputVars);

            if (functionCallInputHelp == null)
                throw new Exception($"The function \"{callFunction.functionLocation}\" doesent support the provided input types. Use the syntax \"helpm <function call>;\"\nFor this function it would be: \"helpm [{callFunction.functionLocation}];\"");


            if (callFunction.parentNamespace.namespaceIntend == NamespaceInfo.NamespaceIntend.@internal)
            {
                Var returnValue = InternalFunctionHandle.HandleInternalFunc(callFunction.functionLocation, inputVars, accessableObjects);
                if (Global.debugMode)
                {
                    Console.WriteLine($"Did function call to {callFunction.parentNamespace.namespaceIntend}-intend {callFunction.functionLocation}.\nIt returns a {callFunction.returnType}.\nIt returned a {returnValue.varDef.varType}-type with a value of \"{returnValue.ObjectValue}\".");
                    Console.ReadKey();
                }
                return returnValue;

            }
            //return 
            List<Var> functionCallInput = new();

            for (int i = 0; i < inputVars.Count; i++)
            {
                functionCallInput.Add(new(new VarDef(functionCallInputHelp.inputVarType[i].varType, functionCallInputHelp.inputVarType[i].varName), false, this.inputVars[i].ObjectValue));
            }

            Var functionReturnValue = InterpretMain.InterpretNormalMode(functionCallInputHelp.inputCode, new(functionCallInput, callFunction.parentNamespace));



            if (functionReturnValue.varDef.varType != VarDef.EvarType.@return || functionReturnValue.returnStatementValue.varDef.varType != callFunction.returnType)
                throw new Exception($"The function \"{callFunction.functionLocation}\" didn't return the expected {callFunction.returnType}-type.");
            return functionReturnValue.returnStatementValue;

            //throw new Exception("Internal: Only internal functions are implemented");
        }

    }

    public class FunctionCallInputHelp
    {
        public List<Command> inputCode;
        public List<VarDef> inputVarType;
        public FunctionCallInputHelp(List<Command> inputCode, List<VarDef> inputVarType)
        {
            this.inputCode = inputCode;
            this.inputVarType = inputVarType;
        }
    }
}
