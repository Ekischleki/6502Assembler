using System.Diagnostics;
using TASI.InitialisationObjects;
using TASI.InternalLangCoreHandle;
using TASI.LangCoreHandleInterface;


namespace TASI
{


    public class GlobalProjectShared
    {
        public bool debugErrorSkip = true;
       
        public List<string> allLoadedFiles = new(); //It is important, that allLoadedFiles and namespaces corrospond

        public bool debugMode = false;
        public string mainFilePath;
        public List<Task> processFiles = new();
        public object processFileLock = new();
        public object iportFileLock = new();
        public List<FileStream> allFileStreams = new();
        public Random randomGenerator = new();
       
        public Dictionary<string, Statement> allStatements = new();
        public Dictionary<string, ReturnStatement> allReturnStatements = new();
    }

    public class GlobalContext
    {
        public int currentLine;
        public string currentFile;
    }

    public class Global
    {
        public Dictionary<string, Statement> AllNormalStatements
        {
            get
            {
                return globalProjectShared.allStatements;
            }
            set
            {
                globalProjectShared.allStatements = value;
            }
        }
        public Dictionary<string, ReturnStatement> AllNormalReturnStatements
        {
            get
            {
                return globalProjectShared.allReturnStatements;
            }
            set
            {
                globalProjectShared.allReturnStatements = value;
            }
        }
        

        public string CurrentFile
        {
            get
            {
                return globalContext.currentFile;
            }
            set
            {
                globalContext.currentFile = value;
            }
        }

        public bool DebugErrorSkip
        {
            get
            {
                return globalProjectShared.debugErrorSkip;
            }
            set
            {
                globalProjectShared.debugErrorSkip = value;
            }
        }
        
        public List<string> AllLoadedFiles
        {
            get
            {
                return globalProjectShared.allLoadedFiles;
            }
            set
            {
                globalProjectShared.allLoadedFiles = value;
            }
        }
        public List<FileStream> AllFileStreams
        {
            get
            {
                return globalProjectShared.allFileStreams;
            }
            set
            {
                globalProjectShared.allFileStreams = value;
            }
        }
        
        public Random RandomGenerator
        {
            get
            {
                return globalProjectShared.randomGenerator;
            }
            set
            {
                globalProjectShared.randomGenerator = value;
            }
        }



        public bool DebugMode
        {
            get
            {
                return globalProjectShared.debugMode;
            }
            set
            {
                globalProjectShared.debugMode = value;
            }
        }
        public int CurrentLine
        {
            get
            {
                return globalContext.currentLine;
            }
            set
            {
                globalContext.currentLine = value;
            }
        }
        public string MainFilePath
        {
            get
            {
                return globalProjectShared.mainFilePath;
            }
            set
            {
                globalProjectShared.mainFilePath = value;
            }
        }
        public List<Task> ProcessFiles
        {
            get
            {
                return globalProjectShared.processFiles;
            }
            set
            {
                globalProjectShared.processFiles = value;
            }
        }
        public object ProcessFileLock
        {
            get
            {
                return globalProjectShared.processFileLock;
            }
            set
            {
                globalProjectShared.processFileLock = value;
            }
        }
        public object IportFileLock
        {
            get
            {
                return globalProjectShared.iportFileLock;
            }
            set
            {
                globalProjectShared.iportFileLock = value;
            }
        }

        public GlobalContext globalContext;
        public GlobalProjectShared globalProjectShared;


        public Global CreateNewContext(string file)
        {
            GlobalContext globalContext = new()
            {
                currentFile = file,
                currentLine = -1
            };
            return new(globalContext, globalProjectShared);
        }


