using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Data.Presto.Utils
{
    internal static class ParameterUtils
    {
        public static string DeparameterizeStatement(PrestoCommand command)
        {
            var currentCommand = command.CommandText;

            for (int i = 0; i < command.Parameters.Count; i++)
            {
                var parameter = command.Parameters[i];
                currentCommand = currentCommand.Replace(parameter.ParameterName, parameter.GenerateSqlValue());
            }

            return currentCommand;
        }
    }
}
