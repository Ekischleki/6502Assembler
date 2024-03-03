using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASI.LangCoreHandleInterface
{
    public interface IReturnStatementHandler
    {
        void HandleReturnStatement(List<Command> inputStatement, AccessableObjects accessableObjects);

    }
}
