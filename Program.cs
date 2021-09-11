using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ConsoleTables;

class Result
{

    public string result(string[] args, string userChoice, int computerChoice)
    {

        int diff = Convert.ToInt32(userChoice) + (args.Length / 2);
        if (diff >= args.Length)
        {
            if (computerChoice > diff && computerChoice < Convert.ToInt32(userChoice))
            {
                return ($"YOU LOSE");
            }
            else if (Convert.ToInt32(userChoice) == computerChoice)
            {
                return ($"DRAW");
            }
            else
            {
                return ($"YOU WIN");
            }
        }
        else
        {
            if (computerChoice > diff || computerChoice < Convert.ToInt32(userChoice))
            {
                return ($"YOU LOSE");
            }
            else if (Convert.ToInt32(userChoice) == computerChoice)
            {
                return ($"DRAW");
            }
            else
            {
                return ($"YOU WIN");
            }
        }
    }
}
class Table
{
    public void table(string[] args)
    {
        Result res = new Result();
        var table = new ConsoleTable(" User \\ PC ");
        table.AddColumn(args);
        string[][] row = new string[args.Length][];
        for (int j = 0; j < args.Length; j++)
        {
            row[j] = new String[args.Length + 1];
            row[j][0] = args[j];
            for (int i = 0; i < args.Length; i++)
            {
                row[j][i + 1] = res.result(args, Convert.ToString(j), i);
            }
            table.AddRow(row[j]);
        }
        table.Write();
    }
}

class Hmac
{
    public string hmac(string[] args)
    {
        /* SECRET KEY */
        var hashKey = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(512);
        int minVal = 1;
        int maxVal = args.Length;
        var rnd = new byte[128];
        using (var rng = new RNGCryptoServiceProvider())
            rng.GetBytes(rnd);
        var key = Math.Abs(BitConverter.ToInt32(rnd, 0));
        key = Convert.ToInt32(key % (maxVal - minVal + 1) + minVal);
        byte[] input = Encoding.ASCII.GetBytes(Convert.ToString(key));
        hashKey.BlockUpdate(input, 0, input.Length);
        byte[] result = new byte[64]; // 512 / 8 = 64
        hashKey.DoFinal(result, 0);
        string hashString = BitConverter.ToString(result);
        hashString = hashString.Replace("-", "").ToLowerInvariant();

        /* Computer Choice */
        int min = 1;
        int max = args.Length;
        var random = new byte[128];
        using (var r = new RNGCryptoServiceProvider())
            r.GetBytes(random);
        var computerChoice = Math.Abs(BitConverter.ToInt32(random, 0));
        computerChoice = Convert.ToInt32(computerChoice % (max - min + 1) + min);


        /* HMAC */
        var secret = hashString;
        var message = Convert.ToString(args[computerChoice - 1]);
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] keyBytes = encoding.GetBytes(secret);
        byte[] messageBytes = encoding.GetBytes(message);
        System.Security.Cryptography.HMACSHA512 cryptographer = new System.Security.Cryptography.HMACSHA512(keyBytes);
        byte[] bytes = cryptographer.ComputeHash(messageBytes);

        Console.WriteLine("HMAC:");
        Console.WriteLine(BitConverter.ToString(bytes).Replace("-", "").ToLower());
        string sh_cc = hashString + " " + Convert.ToString(computerChoice);
        return sh_cc;
    }
}
class Program
{
    public static void Main(string[] args)
    {
        HashSet<string> Hashargs = new HashSet<string>();
        foreach (string arg in args)
        {
            if (arg == "0")
            {
                Console.WriteLine("You cannot use zero, please choose a different value.");
                return;
            }
            else
            {
                if (Hashargs.Add(arg) == false)
                {
                    Console.WriteLine("You have introduced duplicate items.");
                    return;
                }
            }

        }

        if (args.Length < 3)
        {
            Console.WriteLine("You must enter more than two parameters."); return;

        }
        if (args.Length % 2 == 0)
        {
            Console.WriteLine("The number of parameters must be unpaired."); return;
        }
    Moves:
        Hmac k = new Hmac();

        string computerChoise = k.hmac(args);
        string[] hmak_cChoice = computerChoise.Split(new char[] { ' ' });


    next:
        Console.WriteLine("\nAvailable moves:");
        int i = 1;
        foreach (string arg in args)
        {
            Console.WriteLine($"{i} - {arg}");
            i++;
        }
        Console.WriteLine("---------------\nexit - exit\nhelp - help");


        Console.WriteLine("\nChoose your move:");

        string userChoice = Convert.ToString(Console.ReadLine());

        if (Convert.ToString(userChoice) == "help")
        {
            Console.WriteLine("HELP TABLE");
            Table help = new Table();
            help.table(args);
            Console.WriteLine("Press any key...");
            Console.ReadKey();
            goto Moves;
        }
        else if (Convert.ToString(userChoice) == "exit")
        {
            Console.WriteLine("EXIT"); return;
        }
        else if (Convert.ToInt32(userChoice) > args.Length)
        {
            Console.WriteLine($"Choose number < {args.Length}");
            goto next;
        }

        Result r = new Result();
        Console.WriteLine($"\n~~~~~{r.result(args, userChoice, Convert.ToInt32(hmak_cChoice[1]))}~~~~~\nYour choice: {args[Convert.ToInt32(userChoice) - 1]} \nPc choice: {args[Convert.ToInt32(hmak_cChoice[1])]}");
        Console.WriteLine($"\nSecret Key: {hmak_cChoice[0]}");

        Console.WriteLine("\nСontinue playing?\n1 -- yes    2 -- exit   3 -- menu");
        userChoice = (Console.ReadLine());
        if (Convert.ToString(userChoice) == "1")
        {
            goto Moves;
        }
        else if (Convert.ToString(userChoice) == "2")
        {
            Console.WriteLine("EXIT");
            return;
        }
        else if (Convert.ToString(userChoice) == "3")
        {
            goto Moves;
        }
    }

}