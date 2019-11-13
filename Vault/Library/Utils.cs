// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace Microsoft.Vault.Library
{
    using Microsoft.Vault.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class Utils
    {
        public static void GuardVaultName(string vaultName)
        {
            if (false == Consts.ValidVaultNameRegex.Match(vaultName).Success)
            {
                throw new ArgumentException($"Invalid vault name {vaultName}, value must satisfy the following regex: {Consts.ValidVaultNameRegex}", nameof(vaultName));
            }
        }

        public static Dictionary<string, string> AddMd5ChangedBy(IDictionary<string, string> tags, string value, string changedBy)
        {
            tags = tags ?? new Dictionary<string, string>();
            tags[Consts.ChangedByKey] = changedBy ?? $"{Environment.UserDomainName}\\{Environment.UserName}";
            if (value != null)
            {
                tags[Consts.Md5Key] = CalculateMd5(value);
            }
            return new Dictionary<string, string>(tags);
        }

        public static string GetChangedBy(IDictionary<string, string> tags)
        {
            if ((tags == null) || (!tags.ContainsKey(Consts.ChangedByKey)))
            {
                return "";
            }
            return tags[Consts.ChangedByKey];
        }

        public static string CalculateMd5(string value)
        {
            byte[] buff = Encoding.UTF8.GetBytes(value);
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(buff);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }

        public static string GetMd5(IDictionary<string, string> tags)
        {
            if ((tags == null) || (!tags.ContainsKey(Consts.Md5Key)))
            {
                return "";
            }
            return tags[Consts.Md5Key];
        }

        /// <summary>
        /// Return True if this is Debug build, otherwise False
        /// </summary>
        public static bool IsDebug
        {
            get
            {
#if (DEBUG)
                return true;
#else
                return false;
#endif
            }
        }

        #region IList and IEnumerable Extensions

        /// <summary>
        /// Returns a random element from a list, or null if the list is empty.
        /// </summary>
        /// <typeparam name="T">The type of object being enumerated</typeparam>
        /// <param name="rand">An instance of a random number generator</param>
        /// <returns>A random element from a list, or null if the list is empty</returns>
        public static T Random<T>(this IEnumerable<T> list, CryptoRandomGenerator rand)
        {
            if (list != null && list.Count() > 0)
                return list.ElementAt(rand.Next(list.Count()));
            return default(T);
        }

        /// <summary>
        /// Returns a shuffled IEnumerable.
        /// </summary>
        /// <typeparam name="T">The type of object being enumerated</typeparam>
        /// <returns>A shuffled shallow copy of the source items</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(RandomThreadSafe.Instance);
        }

        /// <summary>
        /// Returns a shuffled IEnumerable.
        /// </summary>
        /// <typeparam name="T">The type of object being enumerated</typeparam>
        /// <param name="rand">An instance of a random number generator</param>
        /// <returns>A shuffled shallow copy of the source items</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, CryptoRandomGenerator rand)
        {
            var list = source.ToList();
            list.Shuffle(rand);
            return list;
        }

        /// <summary>
        /// Shuffles an IList in place.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            list.Shuffle(RandomThreadSafe.Instance);
        }

        /// <summary>
        /// Shuffles an IList in place.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        /// <param name="rand">An instance of a random number generator</param>
        public static void Shuffle<T>(this IList<T> list, CryptoRandomGenerator rand)
        {
            int count = list.Count;
            while (count > 1)
            {
                int i = rand.Next(count--);
                T temp = list[count];
                list[count] = list[i];
                list[i] = temp;
            }
        }

        #endregion
    }
}
