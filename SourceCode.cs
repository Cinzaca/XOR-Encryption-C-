using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HideAndSeek
{
    class Program
    {
        private static string filePath1 = Directory.GetCurrentDirectory() + "uncryptedFile.txt";
        private static string filePath2= Directory.GetCurrentDirectory() + "cryptedFile.txt";
        private static readonly Dictionary<char, string> hexCharacterToBinaryDictionary = new Dictionary<char, string> 
        {
            { '0', "0000" },
            { '1', "0001" },
            { '2', "0010" },
            { '3', "0011" },
            { '4', "0100" },
            { '5', "0101" },
            { '6', "0110" },
            { '7', "0111" },
            { '8', "1000" },
            { '9', "1001" },
            { 'a', "1010" },
            { 'b', "1011" },
            { 'c', "1100" },
            { 'd', "1101" },
            { 'e', "1110" },
            { 'f', "1111" }
        };
        static void Main(string[] args)
        {
            string filePathUncrypted= filePath1;
            string filePathCrypted = filePath2;
            string uncryptedData = "";
            string[] vec;
            string key = "";

            while(true)
            {
                Console.WriteLine("Type a command: ");
                vec = Console.ReadLine().Split("\\s+");
                switch(vec[0])
                {
                    case "/?": 
                        Console.WriteLine("Commands:");
                        Console.WriteLine("InputFileName: Used for adding the path of the uncrypted file!");
                        Console.WriteLine("key: Used for adding the encryptiong key in hexadecimal format!");
                        Console.WriteLine("/d: Used for XOR operation");
                        Console.WriteLine("/v: Used for CRC operation");
                        Console.WriteLine("exit: Close the console!");
                        Console.WriteLine("currentDir: show the path of the current directory!");
                        Console.WriteLine("ATENTION: Due to the poor explination of the instructions, this program will only encrypt the data and there will be two files" +
                            " one where the uncrypted data is stored and one where the data after XOR and CRC will be located!" + " Make sure to add the both paths!");
                        Console.WriteLine(" ");
                        break;

                    case "InputFileName":
                        Console.WriteLine("Enter the path of the file that you want to encrypt: ");
                        filePathUncrypted = Console.ReadLine();
                        Console.WriteLine("Enter the path of the location where you want your encrypted file to be place at: ");
                        filePathCrypted = Console.ReadLine();

                        if (!filePathUncrypted.Contains(@"C:\"))
                        {
                            Console.WriteLine("The files couldn't be found! I am searching for the file in the current directory!");
                            filePathUncrypted = Directory.GetCurrentDirectory() + @"\uncryptedFile.txt";
                            filePathCrypted = Directory.GetCurrentDirectory() + @"\cryptedFile.txt";
                            uncryptedData = ReadFromFile(filePathUncrypted);
                        }else
                        {
                            uncryptedData = ReadFromFile(filePathUncrypted);
                        }
                        Console.WriteLine(" ");
                        break;

                    case "key": 
                        Console.WriteLine("Enter the key in Hexa format: ");
                        key = HexStringToBinary(Console.ReadLine());
                        if (key.Length < 2)
                        {
                            Console.WriteLine("Invalid key format!");
                        }
                        else if (key == "")
                        {
                            Console.WriteLine("Key not specified");
                        }
                        Console.WriteLine(" ");
                        break;

                    case "/d":
                        if (uncryptedData.Length != XOR(uncryptedData, key, filePathCrypted).Length) 
                        {
                            Console.WriteLine("Invalid Encryption");
                        }

                        string verifyXOR = XOR(uncryptedData, key, filePathCrypted);
                        WriteIntoFile("XOR:", filePathCrypted);
                        WriteIntoFile(verifyXOR, filePathCrypted);
                        if(verifyXOR == "")
                        {
                            Console.WriteLine("Encryption failed");
                        }else
                        {
                            Console.WriteLine("XOR succeeded!");
                        }
                        Console.WriteLine(" ");
                        break;

                    case "/v":
                        string verifyCRC = CRC(uncryptedData, filePathCrypted);
                        if(verifyCRC == "")
                        {
                            Console.WriteLine("“Decrypted data validation failed");
                        }else
                        {
                            Console.WriteLine("“Decrypted data validation succeeded");

                        }
                        Console.WriteLine(" ");
                        break;

                    case "exit":
                        Console.WriteLine("Good bye!");
                        System.Environment.Exit(0);
                        Console.WriteLine(" ");
                        break;

                    case "currentDir":
                        Console.WriteLine("Current directory is: " + Directory.GetCurrentDirectory());
                        Console.WriteLine(" ");
                        break;

                    default: Console.WriteLine("Invalid parameter {0}");
                        Console.WriteLine(" ");
                        break;
                }
            }
        }

        private static void WriteIntoFile(string input, string path)
        {
                StreamWriter sw = new StreamWriter(path, true);
                sw.WriteLine(input);
                sw.Close();
        }

        private static string ReadFromFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string dataFromFile = sr.ReadToEnd();
            sr.Close();
            return dataFromFile;
        }


        private static string XOR(string uncryptedBytes, string key, string path)
        {
            int f = 0;
            int restOfZeros = uncryptedBytes.Length % key.Length;
            string cryptedBytes = "";
            char[] keyAsArray = key.ToCharArray();
            for(int i = 0; i <= restOfZeros; i++)
            {
                uncryptedBytes = uncryptedBytes + "0";
            }
            char [] uncryptedBytesArray = uncryptedBytes.ToCharArray();

            for (int i = 1; i < uncryptedBytesArray.Length; i+=keyAsArray.Length)
            {
                for (int j = i; j < keyAsArray.Length + i; j++)
                {
                    uint a = Convert.ToUInt32(uncryptedBytesArray[j]);
                    uint b = Convert.ToUInt32(keyAsArray[f]);
                    uint c = a ^ b;
                    cryptedBytes = cryptedBytes + c.ToString();
                    f++;
                }
                f = 0;
            }
            return cryptedBytes;
        }

        private static string CRC(string uncryptedBytes, string path)
        {
            string helper = HexStringToBinary("00000000");
            uncryptedBytes = uncryptedBytes + HexStringToBinary("0000");
            int f = 0;
            string CRC_result = "";
            char[] temp = helper.ToCharArray();
            char[] copyArr = new char[32];
            char[] uncryptedBytesArray = uncryptedBytes.ToCharArray();
          
                for(int i = 0; i < uncryptedBytesArray.Length; i+=32)
                {
                CRC_result = "";
                        for(int j = i; j < 32 + i; j++)
                        {
                    int a = Convert.ToInt32(uncryptedBytesArray[j]);
                    int b = Convert.ToInt32(temp[f]);
                    int c = a ^ b;
                    CRC_result = CRC_result + c.ToString();
                    temp[f] = Convert.ToChar(c);
                    f++;
                        }
                f = 0;
                }
            WriteIntoFile("CRC:", path);
            WriteIntoFile(CRC_result, path);
            return CRC_result;
            }

        private static string HexStringToBinary(string hex)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in hex)
            {
                result.Append(hexCharacterToBinaryDictionary[char.ToLower(c)]);
            }
            return result.ToString();
        }
    }
    }
