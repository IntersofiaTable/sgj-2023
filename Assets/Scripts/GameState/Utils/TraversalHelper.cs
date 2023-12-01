using System;
using System.Collections.Generic;

public static class TraversalHelper
{
    public static HashSet<T> TraverseBFS<T>(T node, Func<T, IEnumerable<T>> neighboursFunc, Predicate<T> condition)
    {
        var toVisit = new Queue<T>();
        var visited = new HashSet<T>();
        var result = new HashSet<T>();
        return TraverseBFSInternal(node, neighboursFunc, condition, ref toVisit, ref visited, ref result, true);
    }

    private static HashSet<T> TraverseBFSInternal<T>(T node, Func<T, IEnumerable<T>> neighboursFunc, Predicate<T> condition, ref Queue<T> toVisit, ref HashSet<T> visited, ref HashSet<T> result, bool isRoot = false)
    {
        visited.Add(node);
        if (condition.Invoke(node)) { result.Add(node); }
        var newNeighbours = neighboursFunc.Invoke(node);
        foreach (var item in newNeighbours)
        {
            if (visited.Contains(item)) continue;
            toVisit.Enqueue(item);
        }
        if (isRoot)
        {
            while (toVisit.Count > 0)
            {
                TraverseBFSInternal(toVisit.Dequeue(), neighboursFunc, condition, ref toVisit, ref visited, ref result);
            }
        }
        return result;
    }
}
