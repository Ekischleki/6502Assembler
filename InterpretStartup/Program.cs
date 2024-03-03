//THIS INTERPRETER IS IN A VERY EARLY STATE!


using System.Diagnostics;
using TASI.InternalLangCoreHandle;
using static TASI.Command;

namespace TASI.InterpretStartup
{
    class TASI_Main
    {


       


        public const string interpreterVer = "1.0";
        public static Logger interpretInitLog = new();
        public static void Main(string[] args)
        {
            Global global = new Global();
            string? location = null;
            if (args.Length == 1)
            {
                location = args[0];
            }
            else if (args.Length != 0)
            {
                ArgCheck.InterpretArguments(ArgCheck.TokeniseArgs(args, ArgCheck.argCommandsDefinitions), global);
            }

            if (location == null)
            {
                global.CurrentLine = -1;
                Console.Clear();

                Console.WriteLine("Enter file location with code:");
            }




            Stopwatch codeRuntime = new();


            //Remove comments 
            try
            {
              

                if (location == null)
                    location = (Console.ReadLine() ?? throw new CodeSyntaxException("Code is null.")).Replace("\"", "");
                global.MainFilePath = Path.GetDirectoryName(location);
                List<Command> commands = LoadFile.ByPath(location, global);

                codeRuntime.Start();


                
                global.CurrentLine = -1;

               
                
                int line = -1;
                /*
                while (true)
                {
                    ConsoleHelper.ClearConsole();



                    Format.PrintFormatedString(Format.FormatCommands(commands, line).Item1);
                    line++;

                    Console.ResetColor();



                    Console.ReadKey();
                }
                */

                
                codeRuntime.Stop();
                Console.WriteLine($"Code finished; Runtime: {codeRuntime.ElapsedMilliseconds} ms");
                Console.ReadKey(true);

            }
            catch (Exception ex)
            {

                Console.Clear();

                switch (ex)
                {



                    case CodeSyntaxException:
                        if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1 && new Random().Next(0, 20) == 1) //April fools
                        {
                            Console.WriteLine("There was a syntathical error in your code. But it can't be your fault, it's probably just an interpreter error.");
                            Console.WriteLine("April fools, it's your fault :P");
                            Console.WriteLine("--------------");
                        }
                        Console.WriteLine("There was a syntathical error in your code.");
                        if (global.CurrentLine != -1)
                            Console.WriteLine($"\nThe error happened on line: {global.CurrentLine + 1}");
                        Console.WriteLine("The error message is:");
                        Console.WriteLine(ex.Message);
                        break;

                    default:
                    case InternalInterpreterException:
                        Console.WriteLine("There was an internal error in the compiler.");
                        Console.WriteLine("Please report this error on github and please include the code and this error message and (if available) you inputs, that lead to this error. You can create a new issue, reporting the error here:\nhttps://github.com/Ekischleki/TASI/issues/new");
                        if (global.CurrentLine != -1)
                            Console.WriteLine($"\nThe error happened on line: {global.CurrentLine + 1}");
                        Console.WriteLine("The error message is:");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Here is the stack trace:");
                        Console.WriteLine(ex.StackTrace);
                        break;
                    case RuntimeCodeExecutionFailException runtimeException:
                        Console.WriteLine("The code threw a fail, because it couldn't take it anymore or smt...");
                        Console.WriteLine($"The fail type is:\n{runtimeException.exceptionType}");
                        Console.WriteLine($"The fail message is:\n{runtimeException.Message}");
                        break;
                }


                Console.ReadKey();

            }


            return;


        }
    }
}