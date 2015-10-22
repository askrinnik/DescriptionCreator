using System.IO;
using System.Linq;

namespace DescriptionCreator
{
  class Program
  {
    static void Main(string[] args)
    {
      ProcessFile(args[0]);
    }

    private static void ProcessFile(string srcFileName)
    {
      var lines = File.ReadAllLines(srcFileName);

      var sqlLines = lines.Select(GenerateSqlCommand).ToArray();
      File.WriteAllLines(srcFileName + ".MsSql.sql", sqlLines);

      var oraLines = lines.Select(GenerateOraCommand).ToArray();
      File.WriteAllLines(srcFileName + ".Oracle.sql", oraLines);
    }

    private static string GenerateSqlCommand(string line)
    {
      const string tableCommand = "EXEC sp_addextendedproperty 'MS_Description', '{0}', 'Schema', dbo, 'table', {1};";
      const string columnCommand = "EXEC sp_addextendedproperty 'MS_Description', '{0}', 'Schema', dbo, 'table', {1}, 'column', {2};";
      return GenerateCommand(line, tableCommand, columnCommand);
    }
    private static string GenerateOraCommand(string line)
    {
      const string tableCommand = "COMMENT ON TABLE {1} IS '{0}';";
      const string columnCommand = "COMMENT ON COLUMN {1}.{2}	IS '{0}';";
      return GenerateCommand(line, tableCommand, columnCommand);
    }

    private static string GenerateCommand(string line, string tableCommand, string columnCommand)
    {
      const int tableNameIndex = 0;
      const int columnNameIndex = 1;
      const int descriptionIndex = 2;

      var cells = line.Split('\t');
      return cells[columnNameIndex] == "NULL"
        ? string.Format(tableCommand, cells[descriptionIndex], cells[tableNameIndex])
        : string.Format(columnCommand, cells[descriptionIndex], cells[tableNameIndex], cells[columnNameIndex]);
    }
  }
}
