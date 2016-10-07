// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace VaultLibrary
{
    #region Using statements
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    /// Implements the common guard methods.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Checks an argument to ensure its value is expected value
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="expectedValue">The expected value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentIsEqual<T>([ValidatedNotNull] T argumentValue, [ValidatedNotNull] T expectedValue, string argumentName)
        {
            if (Comparer<T>.Default.Compare(argumentValue, expectedValue) != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidArgumentValue, argumentName, expectedValue));
            }
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty.
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>    
        public static void ArgumentNotNullOrEmptyString([ValidatedNotNull] string argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue.Length == 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.StringCannotBeEmpty, argumentName));
            }
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty.
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>    
        public static void ArgumentNotNullOrWhitespace([ValidatedNotNull] string argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.StringCannotBeEmpty, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument to ensure it isn't null.
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNull<T>([ValidatedNotNull] T argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Checks an argument to ensure that its value is not the default value for its type.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotDefaultValue<T>(T argumentValue, string argumentName)
        {
            if (IsDefaultValue(argumentValue))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeDefault, argumentName));
            }
        }

        /// <summary>
        /// Checks that string argument value matches the given regex.
        /// </summary>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="pattern">The regex pattern match.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotNullAndMatchRegex([ValidatedNotNull] string argumentValue, string pattern, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (!Regex.IsMatch(argumentValue, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(10)))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.StringMustMatchRegex, argumentName, pattern));
            }
        }

        /// <summary>
        /// Checks that all values of the specified argument satisfy a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValues">The values of the argument.</param>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentsSatisfyCondition<T>(IEnumerable<T> argumentValues, Func<T, bool> predicate, string argumentName)
        {
            if (argumentValues != null && !argumentValues.All(predicate))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentsConditionNotSatisfied, argumentName));
            }
        }

        /// <summary>
        /// Checks whether or not the specified collection is empty.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValues">The values of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentCollectionNotEmpty<T>(IEnumerable<T> argumentValues, string argumentName)
        {
            if (argumentValues == null || !argumentValues.Any())
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCollectionCannotBeEmpty, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Int32"/> to ensure that its value is not zero or negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Int32"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotZeroOrNegativeValue(int argumentValue, string argumentName)
        {
            if (argumentValue <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeZeroOrNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Int32"/> to ensure that its value is not negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Int32"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotNegativeValue(int argumentValue, string argumentName)
        {
            if (argumentValue < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Int64"/> to ensure that its value is not zero or negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Int64"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotZeroOrNegativeValue(long argumentValue, string argumentName)
        {
            if (argumentValue <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeZeroOrNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Int64"/> to ensure that its value is not negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Int64"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotNegativeValue(long argumentValue, string argumentName)
        {
            if (argumentValue < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Decimal"/> to ensure that its value is not zero or negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Decimal"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotZeroOrNegativeValue(decimal argumentValue, string argumentName)
        {
            if (argumentValue <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeZeroOrNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Decimal"/> to ensure that its value is not negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Decimal"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotNegativeValue(decimal argumentValue, string argumentName)
        {
            if (argumentValue < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Double"/> to ensure that its value is not zero or negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Double"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotZeroOrNegativeValue(double argumentValue, string argumentName)
        {
            if (argumentValue <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeZeroOrNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Double"/> to ensure that its value is not negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Double"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotNegativeValue(double argumentValue, string argumentName)
        {
            if (argumentValue < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Single"/> to ensure that its value is not zero or negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Single"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotZeroOrNegativeValue(float argumentValue, string argumentName)
        {
            if (argumentValue <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeZeroOrNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.Single"/> to ensure that its value is not negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Single"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotNegativeValue(float argumentValue, string argumentName)
        {
            if (argumentValue < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.TimeSpan"/> to ensure that its value is not zero or negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.TimeSpan"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotZeroOrNegativeValue(TimeSpan argumentValue, string argumentName)
        {
            if (argumentValue <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeZeroOrNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks an argument of type <see cref="System.TimeSpan"/> to ensure that its value is not negative.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.TimeSpan"/> value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotNegativeValue(TimeSpan argumentValue, string argumentName)
        {
            if (argumentValue < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeNegative, argumentName));
            }
        }

        /// <summary>
        /// Checks if the supplied argument falls into the given range of values.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="minValue">The minimum allowed value of the argument.</param>
        /// <param name="maxValue">The maximum allowed value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentInRange<T>(T argumentValue, T minValue, T maxValue, string argumentName) where T : IComparable<T>
        {
            if (Comparer<T>.Default.Compare(argumentValue, minValue) < 0 || Comparer<T>.Default.Compare(argumentValue, maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeOutOfRange, argumentName, minValue, maxValue));
            }
        }

        /// <summary>
        /// Checks if the supplied argument present in the collection of possible values.
        /// </summary>
        /// <remarks>
        /// Comprasion is case sensitive
        /// </remarks>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="collection">Collection of possible values</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentInCollection<T>(T argumentValue, IEnumerable<T> collection, string argumentName) where T : IComparable<T>
        {
            Guard.ArgumentNotNull(collection, "collection");
            if (!collection.Contains(argumentValue))
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentNotInCollection, argumentName, string.Join(",", collection)));
            }
        }

        /// <summary>
        /// Checks an argument to ensure that its value doesn't exceed the specified ceiling baseline.
        /// </summary>
        /// <param name="argumentValue">The <see cref="System.Double"/> value of the argument.</param>
        /// <param name="ceilingValue">The <see cref="System.Double"/> ceiling value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotGreaterThan(double argumentValue, double ceilingValue, string argumentName)
        {
            if (argumentValue > ceilingValue)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeGreaterThanBaseline, argumentName, ceilingValue));
            }
        }

        /// <summary>
        /// Checks an enum instance to ensure that its value is defined by the specified enum type.
        /// </summary>
        /// <param name="enumType">The enum type the value should correspond to.</param>
        /// <param name="enumValue">The enum value to check.</param>
        /// <param name="argumentName">The name of the argument holding the value.</param>
        /// <remarks>
        /// Does not currently support Flags enums.
        /// </remarks>
        [Obsolete("This method is not generic, where a generic method would be much more type-safe and easy to use. Please use EnumValueIsDefined<T>(T, string).")]
        public static void EnumValueIsDefined(Type enumType, object enumValue, string argumentName)
        {
            if (!Enum.IsDefined(enumType, enumValue))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidEnumValue, argumentName, enumType));
            }
        }

        /// <summary>
        /// Checks an enum instance to ensure that its value is defined by the specified enum type.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value to check.</param>
        /// <param name="argumentName">The name of the argument holding the value.</param>
        /// <remarks>
        /// This method does not currently support Flags enums.
        /// The constraint on the method should be updated to "enum" once the C# compiler supports it.
        /// </remarks>
        public static void EnumValueIsDefined<T>(T enumValue, string argumentName) where T : struct
        {
            if (!typeof(T).IsEnumDefined(enumValue))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidEnumValue, argumentName, typeof(T)));
            }
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignee">The argument type.</param>
        /// <param name="providedType">The type it must be assignable from.</param>
        /// <param name="argumentName">The argument name.</param>
        public static void TypeIsAssignableFromType(Type assignee, Type providedType, string argumentName)
        {
            Guard.ArgumentNotNull(assignee, "assignee");
            Guard.ArgumentNotNull(providedType, "providedType");

            if (!providedType.IsAssignableFrom(assignee))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, ExceptionMessages.TypeNotCompatible, assignee, providedType), argumentName);
            }
        }

        /// <summary>
        /// Checks an argument to ensure it value is valid region index 0 or 1
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentIsValidRegion(int argumentValue, string argumentName)
        {
            Guard.ArgumentInRange<int>(argumentValue, 0, 1, argumentName);
        }

        /// <summary>
        /// Checks an argument to ensure it value is valid hex digit (lower or upper case)
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentIsHexDigit(char argumentValue, string argumentName)
        {
            if (!Uri.IsHexDigit(argumentValue))
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.ArgumentCannotBeOutOfRange, argumentName, "0", "f"));
            }
        }

        private static readonly Regex s_sha1regex = new Regex("^[0-9a-f]{40}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool IsValidSha1(string sha)
        {
            return string.IsNullOrEmpty(sha) ? false : s_sha1regex.IsMatch(sha.ToLower());
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException in case specified sha1 is invalid (null, empty, too long, etc.)
        /// This function will do case-sensitive matching.
        /// </summary>
        public static void ArgumentIsSha1(string argumentValue, string argumentName)
        {
            if (!IsValidSha1(argumentValue))
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidSha, argumentName, "1"));
            }
        }

        private static readonly Regex s_sha256regex = new Regex("^[0-9a-f]{64}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool IsValidSha256(string sha)
        {
            // Regular expression assumes all lower case letters.
            return string.IsNullOrEmpty(sha) ? false : s_sha256regex.IsMatch(sha.ToLower());
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException in case specified sha256 is invalid (null, empty, too long, etc.)
        /// This function will do case-sensitive matching.
        /// </summary>
        public static void ArgumentIsSha256(string argumentValue, string argumentName)
        {
            if (!IsValidSha256(argumentValue))
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidSha, argumentName, "256"));
            }
        }

        private static readonly Regex s_md5regex = new Regex("^[0-9a-f]{32}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool IsValidMd5(string md5)
        {
            // Regular expression assumes all lower case letters.
            return string.IsNullOrEmpty(md5) ? false : s_md5regex.IsMatch(md5.ToLower());
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException in case specified md5 is invalid (null, empty, too long, etc.)
        /// This function will do case-sensitive matching.
        /// </summary>
        public static void ArgumentIsMd5(string argumentValue, string argumentName)
        {
            if (!IsValidMd5(argumentValue))
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidMd5));
            }
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException in case specified sha is not sha1 and not sha256 (null, empty, too long, etc.)
        /// This function will do case-sensitive matching.
        /// </summary>
        public static void ArgumentIsSha1OrSha256(string argumentValue, string argumentName)
        {
            if (string.IsNullOrEmpty(argumentValue) || (!s_sha1regex.IsMatch(argumentValue) && !s_sha256regex.IsMatch(argumentValue)))
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidSha, argumentName, "1/256"));
            }
        }

        public static bool IsValidSha(ShaType shaType, string sha)
        {
            switch (shaType)
            {
                case ShaType.Sha1:
                    return IsValidSha1(sha);
                case ShaType.Sha256:
                    return IsValidSha256(sha);
                default:
                    throw new NotImplementedException("Invalid sha type");
            }
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException in case specified sha is not sha1 and not sha256 (null, empty, too long, etc.)
        /// This function will do case-sensitive matching.
        /// </summary>
        public static void ArgumentIsSha(ShaType shaType, string argumentValue, string argumentName)
        {
            switch (shaType)
            {
                case ShaType.Sha1:
                    ArgumentIsSha1(argumentValue, argumentName);
                    break;
                case ShaType.Sha256:
                    ArgumentIsSha256(argumentValue, argumentName);
                    break;
                default:
                    throw new NotImplementedException("Invalid sha type");
            }
        }

        private static readonly Regex s_sha1PrefixRegex = new Regex("^[0-9a-f]{6,39}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex s_sha256PrefixRegex = new Regex("^[0-9a-f]{6,63}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Throws ArgumentOutOfRangeException in case specified sha prefix is invalid (null, empty, too long, too short, etc.)
        /// This function will do case-sensitive matching.
        /// </summary>
        public static void ArgumentIsShaPrefix(ShaType shaType, string argumentValue, string argumentName)
        {
            if (string.IsNullOrEmpty(argumentValue) || ((shaType == ShaType.Sha1) && !s_sha1PrefixRegex.IsMatch(argumentValue)) || ((shaType == ShaType.Sha256) && !s_sha256PrefixRegex.IsMatch(argumentValue)))
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidShaPrefix, argumentName, shaType));
            }
        }

        private static readonly Regex s_ctphRegex = new Regex(@"^\d{1,16}:[0-9A-Za-z+/]{0,128}:[0-9A-Za-z+/]{0,64}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool IsValidCtph(string ctph)
        {
            return !string.IsNullOrWhiteSpace(ctph) && s_ctphRegex.IsMatch(ctph);
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException in case specified sha256 is invalid (null, empty, too long, etc.)
        /// This function will do case-sensitive matching.
        /// </summary>
        public static void ArgumentIsCtph(string argumentValue, string argumentName)
        {
            if (!IsValidCtph(argumentValue))
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, String.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidCtph, argumentName));
            }
        }

        #region Private methods
        /// <summary>
        /// Determines whether the specified value is the default value for its type.
        /// </summary>
        /// <typeparam name="T">The type of the value to be checked.</typeparam>
        /// <param name="value">The value to be checked.</param>
        /// <returns><c>true</c> if the given value is the default value for its type.; otherwise, <c>false</c>.</returns>
        private static bool IsDefaultValue<T>(T value)
        {
            return Object.Equals(value, default(T));
        }
        #endregion

        #region Nested types
        /// <summary>
        /// This attribute class tells Code Analysis (FxCop) that a method validates that a parameter is not null
        /// </summary>
        internal sealed class ValidatedNotNullAttribute : Attribute
        {
        }
        #endregion
    }
}
