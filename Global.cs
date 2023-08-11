﻿namespace TASI
{


    public class GlobalProjectShared
    {
        public bool debugErrorSkip = true;
        public List<NamespaceInfo> namespaces = new List<NamespaceInfo>();
        public List<string> allLoadedFiles = new(); //It is important, that allLoadedFiles and namespaces corrospond
        public List<Function> allFunctions = new List<Function>();
        public bool debugMode = false;
        public List<FunctionCall> allFunctionCalls = new();
        public string mainFilePath;
        public List<Task> processFiles = new();
        public object processFileLock = new();
        public object iportFileLock = new();
        public List<FileStream> allFileStreams = new();
        public Random randomGenerator = new();
    }

    public class GlobalContext
    {
        public int currentLine;
        public string currentFile;
    }

    public class Global
    {
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
        public List<NamespaceInfo> Namespaces
        {
            get
            {
                return globalProjectShared.namespaces;
            }
            set
            {
                globalProjectShared.namespaces = value;
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
        public List<Function> AllFunctions
        {
            get
            {
                return globalProjectShared.allFunctions;
            }
            set
            {
                globalProjectShared.allFunctions = value;
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
        public List<FunctionCall> AllFunctionCalls
        {
            get
            {
                return globalProjectShared.allFunctionCalls;
            }
            set
            {
                globalProjectShared.allFunctionCalls = value;
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
            InternalFunctionHandle mainInternalFunctionHandle = new();
            globalContext = new GlobalContext();
            globalContext.currentFile = currentFile;
            globalProjectShared = new GlobalProjectShared();


            Namespaces = new();
            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "Test", true));
            AllLoadedFiles.Add("*internal");
            new Function("HelloWorld", VarConstruct.VarType.@void, Namespaces[0], new List<List<VarConstruct>> {
                new List<VarConstruct> { new(VarConstruct.VarType.@bool, "display"), new(VarConstruct.VarType.@string, "text")}
            }, new(), this, mainInternalFunctionHandle);

            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "Console", true));
            AllLoadedFiles.Add("*internal");
            new Function("WriteLine", VarConstruct.VarType.@void, Namespaces[1], new List<List<VarConstruct>> {
                new List<VarConstruct> { new(VarConstruct.VarType.@string, "text")},
                new List<VarConstruct> { new(VarConstruct.VarType.num, "num")},
                new List<VarConstruct> { new(VarConstruct.VarType.@bool, "bool")},
                new List<VarConstruct> { new(VarConstruct.VarType.@int, "int")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("Write", VarConstruct.VarType.@void, Namespaces[1], new List<List<VarConstruct>> {
                new List<VarConstruct> { new(VarConstruct.VarType.@string, "text")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("ReadLine", VarConstruct.VarType.@string, Namespaces[1], new List<List<VarConstruct>> {
                new List<VarConstruct> { new(VarConstruct.VarType.@bool, "showTextWhenTyping")},
                new List<VarConstruct> {}
            }, new(), this, mainInternalFunctionHandle);
            new Function("Clear", VarConstruct.VarType.@void, Namespaces[1], new List<List<VarConstruct>> {
                new List<VarConstruct> {}
            }, new(), this, mainInternalFunctionHandle);

            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "Program", true));
            AllLoadedFiles.Add("*internal");
            new Function("Pause", VarConstruct.VarType.@void, Namespaces[2], new List<List<VarConstruct>> {
                new List<VarConstruct> {},
                new List<VarConstruct> {new(VarConstruct.VarType.@bool, "showPausedMessage")}
            }, new(), this, mainInternalFunctionHandle);

            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "Inf", true));
            AllLoadedFiles.Add("*internal");
            new Function("DefVar", VarConstruct.VarType.@void, Namespaces[3], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "VarType"), new(VarConstruct.VarType.@string, "VarName")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("MakeConst", VarConstruct.VarType.@void, Namespaces[3], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "VarName")}
            }, new(), this, mainInternalFunctionHandle);
            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "Convert", true));
            AllLoadedFiles.Add("*internal");
            new Function("ToNum", VarConstruct.VarType.num, Namespaces[4], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "ConvertFrom"), new(VarConstruct.VarType.@bool, "errorOnParseFail")}
            }, new(), this, mainInternalFunctionHandle);

            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "Filesystem", true));
            AllLoadedFiles.Add("*internal");
            new Function("Open", VarConstruct.VarType.@int, Namespaces[5], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "FilePath"), new(VarConstruct.VarType.@string, "Mode") }
            }, new(), this, mainInternalFunctionHandle);
            new Function("Close", VarConstruct.VarType.@void, Namespaces[5], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@int, "StreamIndex")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("Delete", VarConstruct.VarType.@void, Namespaces[5], new List<List<VarConstruct>>
            {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "FilePath")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("Create", VarConstruct.VarType.@int, Namespaces[5], new List<List<VarConstruct>>
            {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "FilePath")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("Exists", VarConstruct.VarType.@bool, Namespaces[5], new List<List<VarConstruct>>
            {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "FilePath")}
            }, new(), this, mainInternalFunctionHandle);
            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "Filestream", true));
            AllLoadedFiles.Add("*internal");
            new Function("ReadLine", VarConstruct.VarType.@string, Namespaces[6], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@int, "StreamIndex")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("Read", VarConstruct.VarType.@int, Namespaces[6], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@int, "StreamIndex")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("Flush", VarConstruct.VarType.@void, Namespaces[6], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@int, "StreamIndex")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("Write", VarConstruct.VarType.@void, Namespaces[6], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@int, "StreamIndex"), new(VarConstruct.VarType.@int, "Char")}
            }, new(), this, mainInternalFunctionHandle);
            new Function("WriteLine", VarConstruct.VarType.@void, Namespaces[6], new List<List<VarConstruct>> {
                new List<VarConstruct> { new(VarConstruct.VarType.@int, "StreamIndex"), new(VarConstruct.VarType.@string, "text")},
                new List<VarConstruct> { new(VarConstruct.VarType.@int, "StreamIndex"), new(VarConstruct.VarType.num, "num")},
                new List<VarConstruct> { new(VarConstruct.VarType.@int, "StreamIndex"), new(VarConstruct.VarType.@bool, "bool")},
                new List<VarConstruct> { new(VarConstruct.VarType.@int, "StreamIndex"), new(VarConstruct.VarType.@int, "int")}
            }, new(), this, mainInternalFunctionHandle);
            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "Random", true));
            AllLoadedFiles.Add("*internal");
            new Function("Next", VarConstruct.VarType.@int, Namespaces[7], new List<List<VarConstruct>> {
                new List<VarConstruct> {},
                new List<VarConstruct> {new(VarConstruct.VarType.@int, "min")},
                new List<VarConstruct> {new(VarConstruct.VarType.@int, "min"), new(VarConstruct.VarType.@int, "max")}
            }, new(), this, mainInternalFunctionHandle) ;
            new Function("NextNum", VarConstruct.VarType.num, Namespaces[7], new List<List<VarConstruct>> {
                new List<VarConstruct> {},
            }, new(), this, mainInternalFunctionHandle);
            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "Shell", false));
            AllLoadedFiles.Add("unsafe.shell");
            new Function("Execute", VarConstruct.VarType.@string, Namespaces[8], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "cmd")},
            }, new(), this, mainInternalFunctionHandle);
            new Function("Run", VarConstruct.VarType.@void, Namespaces[8], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "cmd")},
            }, new(), this, mainInternalFunctionHandle);
            Namespaces.Add(new NamespaceInfo(NamespaceInfo.NamespaceIntend.@internal, "String", true));
            AllLoadedFiles.Add("*internal");
            new Function("Replace", VarConstruct.VarType.@string, Namespaces[9], new List<List<VarConstruct>> {
                new List<VarConstruct> {new(VarConstruct.VarType.@string, "str"), new(VarConstruct.VarType.@string, "org"), new(VarConstruct.VarType.@string, "new")},
            }, new(), this, mainInternalFunctionHandle);
            
        }
    }
}
