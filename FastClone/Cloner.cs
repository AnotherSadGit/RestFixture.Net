using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FastClone.Internal;

/*  Copyright 2014 Michael Sander (Ymris)
 *
 *  This file is part of FastClone, a library for performing fast deep 
 *  copies of .NET objects.  The original source code is found at 
 *  <http://fastclone.codeplex.com/>.
 *
 *  FastClone is free software:
 *  You can redistribute it and/or modify it under the terms of the
 *  Microsoft Public License (Ms-PL) as published by Microsoft Corporation,
 *  either version 1 of the License, or (at your option) any later version.
 *
 *  FastClone is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  Microsoft Public License (Ms-PL) for more details.
 *
 *  You should have received a copy of the Microsoft Public License (Ms-PL)
 *  along with FastClone.  If not, see 
 *  <https://msdn.microsoft.com/en-us/library/ff647676.aspx>.
 */

namespace FastClone
{
    /// <summary>
    /// Performs a fast clone (deep copy) of a .NET object.
    /// </summary>
    public static class Cloner
    {
        static readonly ConcurrentDictionary<Type, Func<object, Dictionary<object, object>, object>> _TypeCloners = new ConcurrentDictionary<Type, Func<object, Dictionary<object, object>, object>>();

        /// <summary>
        /// Creates a deep clone.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T Clone<T>(T original)
        {
            Func<object, Dictionary<object, object>, object> creator = GetTypeCloner(typeof(T));
            return (T)creator(original, new Dictionary<object, object>());
        }

        static Func<object, Dictionary<object, object>, object> GetTypeCloner(Type type) { return _TypeCloners.GetOrAdd(type, t => new CloneExpressionBuilder(t).CreateTypeCloner()); }
    }
}

