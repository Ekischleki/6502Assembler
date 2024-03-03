using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TASI.InitialisationObjects;

namespace TASI.InternalLangCoreHandle
{
    public static class InterpretationHelp
    {
        public static void HandleReturnStatement(List<Command> returnStatementCommands, AccessableObjects accessableObjects)
        {
            if (!accessableObjects.global.AllNormalReturnStatements.TryGetValue(returnStatementCommands[0].commandText.ToLower(), out ReturnStatement returnStatement))
                UnknownStatementHandler.HandleUnknownReturnStatement(returnStatementCommands, accessableObjects);
            if (!returnStatement.IsValidInput(returnStatementCommands))
                throw new CodeSyntaxException($"Incorrect usage of statement. {returnStatement.CorrectUsage}");
            returnStatement.returnStatementHandler.HandleReturnStatement(returnStatementCommands, accessableObjects);
        }
        public static void HandleStatement(List<Command> returnStatementCommands, AccessableObjects accessableObjects)
        {
            if (!accessableObjects.global.AllNormalStatements.TryGetValue(returnStatementCommands[0].commandText.ToLower(), out Statement statement))
                throw new CodeSyntaxException($"Unknown statement \"{returnStatementCommands[0].commandText}\"");
            if (!statement.IsValidInput(returnStatementCommands))
                throw new CodeSyntaxException($"Incorrect usage of statement. {statement.CorrectUsage}");
            statement.statementHandler.HandleStatement(returnStatementCommands, accessableObjects);
        }

        
        
    }
}
