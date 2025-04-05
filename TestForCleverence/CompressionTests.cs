using Cleverence;
namespace TestForCleverence
{
    public class CompressionTests
    {
        [Fact]
        public void EmptyString_ReturnsEmpty()
        {
            Assert.Equal("", CompressionManager.Compress(""));
        }

        [Fact]
        public void SingleCharacter_ReturnsSameCharacter()
        {
            Assert.Equal("a", CompressionManager.Compress("a"));
        }

        [Fact]
        public void AllUniqueCharacters_ReturnsOriginal()
        {
            Assert.Equal("abcd", CompressionManager.Compress("abcd"));
        }

        [Fact]
        public void MixedRepeatsAndUnique_CompressesCorrectly()
        {
            Assert.Equal("a2bc3", CompressionManager.Compress("aabccc"));
        }

        [Fact]
        public void RepeatedGroups_HandlesSeparatedRepeats()
        {
            Assert.Equal("a3b3a2", CompressionManager.Compress("aaabbbaa"));
        }

        [Fact]
        public void NumericCharacters_CompressesDigits()
        {
            Assert.Throws<FormatException>(() => CompressionManager.Compress("111222"));
        }

        [Fact]
        public void CaseSensitive_HandlesDifferentCases()
        {
            Assert.Equal("A3b2C2", CompressionManager.Compress("AAAbbCC"));
        }

        [Fact]
        public void SpecialCharacters_CompressesCorrectly()
        {
            Assert.Throws<FormatException>(() => CompressionManager.Compress("!!!??"));
        }

        [Fact]
        public void LongSequence_HandlesLargeCounts()
        {
            Assert.Equal("a5b5", CompressionManager.Compress("aaaaabbbbb"));
        }

    }
    public class DecompressionTests
    {
        [Fact]
        public void EmptyString_ReturnsEmpty()
        {
            Assert.Equal("", CompressionManager.DeCompress(""));
        }

        [Fact]
        public void SingleCharacter_ReturnsSameCharacter()
        {
            Assert.Equal("a", CompressionManager.DeCompress("a"));
        }

        [Fact]
        public void NoCompressionNeeded_ReturnsOriginal()
        {
            Assert.Equal("abcd", CompressionManager.DeCompress("abcd"));
        }

        [Fact]
        public void MixedCompressedParts_DecompressesCorrectly()
        {
            Assert.Equal("aabccc", CompressionManager.DeCompress("a2bc3"));
        }

        [Fact]
        public void SeparatedRepeatGroups_HandlesCorrectly()
        {
            Assert.Equal("aaabbbaa", CompressionManager.DeCompress("a3b3a2"));
        }

        [Fact]
        public void NumericCharacters_DecompressesDigits()
        {
            Assert.Throws<FormatException>(() => CompressionManager.DeCompress("1323"));
        }

        [Fact]
        public void CaseSensitive_HandlesDifferentCases()
        {
            Assert.Equal("AAAbbCC", CompressionManager.DeCompress("A3b2C2"));
        }

        [Fact]
        public void SpecialCharacters_DecompressesCorrectly()
        {
            Assert.Throws<FormatException>(() => CompressionManager.DeCompress(",1?323"));
        }

        [Fact]
        public void LongCounts_HandlesLargeNumbers()
        {
            Assert.Equal("aaaaabbbbb", CompressionManager.DeCompress("a5b5"));
        }

        [Fact]
        public void SingleLetterWithCount_DecompressesProperly()
        {
            Assert.Equal("zzz", CompressionManager.DeCompress("z3"));
        }
    }

}
