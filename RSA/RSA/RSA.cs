﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Linq;

/*
 * RSA.cs
 * Developer: Pedro Batista
 * Developement stage: Encryption and Decryption a funcionar, falta fazer para mostrar chaves publicas e privadas
 * Tested on: Windows 10 
 * 
 */

namespace RSA
{
    public class RSA
    {
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        private CspParameters Cp;
        private string Path { get; set; }

        public List<string> EntidadesGuardadas { get; set; }
        public RSAParameters PublicKey { get; set; }
        private RSAParameters PrivateKey { get; set; }

        public RSA(string EtName)
        {

            Path = EtName + "RSAEntities.txt";

            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            EntidadesGuardadas = new List<string>();
            EntidadesGuardadas.Add(EtName);

            //TODO Ver se existe maneira mais compacta de fazer isto
            if (File.Exists(Path))
            {

                string[] entidades = File.ReadAllLines(Path);

                foreach (string entidade in entidades)
                {
                    EntidadesGuardadas.Add(DecryptName(entidade));
                }
            }

            PublicKey = Rsa.ExportParameters(false);
            PrivateKey = Rsa.ExportParameters(true);


        }

        #region encrypt
        /// <summary>
        /// Encripta uma mensagem 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="EntityPublicKey"></param>
        /// <returns byte[]></returns>
        public byte[] EncryptToByte(string message, string EtName = null)
        {
            if (String.IsNullOrEmpty(EtName))
            {
                byte[] messageByte = Encoding.UTF8.GetBytes(message);
                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
                Rsa.ImportParameters(PublicKey);
                return Rsa.Encrypt(messageByte, true);
            }
            else if (EntidadesGuardadas.Contains(EtName))
            {
                byte[] messageByte = Encoding.UTF8.GetBytes(message);

                Cp = new CspParameters { KeyContainerName = EtName };
                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

                return Rsa.Encrypt(messageByte, true);
            }
            else
            {
                Console.WriteLine("Não tens a chave publica da entidade " + EtName);
                return null;
            }
        }

        public byte[] EncryptToByte(byte[] message, string EtName = null)
        {
            if (String.IsNullOrEmpty(EtName))
            {
                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
                Rsa.ImportParameters(PublicKey);
                return Rsa.Encrypt(message, true);
            }
            else if (EntidadesGuardadas.Contains(EtName))
            {
                Cp = new CspParameters { KeyContainerName = EtName };
                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

                return Rsa.Encrypt(message, true);
            }
            else
            {
                Console.WriteLine("Não tens a chave publica da entidade " + EtName);
                return null;
            }
        }

        public string EncryptToString(string message, string EtName = null)
        {
            if (String.IsNullOrEmpty(EtName))
            {
                byte[] messageByte = Encoding.UTF8.GetBytes(message);

                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
                Rsa.ImportParameters(PublicKey);
                return Convert.ToBase64String(Rsa.Encrypt(messageByte, true));
            }
            else if (EntidadesGuardadas.Contains(EtName))
            {
                byte[] messageByte = Encoding.UTF8.GetBytes(message);

                Cp = new CspParameters { KeyContainerName = EtName };
                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

                return Convert.ToBase64String(Rsa.Encrypt(messageByte, true));
            }
            else
            {
                Console.WriteLine("Não tens a chave publica da entidade " + EtName);
                return null;
            }
        }

        public string EncryptToString(byte[] message, string EtName = null)
        {
            if (String.IsNullOrEmpty(EtName))
            {
                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
                Rsa.ImportParameters(PublicKey);
                return Convert.ToBase64String(Rsa.Encrypt(message, true));
            }
            else if (EntidadesGuardadas.Contains(EtName))
            {

                Cp = new CspParameters { KeyContainerName = EtName };
                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

                return Convert.ToBase64String(Rsa.Encrypt(message, true));
            }
            else
            {
                Console.WriteLine("Não tens a chave publica da entidade " + EtName);
                return null;
            }
        }
        #endregion

        #region decrypt
        /// <summary>
        /// Decifra uma mensagem
        /// </summary>
        /// <param name="message"></param>
        /// <returns byte[]></returns>
        public byte[] DecryptToByte(byte[] message)
        {
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(PrivateKey);
            return Rsa.Decrypt(message, true);
        }

