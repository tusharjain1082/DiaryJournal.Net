namespace MarkupConverter;

internal struct Utf16StringReader(string input)
{
    private int currentOffset = 0;

    private static int ConvertToUtf32(char leadingSurrogate, char trailingSurrogate)
    {
        return (leadingSurrogate - 55296) * 1024 + (trailingSurrogate - 56320) + 65536;
    }

    private static bool IsValidUnicodeScalarValue(int codePoint)
    {
        if (0 <= codePoint && codePoint <= 55295)
            return true;
        if (57344 <= codePoint)
            return codePoint <= 1114111;
        return false;
    }

    public int ReadNextScalarValue()
    {
        if(currentOffset >= input.Length)
        {
            return -1;
        }
        var readInput = input;
        var readOffset = currentOffset;
        currentOffset = readOffset + 1;
        var index = readOffset;
        var ch1 = readInput[index];
        var codePoint = (int)ch1;
        if (char.IsHighSurrogate(ch1) && currentOffset < input.Length)
        {
            var ch2 = input[currentOffset];
            if (char.IsLowSurrogate(ch2))
            {
                currentOffset = currentOffset + 1;
                codePoint = ConvertToUtf32(ch1, ch2);
            }
        }
        if (IsValidUnicodeScalarValue(codePoint))
        {
            return codePoint;
        }
        return 65533;
    }
}