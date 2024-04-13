﻿using System;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace project3
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            try
            {
                if (BrokerAndValidator.valid_argument(args))
                {
                    await BrokerAndValidator.process_argument(args, client);
                }
                else
                {
                    BrokerAndValidator.PrintErrorMessage();
                }
            }
            catch
            {
                BrokerAndValidator.PrintErrorMessage();
            }
        }
    }

    internal class BrokerAndValidator
    {
        public static void PrintErrorMessage()
        {
            string errorMessage =
                "Usage: dotnet run <command> <argument>\n\n"
                + "Available commands:\n"
                + "- keyGen  <int>.....................Generate a keypair with <int> number of bits abd store them in public.key and private.key\n"
                + "- sendKey <email>...................Send a public key generated by keyGen to the server. This should be your email address.\n"
                + "- getKey  <email>...................Retrieve a public key for a particular user.\n"
                + "- sendMsg <email> <\"plain text\">....Send a message to a user. Ensure you have a public key for that user.\n"
                + "- getMsg  <email>...................Retrieve a message for a user. You can get any user but you will only be able to use your private key to decode\n\n";
            Console.WriteLine(errorMessage);
        }

        static async Task sendKey(string email, HttpClient client)
        {
            Console.WriteLine($"{email}");
            try
            {
                string publicKeyJson = File.ReadAllText("public.key");
                var publicJson = JsonSerializer.Deserialize<PublicKeyContent>(publicKeyJson);
                string publicKey = publicJson?.key ?? "";
                var content = new StringContent(
                    JsonSerializer.Serialize(
                        new PublicKeyContent { email = email, key = publicKey }
                    ),
                    Encoding.UTF8,
                    "application/json"
                );
                HttpResponseMessage response = await client.PutAsync(
                    $"http://voyager.cs.rit.edu:5050/Key/{email}",
                    content
                );
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("bananas");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    {
                        string privateKeyJson = File.ReadAllText("private.key");

                        var privateJson = JsonSerializer.Deserialize<PrivateKeyContent>(
                            privateKeyJson
                        );
                        bool emailExists = false;
                        foreach (string existingEmail in privateJson.email)
                        {
                            if (existingEmail == email)
                            {
                                emailExists = true;
                                break;
                            }
                        }
                        if (!emailExists)
                        {
                            string[] updatedEmailArray = new string[privateJson.email.Length + 1];
                            // Copy elements from the original array to the new array
                            Array.Copy(
                                privateJson.email,
                                updatedEmailArray,
                                privateJson.email.Length
                            );

                            // Add the new email to the last index of the new array
                            updatedEmailArray[privateJson.email.Length] = email;

                            // Assign the new array back to the object
                            privateJson.email = updatedEmailArray;

                            string updatedPrivateKeyJson = JsonSerializer.Serialize(privateJson);

                            // Write the updated private key to file
                            File.WriteAllText("private.key", updatedPrivateKeyJson);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                PrintErrorMessage();
            }
        }

        public static (BigInteger, BigInteger) getPrivateKeysFromJson(string json)
        {
            // Deserialize JSON string into a dynamic object
            PrivateKeyContent jsonObj = JsonSerializer.Deserialize<PrivateKeyContent>(json);
            // Decode base64-encoded key into byte array
            byte[] formattedBytes = Convert.FromBase64String(jsonObj.key);

            byte[] dBytes = new byte[4];
            byte[] DBytes;
            byte[] nBytes = new byte[4];
            byte[] NBytes;

            Buffer.BlockCopy(formattedBytes, 0, dBytes, 0, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(dBytes);
            }
            int dLength = BitConverter.ToInt32(dBytes, 0);
            DBytes = new byte[dLength];
            Buffer.BlockCopy(formattedBytes, 4, DBytes, 0, dLength);

            Buffer.BlockCopy(formattedBytes, 4 + dLength, nBytes, 0, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(nBytes);
            }

            int nLength = BitConverter.ToInt32(nBytes, 0);
            NBytes = new byte[nLength];

            Buffer.BlockCopy(formattedBytes, 8 + dLength, NBytes, 0, nLength);

            // Convert D and N to BigIntegers
            BigInteger D = new BigInteger(DBytes);
            BigInteger N = new BigInteger(NBytes);

            return (D, N);
        }

        public static (BigInteger, BigInteger) getKeysFromJson(string json)
        {
            // Deserialize JSON string into a dynamic object
            PublicKeyContent jsonObj = JsonSerializer.Deserialize<PublicKeyContent>(json);
            // Decode base64-encoded key into byte array
            byte[] formattedBytes = Convert.FromBase64String(jsonObj.key);

            byte[] eBytes = new byte[4];
            byte[] EBytes;
            byte[] nBytes = new byte[4];
            byte[] NBytes;

            Buffer.BlockCopy(formattedBytes, 0, eBytes, 0, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(eBytes);
            }
            int eLength = BitConverter.ToInt32(eBytes, 0);
            EBytes = new byte[eLength];
            Buffer.BlockCopy(formattedBytes, 4, EBytes, 0, eLength);

            Buffer.BlockCopy(formattedBytes, 4 + eLength, nBytes, 0, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(nBytes);
            }

            int nLength = BitConverter.ToInt32(nBytes, 0);
            NBytes = new byte[nLength];

            Buffer.BlockCopy(formattedBytes, 8 + eLength, NBytes, 0, nLength);

            // Convert D and N to BigIntegers
            BigInteger D = new BigInteger(EBytes);
            BigInteger N = new BigInteger(NBytes);

            return (D, N);
        }

        public static void getMessageFromJson(string json, BigInteger D, BigInteger N)
        {
            dynamic apples = JsonSerializer.Deserialize<PublicMessageContent>(json);
            Console.WriteLine(apples.content);
            byte[] keyBytes = Convert.FromBase64String(apples.content);
            BigInteger encryptedMessage = new BigInteger(keyBytes);
            Console.WriteLine(encryptedMessage);
            Console.WriteLine("encrypted bigint: {0}\n", encryptedMessage);

            BigInteger decryptedMessage = BigInteger.ModPow(encryptedMessage, D, N);
            Console.WriteLine("Decrypted bigint: {0}\n", decryptedMessage);
            byte[] messageBytes = decryptedMessage.ToByteArray();
            Console.WriteLine(Encoding.UTF8.GetString(messageBytes));
        }

        static async Task getMsg(string email, HttpClient client)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(
                    $"http://voyager.cs.rit.edu:5050/Message/{email}"
                );
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseBody))
                    {
                        Console.WriteLine("Empty response body");
                    }
                    else
                    {
                        // dont forget to make this conditional
                        if (has_private_key_email(email))
                        {
                            string privateKeyJson = File.ReadAllText("private.key");
                            var privateJson = JsonSerializer.Deserialize<PrivateKeyContent>(
                                privateKeyJson
                            );
                            byte[] bytes = Convert.FromBase64String(privateJson.key); // Decode Base64 string to byte array
                            BigInteger D,
                                N;

                            (D, N) = getPrivateKeysFromJson(privateKeyJson);
                            getMessageFromJson(responseBody, D, N);
                        }
                        else
                        {
                            Console.WriteLine(
                                "No private key for {0}. sendKey to them to put them on privateKey list",
                                email
                            );
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch data. Status code: {response.StatusCode}");
                    PrintErrorMessage();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                PrintErrorMessage();
            }
        }

        static async Task getKey(string email, HttpClient client)
        {
            Console.WriteLine($"{email}");
            try
            {
                HttpResponseMessage response = await client.GetAsync(
                    $"http://voyager.cs.rit.edu:5050/Key/{email}"
                );
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseBody))
                    {
                        Console.WriteLine("Empty response body");
                    }
                    else
                    {
                        File.WriteAllText($"{email}.key", responseBody);
                        Console.WriteLine($"Key has been written to {email}.key");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch data. Status code: {response.StatusCode}");
                    PrintErrorMessage();
                }
            }
            catch
            {
                PrintErrorMessage();
            }
        }

        static string formatKey(BigInteger E, BigInteger N)
        {
            byte[] eBytes = E.ToByteArray();
            byte[] nBytes = N.ToByteArray();
            int eSize = eBytes.Length;
            int nSize = nBytes.Length;
            byte[] eSizeBytes = BitConverter.GetBytes(eSize);
            byte[] nSizeBytes = BitConverter.GetBytes(nSize);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(eSizeBytes);
                Array.Reverse(nSizeBytes);
            }

            byte[] formattedBytes = new byte[8 + eSize + nSize];
            Buffer.BlockCopy(eSizeBytes, 0, formattedBytes, 0, 4);
            Buffer.BlockCopy(eBytes, 0, formattedBytes, 4, eSize);
            Buffer.BlockCopy(nSizeBytes, 0, formattedBytes, eSize + 4, 4);
            Buffer.BlockCopy(nBytes, 0, formattedBytes, 8 + eSize, nSize);

            string base64Key = Convert.ToBase64String(formattedBytes);
            return base64Key;
        }

        public static void keyGen(int bits)
        {
            int pBits = (int)(bits / 2 + bits * .25);
            int qBits = bits - pBits;

            BigInteger p = BigIntegerExtensions.getPrimes(pBits);
            BigInteger q = BigIntegerExtensions.getPrimes(qBits);

            BigInteger N = p * q;
            BigInteger T = (p - 1) * (q - 1);
            BigInteger E = 65536;

            BigInteger D = BigIntegerExtensions.modInverse(E, T);

            var publicKey = formatKey(E, N);
            var publicJson = new PublicKeyContent { email = "", key = publicKey };
            string publicKeyJson = JsonSerializer.Serialize(publicJson);
            File.WriteAllText("public.key", publicKeyJson);

            var privateKey = formatKey(D, N);
            Console.WriteLine("D: {0}, N: {1}", D, N);

            var privateJson = new PrivateKeyContent { email = new string[] { }, key = privateKey };
            string privateKeyJson = JsonSerializer.Serialize(privateJson);
            File.WriteAllText("private.key", privateKeyJson);
        }

        static async Task sendMsg(string email, string message, HttpClient client)
        {
            try
            {
                if (has_public_key_email(email))
                {
                    string publicKeyJson = File.ReadAllText($"{email}.key");
                    var publicJson = JsonSerializer.Deserialize<PublicKeyContent>(publicKeyJson);
                    byte[] bytes = Convert.FromBase64String(publicJson.key);
                    BigInteger E,
                        N;

                    (E, N) = getKeysFromJson(publicKeyJson);
                    byte[] messageByteArray = Encoding.UTF8.GetBytes(message);
                    BigInteger bigMessageInt = new BigInteger(messageByteArray);

                    BigInteger encryptedMessage = bigMessageInt;
                    BigInteger encodedMessage = BigInteger.ModPow(encryptedMessage, E, N);
                    byte[] encodedBytes = encodedMessage.ToByteArray();
                    string encodedString = Convert.ToBase64String(encodedBytes);
                    var content = new StringContent(
                        JsonSerializer.Serialize(
                            new PublicMessageContent { email = email, content = encodedString }
                        ),
                        Encoding.UTF8,
                        "application/json"
                    );
                    HttpResponseMessage response = await client.PutAsync(
                        $"http://voyager.cs.rit.edu:5050/Message/{email}",
                        content
                    );
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("yes");
                    }
                }
                else
                {
                    PrintErrorMessage();
                }
            }
            catch (Exception ex)
            {
                PrintErrorMessage();
                Console.WriteLine(ex);
            }
        }

        public static async Task process_argument(string[] args, HttpClient client)
        {
            try
            {
                switch (args[0])
                {
                    case "keyGen":
                        keyGen(int.Parse(args[1]));
                        break;
                    case "sendKey":
                        await sendKey(args[1], client);
                        break;
                    case "sendMsg":
                        await sendMsg(args[1], args[2], client);
                        break;
                    case "getMsg":
                        await getMsg(args[1], client);
                        break;
                    case "getKey":
                        await getKey(args[1], client);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                PrintErrorMessage();
            }
        }

        public static bool valid_argument(string[] args)
        {
            try
            {
                if (args.Length < 2 || args.Length > 3)
                {
                    return false;
                }

                switch (args[0])
                {
                    case "keyGen":
                        return int.TryParse(args[1], out _);
                    case "sendMsg":
                        return args.Length == 3;
                    case "sendKey":
                    case "getKey":

                    case "getMsg":
                        return args[1] is string;
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool has_private_key_email(string email)
        {
            try
            {
                string privateKeyJson = File.ReadAllText("private.key");
                var privateJson = JsonSerializer.Deserialize<PrivateKeyContent>(privateKeyJson);
                return privateJson.email.Contains(email);
            }
            catch
            {
                PrintErrorMessage();
                return false;
            }
        }

        public static bool has_public_key_email(string email)
        {
            try
            {
                string filePath = $"{email}.key";
                return File.Exists(filePath);
            }
            catch
            {
                PrintErrorMessage();
                return false;
            }
        }
    }

    public static class BigIntegerExtensions
    {
        public static BigInteger modInverse(BigInteger a, BigInteger b)
        {
            BigInteger i = b,
                v = 0,
                d = 1;
            while (a > 0)
            {
                BigInteger z = i / a,
                    x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - z * x;
                v = x;
            }
            v %= b;
            if (v < 0)
                v = (v + b) % b;
            return v;
        }

        static byte[] getNumber(int bits)
        /**
        This method generates a random byte array
        representing a number with the specified number of bits.
        */
        {
            int numberBytes = bits / 8;
            byte[] randomBytes = new byte[numberBytes];
            RandomNumberGenerator generator = RandomNumberGenerator.Create();

            generator.GetBytes(randomBytes);

            return randomBytes;
        }

        public static BigInteger getPrimes(int bits)
        /**
        This method generates a specified count of prime numbers with the specified number of bits.
        It uses multithreading to find prime numbers concurrently and ensures thread safety when
        updating the count of prime numbers found.
        */
        {
            BigInteger prime = 2;
            bool flag = true;
            while (flag)
            {
                ThreadPool.QueueUserWorkItem(
                    (state) =>
                    {
                        var bi = new BigInteger(getNumber(bits));
                        bi = BigInteger.Abs(bi); // no negative primes
                        if (bi.isProbablyPrime() == "probably prime")
                        {
                            prime = bi;
                            flag = false;
                        }
                    }
                );
            }
            return prime;
        }

        static BigInteger getRandomBigIntForPrimes(BigInteger max)
        /**
            gets random number between max and max-2
            @param max - the bigint max
            @return a - the bigint random number between max and max-2
        */
        {
            RandomNumberGenerator gen = RandomNumberGenerator.Create();
            int a = (max - 2).ToByteArray().Length;
            byte[] aSize = new byte[a];
            gen.GetBytes(aSize);
            BigInteger randomBigInt = new BigInteger(aSize);
            randomBigInt = BigInteger.Abs(randomBigInt);
            randomBigInt = (randomBigInt % (max - 2)) + 2;
            return randomBigInt;
        }

        public static string isProbablyPrime(this BigInteger n, int k = 10)
        {
            if (n <= 3)
            {
                return "probably prime";
            }

            // d = n-1 / 2^(r)
            // mod until d%2 != 0 will give value of r and d
            BigInteger d = n - 1;
            BigInteger r = 0;
            while (d % 2 == 0)
            {
                d /= 2;
                r += 1;
            }

            int count = k;
            while (count > 0)
            {
                count--;
                BigInteger a = getRandomBigIntForPrimes(n);
                BigInteger x = BigInteger.ModPow(a, d, n);

                if (x == 1 || x == n - 1)
                {
                    continue;
                }
                if (!innerLoop(r, x, n))
                {
                    return "composite";
                }
            }
            return "probably prime";
        }

        public static bool innerLoop(BigInteger r, BigInteger x, BigInteger n)
        /**
        I wrote this function because the psuedo code sucks and theres no way to break out
        of a loop and continue that looks nice and i refuse to use goto since its bad practice
        */
        {
            while (r > 1)
            {
                r--;
                x = BigInteger.ModPow(x, 2, n);
                if (x == n - 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

public class PublicKeyContent
{
    public string? email { get; set; }
    public string? key { get; set; }
}

public class PublicMessageContent
{
    public string? email { get; set; }
    public string? content { get; set; }
}

public class PrivateKeyContent
{
    public string[]? email { get; set; }
    public string? key { get; set; }
}
