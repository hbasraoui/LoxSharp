using LoxSharp.ErrorHandling;
using LoxSharp.Scanning;

namespace LoxSharp
{
    public sealed class Lox
    {
        private static void Run(string source, IErrorReporter errorReporter)
        {
            foreach (var token in new Scanner(source, errorReporter))
            {
                Console.WriteLine(token);
            }
        }

        private static async Task RunFile(string path, IErrorReporter errorReporter)
        {
            var script = await File.ReadAllTextAsync(path);
            Run(script, errorReporter);
        }

        private static void RunPrompt(IErrorReporter errorReporter)
        {
            for (;;)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null)
                {
                    break;
                }
                Run(line, errorReporter);
            }
        }

        public static async Task Main(string[] args)
        {
            var errorReporter = new ConsoleErrorReporter();
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                /*
                 EX_USAGE (64): The command was used incorrectly, e.g., with the
			     wrong number of arguments, a bad flag, a bad synt in a parameter, or whatever.
                 */
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                await RunFile(args[0], errorReporter);
                if (errorReporter.HadError)
                {
                    /*
                     EX_DATAERR (65): The input data was incorrect in some way.  This
			         should only be used for user's data and not system files.
                     */
                    Environment.Exit(65);
                }
            }
            else
            {
                RunPrompt(errorReporter);
            }
        }
    }
}