        public string DecryptToString(byte[] message)
        {
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(PrivateKey);
            return Encoding.UTF8.GetString(Rsa.Decrypt(message, true));
        }

        public byte[] DecryptToByte(string message)
        {
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(PrivateKey);
            return Rsa.Decrypt(Encoding.UTF8.GetBytes(message), true);
        }

        public string DecryptToString(string message)
        {
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(PrivateKey);
            return Encoding.UTF8.GetString(Rsa.Decrypt(Encoding.UTF8.GetBytes(message), true));
        }

        #endregion

        #region Adicionar Chave Publica
        /// <summary>
        /// Adiciona uma entidade ao container
        /// </summary>
        /// <param name="EtName"></param>
        /// <param name="PublicKey"></param>
        public void AddEntintyPublicKey(string EtName, RSAParameters EtParameters)
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            Rsa.ImportParameters(EtParameters);

            if (!File.Exists(Path))
            {
                File.WriteAllText(Path, EncryptName(EtName) + "\n");
                EntidadesGuardadas.Add(EtName);
            }
            else if (File.Exists(Path))
            {
                if (!EntidadesGuardadas.Contains(EtName))
                {
                    File.AppendAllText(Path, EncryptName(EtName) + "\n");
                    EntidadesGuardadas.Add(EtName);
                }
            }
        }
        #endregion

        #region Mostrar Chave Publica de uma Entidade
        /// <summary>
        /// vai buscar a chave publica de uma entidade ao container
        /// </summary>
        /// <param name="etname"></param>
        public void ShowEntityPublicKey(string EtName)
        {
            if (EntidadesGuardadas.Contains(EtName))
            {
                while (true)
                {
                    Console.Write("\n1 - Numero\n2 - Texto\n");
                    string escolha = Console.ReadLine();

                    if (escolha == "1")
                    {
                        ShowPublicNumber(EtName);
                        break;
                    }
                    else if (escolha == "2")
                    {
                        ShowPublicStr(EtName);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Comando errado");
                    }
                }
            }
            else
            {
                Console.WriteLine("Não tem a chave publica desta entidade");
            }
        }

        private void ShowPublicNumber(string EtName)
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            RSAParameters RSAKeyInfo = Rsa.ExportParameters(false);
            Console.WriteLine("\n" + EtName + "\n");
            Console.WriteLine("Expoente :");

            Console.Write(" [ ");
            foreach (byte number in RSAKeyInfo.Exponent)
            {
                Console.Write(number + " ");
            }
            Console.Write(" ]\n");

            Console.WriteLine("Modulo :");
            Console.Write(" [ ");
            foreach (byte number in RSAKeyInfo.Modulus)
            {
                Console.Write(number + " ");
            }

            Console.Write(" ]\n");
        }

        private void ShowPublicStr(string EtName)
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            RSAParameters RSAKeyInfo = Rsa.ExportParameters(false);
            Console.WriteLine("\n" + EtName + "\n");
            Console.WriteLine("Expoente :");
            Console.Write(EtName + " [ " + Convert.ToBase64String(RSAKeyInfo.Exponent) + " ]\n");
            Console.WriteLine("Modulo :");
            Console.Write(EtName + " [ " + Convert.ToBase64String(RSAKeyInfo.Modulus) + " ]\n");
        }

        #endregion

        /// <summary>
        /// Apaga a key de uma entidade do container
        /// </summary>
        /// <param name="EtName"></param>
        public void DeleteKeyFromContainer(string EtName)
        {
            if (EntidadesGuardadas.Contains(EtName))
            {
                Cp = new CspParameters { KeyContainerName = EtName };

                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp)
                {
                    PersistKeyInCsp = false
                };
                Rsa.Clear();
                string[] Entidades = File.ReadAllLines(Path);
                File.Delete(Path);
                Entidades = Entidades.Where(val => DecryptName(val) != EtName).ToArray();
                File.WriteAllLines(Path, Entidades);
                EntidadesGuardadas = EntidadesGuardadas.Where(val => val != EtName).ToList();
            }
        }

        #region EncriptarNomes
        public static string EncryptName(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string DecryptName(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        #endregion
    }
}
