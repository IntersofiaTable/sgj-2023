using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests.Editor
{
    public static class AssertExtender
    {
        public static void Collection<T>(IEnumerable<T> collection, params Action<T>[] actions)
        {
            Assert.AreEqual(actions.Length, collection.Count());
            int index = 0;
            foreach (var item in collection)
            {
                actions[index](item);
                index++;
            }
        }
    }
}