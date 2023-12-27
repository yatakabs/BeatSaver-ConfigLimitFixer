using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Shouldly;
using Xunit;
using T = System.String;

namespace ConfigLimitFixer.Tests;

public static class EnumerableExtensionsTests
{
    /// <summary>
    /// Tests Chunk() method properly works with different length of source and chunk size.
    /// </summary>
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(0, 2)]
    [InlineData(1, 2)]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    [InlineData(0, 3)]
    [InlineData(1, 3)]
    [InlineData(2, 3)]
    [InlineData(3, 3)]
    [InlineData(4, 3)]
    public static void ChunkShouldReturnCorrectNumberOfChunks(
        int sourceLength,
        int chunkSize)
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var source = fixture.CreateMany<T>(sourceLength);

        // Act
        var chunks = source.Chunk(chunkSize);

        // Assert
        var expectedNumberOfChunks = (int)Math.Ceiling((double)sourceLength / chunkSize);

        chunks.ShouldNotBeNull();
        chunks.Count().ShouldBe(expectedNumberOfChunks);
        chunks.ShouldAllBe(x => x.Length <= chunkSize);
    }

    /// <summary>
    /// Tests Chunk() method properly works with empty source.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public static void ChunkShouldReturnEmptySequenceWhenSourceIsEmpty(
        int chunkSize)
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var source = fixture.CreateMany<T>(0);

        // Act
        var chunks = source.Chunk(chunkSize);

        // Assert
        chunks.ShouldNotBeNull();
        chunks.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests Chunk() method throws for null source.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public static void ChunkShouldThrowWhenSourceIsNull(
        int chunkSize)
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var source = default(IEnumerable<T>);

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
        {
            return source.Chunk(chunkSize);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.ParamName.ShouldBe("source");
    }

    /// <summary>
    /// Tests Chunk() method throws for invalid chunk size.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public static void ChunkShouldThrowWhenChunkSizeIsInvalid(
        int chunkSize)
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var source = fixture.CreateMany<T>(1);

        // Act
        var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            return source.Chunk(chunkSize);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.ParamName.ShouldBe("chunkSize");
    }
}
