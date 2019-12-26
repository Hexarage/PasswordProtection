﻿using System;
using System.Security.Cryptography;
using System.Linq;

namespace PasswordProtection.Internals
{
    class PassHash
    {
        const int _SaltSize = 16, _HashSize = 34, _HashIter = 5000;

        public static string MakeHash(string password)
        {
            //INPUT: string password - A string containing the password to hashed
            //OUTPUT: A string made from the array of the (salt + hashed password)
            //DESCRIPTION: Hashes a password using PBKDF2/Rfc2898DeriveBytes for reliable password hashing

            byte[] salt, hash, Returnable;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[_SaltSize]);
            hash = new Rfc2898DeriveBytes(password, salt, _HashIter).GetBytes(_HashSize);
            
            Array.Copy(salt, 0, Returnable = new byte[_SaltSize + _HashSize], 0, _SaltSize);
            Array.Copy(hash, 0, Returnable, _SaltSize, _SaltSize + _HashSize);

            return Convert.ToBase64String(Returnable);
        }

        public bool CheckPass(string password, string savedPasswordHash)
        {
            //INPUT:    string password - A string containing the password to hashed
            //          string savedPasswordHash - A string containing the hash Array
            //OUTPUT: true if the password is a match, false if it isn't
            //DESCRIPTION: Hashes the input password with the input hash and checks it to the input hash

            byte[] salt, passHash, dbHash, HashArray;

            HashArray = Convert.FromBase64String(savedPasswordHash);
            Array.Copy(HashArray, 0, salt = new byte[_SaltSize], 0, _SaltSize);
            Array.Copy(HashArray, _SaltSize, dbHash = new byte[_SaltSize], 0, _HashSize);

            passHash = new Rfc2898DeriveBytes(password, salt, _HashIter).GetBytes(_HashSize);

            for (int i = 0; i < _HashSize; i++)
                if (passHash[i] != dbHash[i])
                    return false;
            return true;
        }
    }
}
