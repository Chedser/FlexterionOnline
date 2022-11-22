using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObmankaMP : MonoBehaviour
{

    public static string GetHash(string str) {

        byte[] bytes = Encoding.UTF8.GetBytes(str);

        byte[] resultByte;
        SHA512 shaM = new SHA512Managed();
        resultByte = shaM.ComputeHash(bytes);

        string result = Convert.ToBase64String(resultByte);

        return result;

    }

    public static string GetHashSalted(string str)
    {

        byte[] bytes = Encoding.UTF8.GetBytes(str + "obeafinegirlkissme!");

        byte[] resultByte;
        SHA512 shaM = new SHA512Managed();
        resultByte = shaM.ComputeHash(bytes);

        string result = Convert.ToBase64String(resultByte);

        return result;

    }

    public static string[] GetPassword() {

        string[] syms = {"q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", "f", "g", "h", "j", "k", "l", "z", "x", "c", "v", "b", "n","m",
              "Q", "W", "E", "R", "T",  "Y", "U", "I",  "O", "A", "S", "D", "F", "G", "H", "J", "K", "L",  "Z", "X", "C", "V",  "B", "N",  "M",
               "1", "2", "3",  "4",  "5", "6", "7", "8", "9", "0", "$", "&", "@", "%" };

        int length = Random.Range(7, 15);

        string pass = "";

        for (int i = 0; i < length; i++) {

            pass += syms[Random.Range(0, syms.Length)]; 

        }

        string hash = GetHash(pass);

        string[] result = {pass, hash};

        return result;

    }

  public static  string EncodeForServer(string str)
    {

        byte[] bytesToEncode = Encoding.UTF8.GetBytes(str);
        string encodedText = Convert.ToBase64String(bytesToEncode);

        return encodedText;

    }



}