        public Global(GlobalContext globalContext, GlobalProjectShared globalProjectShared)
        {
            this.globalContext = globalContext;
            this.globalProjectShared = globalProjectShared;
        }
        public Global(string currentFile = "")
        {
            globalContext = new GlobalContext();
            globalContext.currentFile = currentFile;
            globalProjectShared = new GlobalProjectShared();

            //Normal statements
            InternalStatementHandler internalStatementHandler = new();
            AllNormalStatements.Add("loop", new("loop", new()
            {
                new() {}
            }, internalStatementHandler));
            AllNormalStatements.Add("return", new("return", new()
            {
                new() {new(StatementInputType.StatementInput.ValueReturner, "valueToReturn") },
                new()
            }, internalStatementHandler));
            AllNormalStatements.Add("set", new("set", new()
            {
                new() {new(StatementInputType.StatementInput.Statement, "variableName"), new(StatementInputType.StatementInput.ValueReturner, "newVariableValue") },
            }, internalStatementHandler));
            AllNormalStatements.Add("while", new("while", new()
            {
                new() {new(StatementInputType.StatementInput.ValueReturner, "loopWhileTrue"), new(StatementInputType.StatementInput.CodeContainer, "codeToLoop") },
            }, internalStatementHandler));
            AllNormalStatements.Add("if", new("if", new()
            {
                new() {new(StatementInputType.StatementInput.ValueReturner, "ifCheck"), new(StatementInputType.StatementInput.CodeContainer, "doIfTrue")},
                new() {new(StatementInputType.StatementInput.ValueReturner, "ifCheck"), new(StatementInputType.StatementInput.CodeContainer, "doIfTrue"), new("else", "else"), new(StatementInputType.StatementInput.CodeContainer, "doIfFalse") }
            }, internalStatementHandler));
            AllNormalStatements.Add("helpm", new("helpm", new()
            {
                new() {new(StatementInputType.StatementInput.FunctionCall, "functionToCheck (The function call doesn't have to be a valid call)") },
            }, internalStatementHandler));
            AllNormalStatements.Add("listm", new("helpm", new()
            {
                new() {new(StatementInputType.StatementInput.String, "location") },
            }, internalStatementHandler));
            AllNormalStatements.Add("rootm", new("rootm", new()
            {
                new() {},
            }, internalStatementHandler));


            AllNormalStatements.Add("link", new("link", new()
            {
                new() {new(StatementInputType.StatementInput.Statement, "variableToLink"), new(StatementInputType.StatementInput.Statement, "variableToLinkTo") },
            }, internalStatementHandler));
            AllNormalStatements.Add("unlink", new("unlink", new()
            {
                new() {new(StatementInputType.StatementInput.Statement, "variableToReset (After an unlink the variable won't return to its value before the link)") },
            }, internalStatementHandler));
            AllNormalStatements.Add("promise", new("promise", new()
            {
                new() {new(StatementInputType.StatementInput.Statement, "variableName"), new(StatementInputType.StatementInput.CodeContainer, "initPromiseInPromiseContext"), new(StatementInputType.StatementInput.CodeContainer, "promiseCode") },
                new() {new(StatementInputType.StatementInput.Statement, "variableName"), new(StatementInputType.StatementInput.CodeContainer, "promiseCode") },
            }, internalStatementHandler));
            AllNormalStatements.Add("unpromise", new("unpromise", new()
            {
                new() {new(StatementInputType.StatementInput.Statement, "variableName")},
            }, internalStatementHandler));
            AllNormalStatements.Add("makevar", new("makeVar", new()
            {
                new() {new(StatementInputType.StatementInput.Statement, "variableType"), new(StatementInputType.StatementInput.Statement, "variableName")},
                new() {new(StatementInputType.StatementInput.Statement, "variableType"), new(StatementInputType.StatementInput.Statement, "variableName"), new(StatementInputType.StatementInput.ValueReturner, "initialisationValue")},
            }, internalStatementHandler));
            AllNormalStatements.Add("makeconst", new("makeConst", new()
            {
                new() {new(StatementInputType.StatementInput.Statement, "constType"), new(StatementInputType.StatementInput.Statement, "constName"), new(StatementInputType.StatementInput.ValueReturner, "initialisationValue")},
            }, internalStatementHandler));

            //Return statements
            InternalReturnStatementHandler internalReturnStatementHandler = new InternalReturnStatementHandler();
            AllNormalReturnStatements.Add("true", new("true", new()
            {
                new() {},
            }, internalReturnStatementHandler));
            AllNormalReturnStatements.Add("false", new("false", new()
            {
                new() {},
            }, internalReturnStatementHandler));
            AllNormalReturnStatements.Add("void", new("void", new()
            {
                new() {},
            }, internalReturnStatementHandler));
            AllNormalReturnStatements.Add("if", new("if", new()
            {
                new() {new(StatementInputType.StatementInput.ValueReturner, "ifCheck"), new(StatementInputType.StatementInput.CodeContainer, "doIfTrue"), new("else", "else"), new(StatementInputType.StatementInput.CodeContainer, "doIfFalse") }

            }, internalReturnStatementHandler));
            AllNormalReturnStatements.Add("do", new("do", new()
            {
                new() {new(StatementInputType.StatementInput.CodeContainer, "doCode")},
            }, internalReturnStatementHandler));
            AllNormalReturnStatements.Add("linkable", new("linkable", new()
            {
                new() {new(StatementInputType.StatementInput.Statement, "variableName")},
            }, internalReturnStatementHandler));

        }
    }
}
