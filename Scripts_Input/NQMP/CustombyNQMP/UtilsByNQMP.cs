using System.Runtime.CompilerServices;
using System;
using System.Buffers;
using System.Collections.Generic;
public static class MathFast
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Abs(int value) => (value ^ (value >> 31)) - (value >> 31);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sqr(int v) => v * v;
}
public readonly ref struct SpanList<T>
{
    private readonly ReadOnlySpan<T> _span;
    public SpanList(ReadOnlySpan<T> span) => _span = span;
    public int Count => _span.Length;
    public ref readonly T this[int index] => ref _span[index];
    public ReadOnlySpan<T>.Enumerator GetEnumerator() => _span.GetEnumerator();
}

public sealed class ObjectPool<T> where T : class, new()
{
    private readonly ArrayPool<T> _pool = ArrayPool<T>.Shared;
    private T[] _buffer;
    private int _count;

    public ObjectPool(int capacity = 64)
    {
        _buffer = _pool.Rent(capacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get()
    {
        if (_count == 0) return new T();
        var obj = _buffer[--_count];
        _buffer[_count] = null;
        return obj;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Return(T obj)
    {
        if (_count == _buffer.Length)
        {
            var larger = _pool.Rent(_buffer.Length * 2);
            Array.Copy(_buffer, larger, _buffer.Length);
            _pool.Return(_buffer, clearArray: false);
            _buffer = larger;
        }
        _buffer[_count++] = obj;
    }
}

public sealed class SpatialHashGrid<T> where T : IMapObject
{
    private readonly Dictionary<long, List<T>> _cells = new Dictionary<long, List<T>>(1024);
    private readonly int _cellSize;

    public SpatialHashGrid(int cellSize = 64) => _cellSize = cellSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long Hash(int x, int y) => ((long)(x / _cellSize) << 32) | (uint)(y / _cellSize);

    public void Clear() => _cells.Clear();

    public void Insert(T obj)
    {
        var key = Hash(obj.getX(), obj.getY());
        if (!_cells.TryGetValue(key, out var bucket))
            _cells[key] = bucket = new List<T>(4);
        bucket.Add(obj);
    }

    // trả về SpanList<T> để không cấp phát bộ nhớ
    public SpanList<T> Query(int x, int y, int range, Span<T> buffer)
    {
        int minX = (x - range) / _cellSize;
        int maxX = (x + range) / _cellSize;
        int minY = (y - range) / _cellSize;
        int maxY = (y + range) / _cellSize;
        int count = 0;
        for (int ix = minX; ix <= maxX; ix++)
        {
            for (int iy = minY; iy <= maxY; iy++)
            {
                if (_cells.TryGetValue(((long)ix << 32) | (uint)iy, out var bucket))
                {
                    foreach (var obj in bucket)
                    {
                        if (count < buffer.Length)
                            buffer[count++] = obj;
                    }
                }
            }
        }
        return new SpanList<T>(buffer.Slice(0, count));
    }
}