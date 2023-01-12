using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DataEncryptoDecryptor
{
    public static string LoadSecretKey()
    {
        //TODO:
        return "8j8r0928c30j9238jc09j703j270c094";
    }

    public static T LoadEncryptedFile<T>(string filePath, string key)
        where T : class, new()
    {
        try
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            if (File.Exists(filePath))
            {
                Aes aes = Aes.Create();
                using (FileStream dataStream = new FileStream(filePath, FileMode.Open))
                {
                    byte[] iv = new byte[aes.IV.Length];
                    dataStream.Read(iv, 0, iv.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(
                       dataStream,
                       aes.CreateDecryptor(keyBytes, iv),
                       CryptoStreamMode.Read))
                    {
                        StreamReader reader = new StreamReader(cryptoStream);
                        string text = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(text);

                    }
                }
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("[DataEncryptoDecryptor] Exception occured. Msg: " + ex.Message);
        }

        return null;
    }

    public static void SaveEncryptedFile<T>(T data, string filePath, string key)
        where T : class
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        Aes aes = Aes.Create();
        using (FileStream dataStream = new FileStream(filePath, FileMode.Create))
        {
            dataStream.Write(aes.IV, 0, aes.IV.Length);
            using (CryptoStream cryptoStream = new CryptoStream(dataStream, aes.CreateEncryptor(keyBytes, aes.IV), CryptoStreamMode.Write))
            {
                using (StreamWriter writer = new StreamWriter(cryptoStream))
                {
                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    writer.Write(json);
                }
            }
        }
    }
}
