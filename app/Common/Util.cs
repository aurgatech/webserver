using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aurga.Common
{
    public static class Util
    {
        /// <summary>
        /// Convert hex string into byte array
        /// </summary>
        /// <param name="hexstr"></param>
        /// <returns></returns>
        public static byte[] Hex2Bytes(string hexstr)
        {
            if (string.IsNullOrEmpty(hexstr) || hexstr.Length % 2 != 0)
            {
                return null;
            }

            try
            {
                return Enumerable.Range(0, hexstr.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hexstr.Substring(x, 2), 16))
                         .ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Bytes2Hex(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return string.Empty;
            }

            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public static void PoisonSalt(byte[] salt, int[][] swaps, int invert, int zero)
        {
            byte tmp;
            for (int i = 0; i < swaps.Length; i++)
            {
                tmp = salt[swaps[i][0]];
                salt[swaps[i][0]] = salt[swaps[i][1]];
                salt[swaps[i][1]] = tmp;
            }

            if (invert > -1)
            {
                tmp = (byte)(~salt[invert] & 0xFF);
                salt[invert] = tmp;
            }

            if (zero > -1)
            {
                salt[zero] = 0;
            }
        }

        public static void Encrypt(byte[] input, byte[] key)
        {
            byte[] iv = { 0xA2, 0xB4, 0x00, 0xAE, 0xA3, 0x91, 0xE6, 0x40, 0x03, 0xF0, 0xD4, 0xEE, 0xA2, 0x9D, 0x76, 0x5B };
            // Create AES engine
            var engine = new AesEngine();

            // Create CTR cipher (no padding)
            var cipher = new BufferedBlockCipher(new SicBlockCipher(engine));

            // Initialize cipher with key and IV
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

            // Encrypt data
            byte[] output = new byte[cipher.GetOutputSize(input.Length)];
            int length1 = cipher.ProcessBytes(input, 0, input.Length, output, 0);
            cipher.DoFinal(output, length1);

            for (int i = 0; i < output.Length; i++)
            {
                input[i] = output[i];
            }
        }

        public static void GetDataByLocalKey(byte[] data, byte[] key)
        {
            int[][] swaps = { new[] { 3, 11 }, new[] { 4, 1 } };
            byte[] tmpKey = new byte[key.Length];
            Array.Copy(key, tmpKey, key.Length);

            PoisonSalt(tmpKey, swaps, 6, 5);
            Encrypt(data, tmpKey);
        }

        public static void GetDataByKey1(byte[] data, byte[] key)
        {
            int[][] swaps = { new[] { 4, 0 }, new[] { 7, 9 } };
            byte[] tmpKey = new byte[key.Length];
            Array.Copy(key, tmpKey, key.Length);

            PoisonSalt(tmpKey, swaps, 11, -1);
            Encrypt(data, tmpKey);
        }

        public static void GetDataByKey2(byte[] data, byte[] key)
        {
            int[][] swaps = { new[] { 4, 0 }, new[] { 7, 9 } };
            byte[] tmpKey = new byte[key.Length];
            Array.Copy(key, tmpKey, key.Length);

            PoisonSalt(tmpKey, swaps, 6, -1);
            Encrypt(data, tmpKey);
        }

        public static void GetDataByKey3(byte[] data, byte[] key)
        {
            int[][] swaps = { new[] { 7, 10 }, new[] { 2, 5 } };
            byte[] tmpKey = new byte[key.Length];
            Array.Copy(key, tmpKey, key.Length);

            PoisonSalt(tmpKey, swaps, 3, -1);
            Encrypt(data, tmpKey);
        }

        public static void GetDataByKey4(byte[] data, byte[] key)
        {
            int[][] swaps = { new[] { 2, 11 }, new[] { 6, 5 } };
            byte[] tmpKey = new byte[key.Length];
            Array.Copy(key, tmpKey, key.Length);

            PoisonSalt(tmpKey, swaps, 9, -1);
            Encrypt(data, tmpKey);
        }

        public static byte[] GenerateRandomBytes(int numberOfBytes)
        {
            byte[] randomBytes = new byte[numberOfBytes];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        public static string GenerateUserToken(string token)
        {
            var key = GenerateRandomBytes(16);
            var token_data = Hex2Bytes(token);

            int[][] swaps = { new[] { 0, 0 }, new[] { 0, 0 } };

            swaps[0][0] = key[5] % 15;
            swaps[0][1] = key[1] % 15;
            swaps[1][0] = key[10] % 15;
            swaps[1][1] = key[3] % 15;
            int revert = key[7] % 15;
            int zero = key[14] % 15;

            if (swaps[0][0] == swaps[0][1])
            {
                if (swaps[0][0] > 8)
                {
                    swaps[0][1] = swaps[0][0] - 1;
                }
                else
                {
                    swaps[0][1] = swaps[0][0] + 1;
                }
            }

            if (swaps[1][0] == swaps[1][1])
            {
                if (swaps[1][0] > 8)
                {
                    swaps[1][1] = swaps[1][0] - 1;
                }
                else
                {
                    swaps[1][1] = swaps[1][0] + 1;
                }
            }

            if (revert == zero)
            {
                if (revert > 8)
                {
                    zero = revert - 1;
                }
                else
                {
                    zero = revert + 1;
                }
            }

            byte[] tmpKey = new byte[key.Length];
            Array.Copy(key, tmpKey, key.Length);

            PoisonSalt(tmpKey, swaps, revert, zero);

            Encrypt(token_data, tmpKey);

            return Bytes2Hex(key) + Bytes2Hex(token_data);
        }

        public static string DecodeUserToken(string userToken)
        {
            var data = Hex2Bytes(userToken);
            var key = data.Take(16).ToArray();
            var token_data = data.Skip(16).ToArray();

            int[][] swaps = { new[] { 0, 0 }, new[] { 0, 0 } };

            swaps[0][0] = key[5] % 15;
            swaps[0][1] = key[1] % 15;
            swaps[1][0] = key[10] % 15;
            swaps[1][1] = key[3] % 15;
            int revert = key[7] % 15;
            int zero = key[14] % 15;

            if (swaps[0][0] == swaps[0][1])
            {
                if (swaps[0][0] > 8)
                {
                    swaps[0][1] = swaps[0][0] - 1;
                }
                else
                {
                    swaps[0][1] = swaps[0][0] + 1;
                }
            }

            if (swaps[1][0] == swaps[1][1])
            {
                if (swaps[1][0] > 8)
                {
                    swaps[1][1] = swaps[1][0] - 1;
                }
                else
                {
                    swaps[1][1] = swaps[1][0] + 1;
                }
            }

            if (revert == zero)
            {
                if (revert > 8)
                {
                    zero = revert - 1;
                }
                else
                {
                    zero = revert + 1;
                }
            }

            byte[] tmpKey = new byte[key.Length];
            Array.Copy(key, tmpKey, key.Length);

            PoisonSalt(tmpKey, swaps, revert, zero);

            Encrypt(token_data, tmpKey);

            return Bytes2Hex(token_data);
        }
    }
}

