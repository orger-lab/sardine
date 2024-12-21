using System.Data;
using System.Text;

namespace Sardine.Core.Utils.Text
{
    /// <summary>
    /// Helper methods for Sardine.
    /// </summary>
    public static class StringOperations
    {
        static readonly string[] charsToSanitize = [@"#", @"%", @"&", @"{", @"}", @"\", @"<", @">", ".", @"*", @"?", @"/", @"$", @"!", @"'", @"""", @":", @"@", @"+", @"`", @"|", @"="];
        private static readonly string[] separators = ["_", " ", "."];

        public static string SanitizeFileName(string str)
        {
            ArgumentNullException.ThrowIfNull(str);

            return string.Join(string.Empty, str.Split(charsToSanitize, StringSplitOptions.RemoveEmptyEntries));
        }
        
        public static string ToTitleCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            var words = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            

            words = words
                .Select(word => char.ToUpper(word[0], System.Globalization.CultureInfo.InvariantCulture) + word.Substring(1))
                .ToArray();

            return string.Join(string.Empty, words);
        }

        public static string SwitchSeparators(string str, string? newSeparator = null)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            var words = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(newSeparator ?? string.Empty, words);
        }

        public static string GenerateNameWithSpacesFromCamelCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            System.Text.StringBuilder newText = new(text.Length * 2);

            _ = newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    {
                        _ = newText.Append(' ');
                    }
                }

                _ = newText.Append(text[i]);
            }

            return newText.ToString().Replace('_', ' ');
        }

        public static string RandomString(int length)
        {
            Random random = new();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
#pragma warning disable CA5394 // Do not use insecure randomness
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
#pragma warning restore CA5394 // Do not use insecure randomness
        }

        public static string FilterToNCharacters(string inString, int maxChars, bool showTooLargeIdentifier = false)
        {
            if (inString is null)
                return string.Empty;

            if (inString.Length <= maxChars)
                return inString;

            if (showTooLargeIdentifier)
                return inString[..(maxChars - 3)] + "...";

            return inString[..maxChars];
        }


        /// <summary>Separates a capitalized text into multiple words separated by spaces.</summary>
        /// <param name="text">Text to be separeated.</param>
        /// <param name="preserveAcronyms">Keep consecutive capital letters togheter to form acronyms.</param>
        /// <returns>A string that is the variable text separeted by spaces.</returns>
        public static string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            StringBuilder newText = new(text.Length * 2);

            _ = newText.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1]))))
                {
                    _ = newText.Append(' ');
                }
                _ = newText.Append(text[i]);

            }
            return newText.ToString();
        }
    }
}
