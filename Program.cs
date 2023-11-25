using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EncriptarDesencriptarEnv
{
    class Encriptacion
    {
        private static readonly string claveOriginal = "!Tb^5Kv@8uQgFhPw2sLrNz3GqZc6Xy+Rb!Tb^5Kv@8uQgFhPw2sLrNz3GqZc6Xy+Rb!Tb^5Kv@8uQgFhPw2sLrNz3GqZc6Xy+Rb!Tb^5Kv@8uQgFhPw2sLrNz3"; // Cambia esto a una clave segura
        private static readonly string clave = GenerarClave();

        private static string GenerarClave()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(claveOriginal));
                return Convert.ToBase64String(hashBytes).Substring(0, 32); // Tomar solo los primeros 32 bytes (256 bits)
            }
        }

        public static void EncriptarYGuardarArchivo(string rutaArchivo, string contenido)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(clave);
                    aesAlg.IV = aesAlg.Key.Take(16).ToArray();

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (FileStream fsOut = new FileStream(rutaArchivo + ".env", FileMode.Create))
                    {
                        using (CryptoStream cs = new CryptoStream(fsOut, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(contenido);
                            cs.Write(bytes, 0, bytes.Length);
                        }
                    }

                    Console.WriteLine($"Archivo encriptado y guardado en: {rutaArchivo}.env");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al encriptar y guardar el archivo: {ex.Message}");
            }
        }

        public static string DesencriptarYLeerArchivo(string rutaArchivoEncriptado)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(clave);
                    aesAlg.IV = aesAlg.Key.Take(16).ToArray();

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (FileStream fsIn = new FileStream(rutaArchivoEncriptado, FileMode.Open))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(fsIn, decryptor, CryptoStreamMode.Read))
                            {
                                cs.CopyTo(ms);
                            }

                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al desencriptar y leer el archivo: {ex.Message}");
                return null;
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string rutaArchivo = ""; // Puedes cambiar el nombre del archivo base según tus necesidades
            string contenidoOriginal = "SERVIDOR=localhost\nBD=Radionica\nCorreoRemitente=hsemail2023@gmail.com\nContrasena=temvndzlivfiuylh\nBaud=115200";

            //// Encriptar y guardar el archivo
            //Encriptacion.EncriptarYGuardarArchivo(rutaArchivo, contenidoOriginal);
            //Console.ReadKey();

            // Desencriptar y leer el archivo
            string contenidoDesencriptado = Encriptacion.DesencriptarYLeerArchivo(rutaArchivo + ".env");
            Console.WriteLine($"Contenido desencriptado: \n{contenidoDesencriptado}");

            Console.ReadKey();
        }
    }
}