using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASI.LangCoreHandleInterface
{
    public interface IStatementHandler
    {
        void HandleStatement(List<Command> inputStatement, AccessableObjects accessableObjects);
    }
}
