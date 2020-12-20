using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using rinseoff;
namespace rinseoffcli
{
    class Program
    {

        static void showHelp(int exitCode = 1){
            Console.WriteLine("Must specify keygen <key path> or store/load, then a file name followed by a key file.\nFormat: <verb> <data file> <key file>");
            Environment.Exit(exitCode);
        }
        static void validateArgCount(string[] args, int count){
            if (args.Length != count){
                stderrWrite("Invalid number of arguments");
                showHelp(2);
            }
        }
        static void stderrWrite(string msg){
            var stderrStream = Console.Error;
            stderrStream.WriteLine(msg);
            stderrStream.Flush();
        }
        static void loadData(string filepath, string keypath){
            // Load in an encrypted file and use a key file to decrypt it, then log bytes back to stdout
            byte[] plaintext = {};
            byte[] readBytes(string file){
                // Read bytes in from a file, exit with error message if not possible
                byte[] bytesToRead = {};
                try{
                    bytesToRead = File.ReadAllBytes(file);
                }
                catch(FileNotFoundException){
                    stderrWrite(file + " is not found");
                    Environment.Exit(9);
                }
                catch(UnauthorizedAccessException){
                    stderrWrite("No permssion to read " + file);
                    Environment.Exit(10);
                }
                catch(IOException){
                    stderrWrite("Failed to read " + file);
                    Environment.Exit(11);
                }
                return bytesToRead;
            }
            var stdout = Console.OpenStandardOutput();
            try{
                // Decrypt a file using a key file
                plaintext = RinseOff.decrypt_secret_bytes(
                    readBytes(filepath),
                    readBytes(keypath)
                );
            }
            catch(Sodium.Exceptions.KeyOutOfRangeException){
                stderrWrite("Keyfile is not appropriate size for key");
                Environment.Exit(4);
            }
            catch(System.Security.Cryptography.CryptographicException){
                stderrWrite("Could not decrypt " + filepath + " with " + keypath);
                Environment.Exit(12);
            }
            // print the plaintext and exit
            foreach(byte b in plaintext){
                stdout.WriteByte(b);
            }
            stdout.Flush();
        }

        static void storeData(string filepath, string keypath){
            byte[] encryptedInput = new byte[0];

            byte[] readUntilClose(){
                // Read binary from STDIN until close
                var readData = new List<byte>();
                Stream inputStream = Console.OpenStandardInput();
                int inp;
                while(true){
                    inp = inputStream.ReadByte();
                    if (inp != -1){
                        readData.Add((byte) inp);
                    }
                    else{
                        return readData.ToArray();
                    }
                }
            }
            // Encrypt stdin with keyfile data then write out to output file
            try{
                encryptedInput = RinseOff.encrypt_secret_bytes(readUntilClose(), File.ReadAllBytes(keypath));
            }
            catch(FileNotFoundException){
                stderrWrite("Key file " + keypath + " does not exist");
                Environment.Exit(3);
            }
            catch(Sodium.Exceptions.KeyOutOfRangeException){
                stderrWrite("Keyfile is not appropriate size for key");
                Environment.Exit(4);
            }
            catch(IOException){
                stderrWrite("Failed to read key file " + keypath);
                Environment.Exit(5);
            }
            try{
                File.WriteAllBytes(filepath, encryptedInput);
            }
            catch(ArgumentNullException)
            {
                Environment.Exit(0);
            }
            catch(DirectoryNotFoundException){
                stderrWrite("Output path " + filepath + " not found");
                Environment.Exit(7);
            }
            catch(IOException){
                stderrWrite("Could not write to " + filepath);
                Environment.Exit(8);
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0){
                showHelp();
            }
            var cmd = args[0].ToLower();
            switch(cmd){
                case "keygen":
                    validateArgCount(args, 2);
                    try{
                        RinseOff.generateKeyFile(args[1]);
                    }
                    catch(UnauthorizedAccessException){
                        stderrWrite("Cannot write to key file due to lack of perms " + args[1]);
                        Environment.Exit(6);
                    }
                    catch(DirectoryNotFoundException){
                        stderrWrite("Path not found " + args[1]);
                        Environment.Exit(6);
                    }
                    catch(IOException){
                        stderrWrite("Path not found " + args[1]);
                        Environment.Exit(6);
                    }
                break;
                case "store":
                    validateArgCount(args, 3);
                    storeData(args[1], args[2]);
                break;
                case "load":
                    validateArgCount(args, 3);
                    loadData(args[1], args[2]);
                break;
                default:
                    stderrWrite("Invalid command");
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
