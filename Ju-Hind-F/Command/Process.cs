﻿using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.IO;

using firetruck.Utils;

using System.Diagnostics;

namespace firetruck.Command
{

    internal class Prooocessesss
    {

        public static string CurrentDirDest = UserHomeDir;

        private static string UserHomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static string firetruckEnvDir = Path.Combine(UserHomeDir, "vin_env", "vars");
        private static string EnvVarsFile = Path.Combine(firetruckEnvDir, "env_vars.json");
        private static string AliasesFile = Path.Combine(firetruckEnvDir, "aliases.json");

        public static List<string> DecoraterCommands = new List<string>();
        public static List<string> AlertCommands = new List<string>();
        private static Dictionary<string, string> EnvironmentVariables = new Dictionary<string, string>();
        private static Dictionary<string, string> Aliases = new Dictionary<string, string>();
        private static List<string> CommandHistory = new List<string>();

        static Prooocessesss()
        {
            CurrentDirDest = Environment.CurrentDirectory;
            EnsureEnvironmentSetup();
            LoadEnvironmentVariables();
            LoadAliases();
        }

        // Ensure the directory and files exist, or create them after Listing an error
        // Ensure the directory and files exist, or create them after Listing an error
        private static void EnsureEnvironmentSetup()
        {
            try
            {
                if (!Directory.Exists(firetruckEnvDir))
                {
                    firetruckErrorList.New("Error: Environment directory does not exist. Creating directory at " + firetruckEnvDir);
                    Directory.CreateDirectory(firetruckEnvDir);
                }

                if (!File.Exists(EnvVarsFile))
                {
                    firetruckErrorList.New("Error: env_vars.json file not Got. Creating file.");
                    File.WriteAllText(EnvVarsFile, "{}");
                }

                if (!File.Exists(AliasesFile))
                {
                    firetruckErrorList.New("Error: aliases.json file not Got. Creating file.");
                    File.WriteAllText(AliasesFile, "{}");
                }

                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
            catch (Exception ex)
            {
                firetruckErrorList.New($"Error creating environment setup: {ex.Message}");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
        }

        private static void SaveEnvironmentVariables()
        {
            File.WriteAllText(EnvVarsFile, JsonSerializer.Serialize(EnvironmentVariables, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static void LoadEnvironmentVariables()
        {
            try
            {
                if (File.Exists(EnvVarsFile))
                {
                    string json = File.ReadAllText(EnvVarsFile);
                    EnvironmentVariables = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                firetruckErrorList.New($"Error loading environment variables: {ex.Message}");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
        }

        private static void SaveAliases()
        {
            File.WriteAllText(AliasesFile, JsonSerializer.Serialize(Aliases, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static void LoadAliases()
        {
            try
            {
                if (File.Exists(AliasesFile))
                {
                    string json = File.ReadAllText(AliasesFile);
                    Aliases = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                firetruckErrorList.New($"Error loading aliases: {ex.Message}");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
        }

        public static List<List<string>> DecoCommands(List<string> commands)
        {
            DecoraterCommands.Clear();

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].StartsWith("@"))
                {
                    if (!commands[i].Equals("@"))
                    {
                        DecoraterCommands.Add(commands[i]);
                        commands.RemoveAt(i);
                        i--;
                    }
                    else if (commands[i].Equals("@"))
                    {
                        commands.RemoveAt(i);
                        i--;
                    }
                }
            }

            return new List<List<string>> { DecoraterCommands, commands };
        }

        public static List<string> SeperateThemCommands(List<string> commands)
        {
            List<string> SepCommands = new List<string>();
            StringBuilder currentCommand = new StringBuilder();

            foreach (var command in commands)
            {
                if (command == ";")
                {
                    SepCommands.Add(currentCommand.ToString().Trim());
                    currentCommand.Clear();
                }
                else
                {
                    if (currentCommand.Length > 0)
                    {
                        currentCommand.Append(" ");
                    }
                    currentCommand.Append(command);
                }
            }

            if (currentCommand.Length > 0)
            {
                SepCommands.Add(currentCommand.ToString().Trim());
            }

            return SepCommands;
        }

        public static void ProcessBinCommand(string[] parts)
        {
            if (parts.Length == 1 && parts[0] == "@bin")
            {
                firetruckErrorList.New($"Usage: @bin <command> - Display file contents in binary format");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
                return;
            }

            string executable = parts.Length > 1 ? parts[1] : parts[0];
            string[] args = parts.Skip(2).ToArray();

            string filePath = $"C:\\Users\\{Environment.UserName}\\vin_env\\bin\\{executable}\\{executable}.exe";
            try
            {
                using (Process process = Process.Start(filePath, string.Join(" ", args)))
                {
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                firetruckErrorList.New($"Error starting process: {ex.Message}");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
        }

        public static void ProcessIbinCommand(string[] parts)
        {

            //parts[0] = "";
            // Print the command parts for debugging purposes
            //Console.WriteLine($"PARTS: len: {parts.Length}");
            //foreach (var abc in parts)
            //{
            //    Console.WriteLine(abc);
            //}

            // Ensure the first part (parts[0]) is in the <executable>:<file> format
            if (parts.Length < 2 || !parts[1].Contains(":"))
            {
                firetruckErrorList.New($"Usage: @ibin <executable>:<file> <args> - Run binary files with arguments");
                //firetruckErrorList.New($"YourCommand:");
                //int x = 0;
                //foreach (var item in parts)
                //{
                //    firetruckErrorList.New($"{x}: Expected: `python:abc.py` Got: `{item}`");
                //    x = x + 1;
                //}
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
                return;
            }

            try
            {
                // Split the executable and file part (e.g., python:abc.py)
                string[] execFileParts = parts[1].Split(':');
                if (execFileParts.Length != 2)
                {
                    firetruckErrorList.New("Invalid command format. Expected format: <executable>:<file>");
                    //firetruckErrorList.New($"YourCommand:");
                    //int x = 0;
                    //foreach (var item in parts)
                    //{
                    //    firetruckErrorList.New($"{x}: Expected: `python:abc.py` Got: `{item}`");
                    //    x = x + 1;
                    //}
                    firetruckErrorList.ListThem();
                    firetruckErrorList.CacheClean();
                    return;
                }

                // Extract the executable (e.g., "python") and file (e.g., "abc.py")
                string executable = execFileParts[0];  // e.g., "python"
                string fileName = execFileParts[1];    // e.g., "abc.py"

                // Combine the rest of the arguments (if any) (e.g., arg1, arg2)
                string[] args = parts.Length > 2 ? parts.Skip(1).ToArray() : new string[1];  // Skip the first part

                // Construct the full path for the script in the ibin directory
                string filePath = Path.Combine($"C:\\Users\\{Environment.UserName}\\vin_env\\ibin\\{executable}", fileName);

                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    firetruckErrorList.New($"Error: File '{fileName}' not Got in {filePath}");
                    firetruckErrorList.ListThem();
                    firetruckErrorList.CacheClean();
                    return;
                }

                // Prepare the process to execute the file
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = executable,  // e.g., "python"
                    Arguments = $"\"{filePath}\" " + string.Join(" ", args),  // Script path and arguments
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the process
                using (Process process = Process.Start(processStartInfo))
                {
                    // Capture the output and error streams
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // Display the output or error
                    if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine(output);
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        firetruckErrorList.New($"Error: {error}");
                        firetruckErrorList.ListThem();
                        firetruckErrorList.CacheClean();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle general errors during process execution
                firetruckErrorList.New($"Error starting process: {ex.Message}");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
        }

        private static string GetExtension(string executable)
        {
            switch (executable)
            {
                case "py":
                    return "py";
                case "roobi":
                    return "rb";
                default:
                    throw new ArgumentException("Unsupported executable");
            }
        }

        public static void ProoocessesssEach(List<string> commands)
        {
            if (!commands.Any()) { return; }

            foreach (var command in commands)
            {
                CommandHistory.Add(command);
                var parts = command.Split(' ');

                // Check if it is a get env arg value ` $(var_name) `
                //// List<string> parts = new List<string>();
                //// parts = ["@bin", "ls", "--path", "$(BinPath)"];
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("$("))
                    {
                        parts[i] = parts[i].Replace("$(", "");
                        parts[i] = parts[i].Replace(")", "");

                        //Console.WriteLine($":: {parts[i]}");

                        //firetruckOutput.result(EnvironmentVariables[parts[i]]);
                        try
                        {
                            parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                        }
                        catch (Exception exept)
                        {
                            //firetruckErrorList.New(exept.ToString());
                            firetruckErrorList.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                            firetruckErrorList.ListThem();
                            firetruckErrorList.CacheClean();
                            return;
                        }
                    }
                }

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("\\"))
                    {
                        parts[i] = parts[i].Replace("\\n", "\n");
                        parts[i] = parts[i].Replace("\\t", "\t");
                    }
                }

                string mainCommand = parts[0].ToLower();

                // Check if the command is an alias
                if (Aliases.ContainsKey(mainCommand))
                {
                    var aliasedCommand = Aliases[mainCommand];
                    parts = (aliasedCommand + " " + string.Join(" ", parts.Skip(1))).Split(' ');
                    mainCommand = parts[0].ToLower();
                }

                Console.WriteLine("");

                switch (mainCommand)
                {
                    case "@help":
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("!: This is FireTruckVM");

                        Console.WriteLine(" @help     - Get this help message");
                        Console.WriteLine(" @cls      - Clear the console");
                        Console.WriteLine(" @exit     - Exit the application");
                        Console.WriteLine(" @evars    - Manage environment variables ('set', 'get', 'list', 'unset')");
                        Console.WriteLine(" @alias    - Manage command aliases ('add', 'remove', 'list')");
                        Console.WriteLine(" @history  - View or clear command history");
                        Console.WriteLine(" @encrypt  - Encrypt a file or directory");
                        Console.WriteLine(" @decrypt  - Decrypt a file or directory");
                        Console.WriteLine(" @bin      - Display file contents in binary format");
                        Console.WriteLine(" @ibin     - Convert binary string back to file");
                        Console.WriteLine(" @cd       - Change the current directory");
                        Console.WriteLine(" @std      - Standard Shell Lib, type `@std #help` for help");
                        Console.ResetColor();

                        Console.WriteLine("\n!: You can add more commands and aliases! Just explore and have fun!");
                        Console.WriteLine("!: Need more help? Type '@help' anytime!");
                        Console.ResetColor();
                        break;

                    case "@std":
                        ProcessStdCommand(parts);
                        break;

                    case "@cls":
                        Console.Clear();
                        break;

                    case "@cd":
                        ProoocessesssCdCommand(parts);
                        break;

                    case "@exit":
                        Console.WriteLine("Exiting the application...");
                        Environment.Exit(0);
                        break;

                    case "@evars":
                        ProoocessesssEnvCommand(parts);
                        break;

                    case "@alias":
                        ProoocessesssAliasCommand(parts);
                        break;

                    case "@history":
                        ProoocessesssHistoryCommand(parts);
                        break;

                    case "@encrypt":
                    case "@decrypt":
                        ProoocessesssEncryptionCommand(parts);
                        break;

                    case "@bin":
                        ProcessBinCommand(parts);
                        break;

                    case "@ibin":
                        ProcessIbinCommand(parts);
                        break;

                    default:
                        firetruckErrorList.New($"Command: `{command}` is not a valid internal command, type `@help` for help!");
                        firetruckErrorList.ListThem();
                        firetruckErrorList.CacheClean();
                        break;
                }
            }
        }

        private static void ProoocessesssCdCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                firetruckErrorList.New($"Usage: @cd <directory> - Change the current working directory.");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
                return;
            }

            string newDir = parts[1];

            try
            {
                if (newDir == "..")
                {
                    // Move up one directory level
                    newDir = Path.GetDirectoryName(CurrentDirDest);
                }
                else if (newDir == "~")
                {
                    newDir = UserHomeDir;
                }
                else if (!Path.IsPathRooted(newDir))
                {
                    newDir = Path.Combine(CurrentDirDest, newDir);
                }

                if (newDir == null)
                {
                    firetruckErrorList.New($"Error: Cannot navigate above the root directory.");
                    firetruckErrorList.ListThem();
                    firetruckErrorList.CacheClean();
                    return;
                }

                newDir = Path.GetFullPath(newDir);

                if (Directory.Exists(newDir))
                {
                    CurrentDirDest = newDir;
                    Environment.CurrentDirectory = newDir;  // Ensure the process working directory is also updated
                    Console.WriteLine($"Changed directory to: {CurrentDirDest}");
                }
                else
                {
                    firetruckErrorList.New($"Error: Directory '{newDir}' does not exist.");
                    firetruckErrorList.ListThem();
                    firetruckErrorList.CacheClean();
                }
            }
            catch (Exception ex)
            {
                firetruckErrorList.New($"Error: {ex.Message}");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
        }
        private static void ProcessStdCommand(string[] parts)
        {
            if (parts.Length < 2) return;

            try
            {
                // Get the current user's name
                string currentUser = Environment.UserName;

                // Base path to the firetruck std directory
                string basePath = $@"C:\Users\{currentUser}\vin_env\third_party\firetruck\std\";

                // Extract the command after "@std"
                string stdCommand = parts[1];
                string[] commandParts = stdCommand.Split('.'); // Split on dot to detect category, class, etc.

                // Start building the path
                string commandPath = basePath;
                foreach (var part in commandParts)
                {
                    commandPath = Path.Combine(commandPath, part); // Keep adding to the path for each command part
                }

                // The remaining parts are the command arguments (after the first two parts)
                string[] commandArgs = parts.Skip(2).ToArray();  // All arguments after the command

                // Check if the command is already an executable (like ls.exe)
                if (!commandPath.EndsWith(".exe"))
                {
                    // Try to add .exe to the command path to see if it's an executable
                    if (File.Exists(commandPath + ".exe"))
                    {
                        commandPath += ".exe";  // Append .exe and use it
                    }
                    else if (Directory.Exists(commandPath))
                    {
                        // If it's a directory, check if there is an executable inside the directory
                        string executableFile = Directory.EnumerateFiles(commandPath, "*.exe").FirstOrDefault();
                        if (executableFile != null)
                        {
                            commandPath = executableFile;  // Use the first found executable in the directory
                        }
                        else
                        {
                            Console.WriteLine($"Error: No executable found in {commandPath}");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: Command not found at {commandPath}");
                        return;
                    }
                }

                // Try to execute the command with the given arguments
                try
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = commandPath,
                        Arguments = string.Join(" ", commandArgs),  // Join the arguments into a single string
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(processInfo))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            Console.WriteLine(result);  // Output the result from the executed command
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing command: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing command: {ex.Message}");
            }
        }
        private static void ProoocessesssEnvCommand(string[] parts)
        {
            if (parts.Length < 2) return;

            switch (parts[1].ToLower())
            {
                case "set":
                    if (parts.Length >= 4)
                    {
                        EnvironmentVariables[parts[2]] = string.Join(" ", parts.Skip(3));
                        SaveEnvironmentVariables();
                        //Console.WriteLine($"Environment variable '{parts[2]}' set.");
                        firetruckOutput.result($"Environment variable '{parts[2]}' set.");
                    }
                    break;
                case "get":
                    if (parts.Length >= 3 && EnvironmentVariables.ContainsKey(parts[2]))
                    {
                        firetruckOutput.result(EnvironmentVariables[parts[2]]);
                    }
                    else
                    {
                        firetruckErrorList.New($"Environment variable '{parts[2]}' not Got.");
                        firetruckErrorList.ListThem();
                        firetruckErrorList.CacheClean();
                    }
                    break;
                case "list":
                    if (EnvironmentVariables.Count == 0)
                    {
                        firetruckErrorList.New("No environment variables set.");
                        firetruckErrorList.ListThem();
                        firetruckErrorList.CacheClean();
                    }
                    else
                    {
                        foreach (var kvp in EnvironmentVariables)
                        {
                            Console.WriteLine($" {kvp.Key} ==> {kvp.Value}");
                        }
                    }
                    break;
                case "unset":
                    if (parts.Length >= 3)
                    {
                        if (EnvironmentVariables.Remove(parts[2]))
                        {
                            SaveEnvironmentVariables();
                            Console.WriteLine($"Environment variable '{parts[2]}' removed.");
                        }
                        else
                        {
                            firetruckErrorList.New($"Environment variable '{parts[2]}' not Got.");
                            firetruckErrorList.ListThem();
                            firetruckErrorList.CacheClean();
                        }
                    }
                    break;
                default:
                    firetruckErrorList.New("Invalid @evars command. Use 'set', 'get', 'list', or 'unset'.");
                    firetruckErrorList.ListThem();
                    firetruckErrorList.CacheClean();
                    break;
            }
        }

        private static void ProoocessesssAliasCommand(string[] parts)
        {
            if (parts.Length < 2) return;

            switch (parts[1].ToLower())
            {
                case "add":
                    if (parts.Length >= 4)
                    {
                        Aliases[parts[2]] = string.Join(" ", parts.Skip(3));
                        SaveAliases();
                        firetruckOutput.result($"Alias '{parts[2]}' added.");
                    }
                    else
                    {
                        firetruckErrorList.New("Invalid alias command. Use '@alias add [name] [command]'.");
                        firetruckErrorList.ListThem();
                        firetruckErrorList.CacheClean();
                    }
                    break;
                case "remove":
                    if (parts.Length >= 3)
                    {
                        if (Aliases.Remove(parts[2]))
                        {
                            SaveAliases();
                            firetruckOutput.result($"Alias '{parts[2]}' removed.");
                        }
                        else
                        {
                            firetruckErrorList.New($"Alias '{parts[2]}' not Got.");
                            firetruckErrorList.ListThem();
                            firetruckErrorList.CacheClean();
                        }
                    }
                    else
                        Console.WriteLine("Invalid alias command. Use '@alias remove [name]'.");
                    break;
                case "list":
                    if (Aliases.Count == 0)
                    {
                        firetruckErrorList.New("No aliases defined.");
                        firetruckErrorList.ListThem();
                        firetruckErrorList.CacheClean();
                    }
                    else
                    {
                        foreach (var kvp in Aliases)
                        {
                            Console.WriteLine($" {kvp.Key} ==> {kvp.Value}");
                        }
                    }
                    break;
                default:
                    firetruckErrorList.New("Invalid @alias command. Use 'add', 'remove', or 'list'.");
                    firetruckErrorList.ListThem();
                    firetruckErrorList.CacheClean();
                    break;
            }
        }

        private static void ProoocessesssHistoryCommand(string[] parts)
        {
            if (parts.Length > 1 && parts[1].ToLower() == "clear")
            {
                CommandHistory.Clear();
                firetruckOutput.result("Command history cleared.");
            }
            else
            {
                if (CommandHistory.Count == 0)
                {
                    firetruckErrorList.New("Command history is empty.");
                    firetruckErrorList.ListThem();
                    firetruckErrorList.CacheClean();
                }
                else
                    for (int i = 0; i < CommandHistory.Count; i++)
                        Console.WriteLine($"{i + 1}: {CommandHistory[i]}");
            }
        }

        private static void ProoocessesssEncryptionCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                firetruckErrorList.New($"Usage: {parts[0]} [file_or_directory]");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
                return;
            }

            string path = parts[1];
            bool isEncrypt = parts[0].ToLower() == "@encrypt";

            if (File.Exists(path))
            {
                ProoocessesssFileEncryption(path, isEncrypt);
            }
            else if (Directory.Exists(path))
            {
                ProoocessesssDirectoryEncryption(path, isEncrypt);
            }
            else
            {
                firetruckErrorList.New($"File or directory not Got: {path}");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
        }

        private static void ProoocessesssFileEncryption(string filePath, bool isEncrypt)
        {
            try
            {
                string outputPath = isEncrypt ? filePath + ".enc" : filePath.Replace(".enc", "");
                byte[] key = new byte[32]; // 256-bit key
                byte[] iv = new byte[16];  // 128-bit IV

                // In a real-world scenario, you'd want to securely generate and manage these
                new Random().NextBytes(key);
                new Random().NextBytes(iv);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (FileStream inputFile = new FileStream(filePath, FileMode.Open))
                    using (FileStream outputFile = new FileStream(outputPath, FileMode.Create))
                    {
                        ICryptoTransform cryptoTransform = isEncrypt
                            ? aes.CreateEncryptor()
                            : aes.CreateDecryptor();

                        using (CryptoStream cryptoStream = new CryptoStream(outputFile, cryptoTransform, CryptoStreamMode.Write))
                        {
                            inputFile.CopyTo(cryptoStream);
                        }
                    }
                }

                firetruckOutput.result($"{(isEncrypt ? "Encrypted" : "Decrypted")} file: {outputPath}");
            }
            catch (Exception ex)
            {
                firetruckErrorList.New($"Error during {(isEncrypt ? "encryption" : "decryption")}: {ex.Message}");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
        }

        private static void ProoocessesssDirectoryEncryption(string dirPath, bool isEncrypt)
        {
            try
            {
                foreach (string filePath in Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories))
                {
                    ProoocessesssFileEncryption(filePath, isEncrypt);
                }
                firetruckOutput.result($"Finished {(isEncrypt ? "encrypting" : "decrypting")} directory: {dirPath}");
            }
            catch (Exception ex)
            {
                firetruckErrorList.New($"Error during directory {(isEncrypt ? "encryption" : "decryption")}: {ex.Message}");
                firetruckErrorList.ListThem();
                firetruckErrorList.CacheClean();
            }
        }
    }

    public class PleaseProoocessesss
    {
        public static void TheseCommands(List<string> commands)
        {
            List<string> separatedCommands = Prooocessesss.SeperateThemCommands(commands);
            Prooocessesss.ProoocessesssEach(separatedCommands);
        }
    }
}