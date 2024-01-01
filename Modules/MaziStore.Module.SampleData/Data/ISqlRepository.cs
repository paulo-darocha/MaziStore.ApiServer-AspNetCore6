using System.Collections.Generic;

namespace MaziStore.Module.SampleData.Data
{
   public interface ISqlRepository
   {
      void RunCommand(string command);

      void RunCommands(IEnumerable<string> commands);

      IEnumerable<string> ParseCommand(IEnumerable<string> lines);

      string GetDbConnectionType();
   }
}
