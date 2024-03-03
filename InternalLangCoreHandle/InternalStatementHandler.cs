
using System.Xml.Linq;
using TASI.LangCoreHandleInterface;


namespace TASI.InternalLangCoreHandle
{




    internal class InternalStatementHandler : IStatementHandler
    {

        public void HandleStatement(List<Command> commands, AccessableObjects accessableObjects)
        {
           
            if (commands[0].commandType != Command.CommandTypes.Statement)
                throw new InternalInterpreterException("Internal: StaticStatements must start with a Statement");

            switch (commands[0].commandText.ToLower())
            {
                default:
                    throw new InternalInterpreterException("Invalid statement for this statement handler");
            }
        }
    }
    
        
    public class InternalReturnStatementHandler : IReturnStatementHandler
    {
        public  void HandleReturnStatement(List<Command> commands, AccessableObjects accessableObjects)
        {
            
            if (commands[0].commandType != Command.CommandTypes.Statement)
                throw new InternalInterpreterException("Internal: ReturnStatements must start with a Statement");

            switch (commands[0].commandText.ToLower())
            {


                default:
                   throw new InternalInterpreterException("Invalid statement for this statement handler");

            }

            



        }
        
    }

    public class UnknownStatementHandler
    {
        public static void HandleUnknownReturnStatement(List<Command> commands, AccessableObjects accessableObjects)
        {

        }
    }
}
