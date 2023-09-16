using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
#pragma warning disable IDE0008

namespace StardewModdingAPI.Toolkit.Extensions;

[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
internal static class CollectionExt
{
    #region Any

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any(this Array array)
    {
        return array.Length != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this T[] array)
    {
        return array.Length != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this List<T> list)
    {
        return list.Count != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this IList<T> list)
    {
        return list.Count != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this IReadOnlyList<T> list)
    {
        return list.Count != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any(this Queue queue)
    {
        return queue.Count != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this Queue<T> queue)
    {
        return queue.Count != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any(this ICollection collection)
    {
        return collection.Count != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this ICollection<T> collection)
    {
        return collection.Count != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this IReadOnlyCollection<T> collection)
    {
        return collection.Count != 0;
    }

    #endregion Any

    #region Any (Predicated)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any(this Array array, Func<object, bool> predicate)
    {
        foreach (var item in array)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this T[] array, Func<T, bool> predicate)
    {
        foreach (var item in array)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this List<T> list, Func<T, bool> predicate)
    {
        foreach (var item in list)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this IList<T> list, Func<T, bool> predicate)
    {
        foreach (var item in list)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
    {
        foreach (var item in list)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any(this Queue queue, Func<object, bool> predicate)
    {
        foreach (var item in queue)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this Queue<T> queue, Func<T, bool> predicate)
    {
        foreach (var item in queue)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any(this ICollection collection, Func<object, bool> predicate)
    {
        foreach (var item in collection)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        foreach (var item in collection)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Any<T>(this IReadOnlyCollection<T> collection, Func<T, bool> predicate)
    {
        foreach (var item in collection)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region All (Predicate)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All(this Array array, Func<object, bool> predicate)
    {
        foreach (var item in array)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All<T>(this T[] array, Func<T, bool> predicate)
    {
        foreach (var item in array)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All<T>(this List<T> list, Func<T, bool> predicate)
    {
        foreach (var item in list)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All<T>(this IList<T> list, Func<T, bool> predicate)
    {
        foreach (var item in list)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
    {
        foreach (var item in list)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All(this Queue queue, Func<object, bool> predicate)
    {
        foreach (var item in queue)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All<T>(this Queue<T> queue, Func<T, bool> predicate)
    {
        foreach (var item in queue)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All(this ICollection collection, Func<object, bool> predicate)
    {
        foreach (var item in collection)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        foreach (var item in collection)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool All<T>(this IReadOnlyCollection<T> collection, Func<T, bool> predicate)
    {
        foreach (var item in collection)
        {
            if (!predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}
