using System;
using System.IO;
namespace rinseoffcli
{
    class Program
    {

        static void showHelp(int exitCode = 1){
            Console.WriteLine("Must specify store or load, then a file name followed by a key file.\nFormat: <verb> <data file> <key file>");
            System.Environment.Exit(exitCode);
        }
        static void storeData(string filepath, string keypath){
            Stream inputStream = Console.OpenStandardInput();
            if (! File.Exists(keypath)){
                Console.WriteLine("Key file " + keypath + " does not exist");
                System.Environment.Exit(3);
            }

        }
        static void Main(string[] args)
        {
            if (args.Length == 0){
                showHelp();
            }
            var cmd = args[0].ToLower();
            switch(cmd){
                case "store":
                    if (args.Length != 3){
                        Console.WriteLine("Invalid number of arguments");
                        showHelp(2);
                    }
                    storeData(args[1], args[2]);
                break;
                default:
                    Console.WriteLine("Invalid command");
                    showHelp();
                break;
                case "help":
                case "--help":
                case "-h":
                showHelp();
                break;
            }
        }
    }
}
