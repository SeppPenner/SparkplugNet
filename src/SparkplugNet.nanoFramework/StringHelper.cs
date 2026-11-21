namespace SparkplugNet.NanoFramework
{
    internal static class StringHelper
    {
        internal static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null) return true;

            // Match the Unicode White_Space characters used by .NET's string.IsNullOrWhiteSpace.
            var length = value.Length;
            for (var i = 0; i < length; i++)
            {
                var character = value[i];
                if ((character >= '\t' && character <= '\r') ||
                    (character >= '\u2000' && character <= '\u200A')) continue;

                switch (character)
                {
                    case ' ':
                    case '\u0085':
                    case '\u00A0':
                    case '\u1680':
                    case '\u2028':
                    case '\u2029':
                    case '\u202F':
                    case '\u205F':
                    case '\u3000':
                        continue;
                    default:
                        return false;
                }
            }

            return true;
        }
    }
}
