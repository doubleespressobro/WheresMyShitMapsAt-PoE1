using System.Collections.Generic;
using WheresMyShitMapsAt.Types;

namespace WheresMyShitMapsAt.Cache;

public sealed class HighlightCache
{
    private Dictionary<long, MapHighlightInfo> _activeCache;
    private Dictionary<long, MapHighlightInfo> _updatingCache;
    private readonly object _swapLock = new();

    public HighlightCache()
    {
        _activeCache = new Dictionary<long, MapHighlightInfo>();
        _updatingCache = new Dictionary<long, MapHighlightInfo>();
    }

    public void Update(Dictionary<long, MapHighlightInfo> newHighlights)
    {
        _updatingCache.Clear();
        foreach (var kvp in newHighlights)
        {
            _updatingCache[kvp.Key] = kvp.Value;
        }

        lock (_swapLock)
        {
            (_activeCache, _updatingCache) = (_updatingCache, _activeCache);
        }
    }

    public IReadOnlyDictionary<long, MapHighlightInfo> GetCurrentHighlights()
    {
        lock (_swapLock)
        {
            return new Dictionary<long, MapHighlightInfo>(_activeCache);
        }
    }

    public void Clear()
    {
        lock (_swapLock)
        {
            _activeCache.Clear();
            _updatingCache.Clear();
        }
    }
}