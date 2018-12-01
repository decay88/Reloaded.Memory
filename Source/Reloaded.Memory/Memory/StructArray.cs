﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Reloaded.Memory
{
    /// <summary>
    /// Utility class for working with struct arrays.
    /// </summary>
    public static unsafe class StructArray
    {
        /* Implementation */

        /// <summary>
        /// Reads a generic type array from a specified memory address.
        /// </summary>
        /// <typeparam name="T">An individual struct type of a class with an explicit StructLayout.LayoutKind attribute.</typeparam>
        /// <param name="memoryAddress">The memory address to read from.</param>
        /// <param name="value">Local variable to receive the read in struct array.</param>
        /// <param name="arrayLength">The number of items to read from memory.</param>
        /// <param name="marshal">Set to true to marshal the element.</param>
        public static void FromPtr<T>(IntPtr memoryAddress, out T[] value, int arrayLength, bool marshal = false)
        {
            int structSize = Struct.GetSize<T>(marshal);
            value = new T[arrayLength];

            for (int x = 0; x < arrayLength; x++)
            {
                IntPtr address = memoryAddress + (structSize * x);
                Struct.FromPtr(address, out T result, marshal);
                value[x] = result;
            }
        }

        /// <summary>
        /// Writes a generic type array to a specified memory address.
        /// </summary>
        /// <typeparam name="T">An individual struct type of a class with an explicit StructLayout.LayoutKind attribute.</typeparam>
        /// <param name="memoryAddress">The memory address to write to.</param>
        /// <param name="item">The item to write to the address.</param>
        /// <param name="marshal">Set this to true in order to marshal the value when writing to memory.</param>
        public static void ToPtr<T>(IntPtr memoryAddress, T[] item, bool marshal = false)
        {
            int structSize = Struct.GetSize<T>(marshal);

            for (int x = 0; x < item.Length; x++)
            {
                IntPtr address = memoryAddress + (structSize * x);
                Struct.ToPtr(address, ref item[x], marshal);
            }
        }

        /// <summary>
        /// Converts a pointer/memory address to a specified structure or class type with explicit StructLayout attribute.
        /// </summary>
        /// <param name="value">Local variable to receive the read in struct array.</param>
        /// <param name="data">A byte array containing data from which to extract a structure from.</param>
        /// <param name="startIndex">The index in the byte array to read the element(s) from.</param>
        /// <param name="marshalElement">Set to true to marshal the element.</param>
        public static void FromArray<T>(byte[] data, out T[] value, int startIndex = 0, bool marshalElement = false)
        {
            int structSize = Struct.GetSize<T>(marshalElement);
            int structureCount = (data.Length - startIndex) / structSize;
            value = new T[structureCount];

            for (int x = 0; x < value.Length; x++)
            {
                int offset = startIndex + (structSize * x);
                Struct.FromArray<T>(data, out T result, offset, marshalElement);
                value[x] = result;
            }
        }

        /// <summary>
        /// Returns the size of a specific primitive or struct type.
        /// </summary>
        /// <param name="marshalElement">If set to true; will return the size of an element after marshalling.</param>
        /// <param name="elementCount">The number of array elements present.</param>
        public static int GetSize<T>(int elementCount, bool marshalElement = false)
        {
            return Struct.GetSize<T>(marshalElement) * elementCount;
        }

        /// <summary>
        /// Creates a byte array from specified structure or class type with explicit StructLayout attribute.
        /// </summary>
        /// <param name="items">The item to convert into a byte array.</param>
        /// <param name="marshalElements">Set to true to marshal the item(s).</param>
        public static byte[] GetBytes<T>(T[] items, bool marshalElements = false)
        {
            int totalSize = GetSize<T>(items.Length);
            List<byte> array = new List<byte>(totalSize);

            for (int x = 0; x < items.Length; x++)
                array.AddRange(Struct.GetBytes(ref items[x]));

            return array.ToArray();
        }
    }
}