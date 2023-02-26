﻿namespace Text_adventure_Script_Interpreter
{
    public class NamespaceInfo
    {
        public enum NamespaceIntend
        {
            Main, Supervisor, Generic, Internal
        }
        public string name;
        public List<Method> namespaceMethods = new List<Method>();
        public List<VarDef.evarType> namespaceVars = new List<VarDef.evarType>();
        public List<Var> publicNamespaceVars = new List<Var>();
        public NamespaceIntend namespaceIntend;


        public NamespaceInfo(NamespaceIntend namespaceIntend, string name)
        {
            Text_adventure_Script_Interpreter_Main.interpretInitLog.Log($"Creating new Namespace. Intend: {namespaceIntend}; Name: {name}");
            this.namespaceIntend = namespaceIntend;
            this.name = name;
        }

    }


    public class Var
    {
        public VarDef varDef;

        public bool tempVar;
        public double? numValue;
        public string? stringValue;
        public double[] numArrayValue;
        public string[] stringArrayValue;


        public object objectValue
        {
            get
            {
                if (varDef.varType == VarDef.evarType.Num || varDef.varType == VarDef.evarType.Bool)
                    return numValue;
                else
                    return stringValue;
            }
            set
            {
                if (varDef.varType == VarDef.evarType.Num || varDef.varType == VarDef.evarType.Bool)
                    numValue = (double)value;
                else
                    stringValue = (string)value;
            }
        }


        public Var(VarDef varDef, bool tempVar, object? value)
        {
            this.varDef = varDef;
            this.tempVar = tempVar;
            switch (varDef.varType)
            {
                case VarDef.evarType.Num:
                    if (varDef.isArray == true)
                        numArrayValue = (double[])value;
                    else
                        numValue = (double)value;
                    break;
                case VarDef.evarType.Bool: //Bool values are just num values *Shock*
                    if (varDef.isArray == true)
                        throw new Exception("Sorry, but there are no bool arrays rn. Gonna add them in later. I promise!");
                    if ((bool)value)
                        numValue = 1;
                    else
                        numValue = 0;
                    break;
                case VarDef.evarType.String:
                    if (varDef.isArray == true)
                        stringArrayValue = (string[])value;
                    else
                        stringValue = (string)value;
                    break;
                case VarDef.evarType.Void:
                    throw new Exception("Can't create a variable with the \"Void\" type. E.U.0008");
                default:
                    throw new Exception("Unknown variable type at NamespaceInfo.Var(Switch(vartype). E.Internal.0001");
            }
        }
        public Var()
        {
            tempVar = true;
            varDef = new(VarDef.evarType.Void, "");


        }


    }

    public class VarDef
    {
        public VarDef(evarType evarType, string varName)
        {
            varType = evarType;
            this.varName = varName;
            this.isArray = false;
        }
        public VarDef(evarType evarType, string varName, bool isArray)
        {
            varType = evarType;
            this.varName = varName;
            if (evarType == evarType.Void)
                throw new Exception("Can't create an array with type void. I mean what you wanna put in there lol?. E.U 0009");
            this.isArray = isArray;
        }
        public enum evarType
        {
            @Num, @String, @Bool, @Void
        }
        public evarType varType;
        public string varName;
        public bool isArray;
    }


}
