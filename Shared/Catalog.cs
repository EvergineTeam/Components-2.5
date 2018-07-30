// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;

#endregion

namespace WaveEngine.Components
{
    /// <summary>
    /// Class that holds a catalog of items organized by their types.
    /// </summary>
    public static class Catalog
    {
        /// <summary>
        /// Hold the generic values.
        /// </summary>
        private static Dictionary<Type, object> items = new Dictionary<Type, object>();

        #region Public Methods

        /// <summary>
        /// Registers an item.
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <param name="item">The item to register.</param>
        /// <exception cref="System.ArgumentNullException">If item is null.</exception>
        public static void RegisterItem<T>(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item cannot be null.");
            }

            items[item.GetType()] = item;
        }

        /// <summary>
        /// Gets an item.
        /// </summary>
        /// <typeparam name="T">Type of the item.</typeparam>
        /// <returns>The registered item, or the default value for its type in case it was not found.</returns>
        public static T GetItem<T>()
        {
            Type type = typeof(T);

            // Search
            if (items.ContainsKey(type))
            {
                return (T)items[type];
            }

            return default(T);
        }
        #endregion
    }
}
