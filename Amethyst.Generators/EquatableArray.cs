using System.Collections;
using System.Collections.Immutable;

namespace Amethyst.Generators;

// Immutable arrays compare by reference, which would make every step look changed to the incremental pipeline.
internal readonly struct EquatableArray<T>(ImmutableArray<T> array) : IEquatable<EquatableArray<T>>, IEnumerable<T> where T : IEquatable<T>
{
    private ImmutableArray<T> Array => array.IsDefault ? ImmutableArray<T>.Empty : array;

    public bool IsEmpty => Array.IsEmpty;

    public int Length => Array.Length;

    public T this[int index] => Array[index];

    public ImmutableArray<T>.Enumerator GetEnumerator()
    {
        return Array.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>) Array).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) Array).GetEnumerator();
    }

    public bool Equals(EquatableArray<T> other)
    {
        return Array.SequenceEqual(other.Array);
    }

    public override bool Equals(object? other)
    {
        return other is EquatableArray<T> array && Equals(array);
    }

    public override int GetHashCode()
    {
        var hash = 17;

        foreach (var item in Array)
        {
            hash = hash * 31 + item.GetHashCode();
        }

        return hash;
    }
}