using UnityEngine;

namespace WaveCaveGames.Utilities{
	
	public class ArrayUtility {
		//array utilities
		public static int Size<T>(T[] array){
			return array.Length;
		}
		public static void IncreaseArray<T>(ref T[] array, T item){
			var original = array;
			array = new T[original.Length + 1];
			for (int i = 0; i < original.Length; i++) array[i] = original[i];
			array[original.Length] = item;
		}
		public static void IncreaseArray<T>(ref T[] array, T item, int itemIndex){
			if (itemIndex < 0) {
				Debug.LogError("[ArrayUtility] array index can't be negative!");
				return;
			}
			if (itemIndex > array.Length) {
				Debug.LogError("[ArrayUtility] index " + itemIndex + " in array \"" + array + "\" is out of range!");
				return;
			}
			var original = array;
			array = new T[original.Length + 1];
			bool isItemAdded = false;
			for (int i = 0; i < original.Length + 1; i++) {
				if (i == itemIndex) {
					array[itemIndex] = item;
					isItemAdded = true;
				} else array[i] = original[isItemAdded ? i - 1 : i];
			}
		}
		public static void IncreaseArray<T>(ref T[] array, params T[] items){
			var original = array;
			array = new T[original.Length + items.Length];
			for (int i = 0; i < original.Length; i++) array[i] = original[i];
			for (int i = 0; i < items.Length; i++) array[i + original.Length] = items[i];
		}
		public static void IncreaseArrayAvoidNull<T>(ref T[] array, T item){
			if (item == null) return;
			IncreaseArray(ref array, item);
		}
		public static void IncreaseArrayAvoidNull<T>(ref T[] array, T item, int itemIndex){
			if (item == null) return;
			IncreaseArray(ref array, item, itemIndex);
		}
		public static void DecreaseArray<T>(ref T[] array, int index){
			if (index < 0) {
				Debug.LogError("[ArrayUtility] array index can't be negative!");
				return;
			}
			if (index >= array.Length) {
				Debug.LogError("[ArrayUtility] index " + index + " in array \"" + array + "\" is out of range!");
				return;
			}
			var original = array;
			array = new T[original.Length - 1];
			for (int i = 0; i < array.Length; i++) array[i] = original[(i >= index) ? i + 1 : i];
		}
		public static void DecreaseArray<T>(ref T[] array, int from, int to){
			if (from > to) {
				Debug.LogError("[ArrayUtility] can't decrease array because from number is greater than to number!");
				return;
			}
			if (from < 0 || to < 0) {
				Debug.LogError("[ArrayUtility] array index can't be negative!");
				return;
			}
			if (from >= array.Length || to >= array.Length) {
				Debug.LogError("[ArrayUtility] can't decrease array because either from or to index in array \"" + array + "\" is out of range!");
				return;
			}
			var original = array;
			array = new T[original.Length - to + from - 1];
			for (int i = 0; i < array.Length; i++) array[i] = original[(i >= from) ? i + to - from + 1: i];
		}
		public static void DecreaseArrayObject<T>(ref T[] array, T item){
			DecreaseArray(ref array, ObjectToIndex(array, item));
		}
		public static void DecreaseArrayObject<T>(ref T[] array, T from, T to){
			DecreaseArray(ref array, ObjectToIndex(array, from), ObjectToIndex(array, to));
		}
		public static void RemoveAll<T>(ref T[] array, T item){
			for (int i = array.Length - 1; i >= 0; i--) {
				if (Equals(array[i], item)) DecreaseArray(ref array, i);
			}
		}
		public static void Resize<T>(ref T[] array, int size){
			if (size == array.Length) return;
			if (size < 0) {
				Debug.LogError("[ArrayUtility] array size can't be negative!");
				return;
			}
			if (size == 0) {
				array = new T[0];
				return;
			}
			if (size < array.Length) {
				DecreaseArray(ref array, size, array.Length - 1);
				return;
			}
			//greater than array length
			var original = array;
			array = new T[size];
			for (int i = 0; i < original.Length; i++) array[i] = original[i];
			if (original.Length != 0) {
				for (int i = original.Length; i < size; i++) array[i] = original[original.Length - 1];
			}
		}
		public static void ClearArray<T>(ref T[] array){
			array = new T[0];
		}
		public static void RemoveEmptyItems<T>(ref T[] array){
			for (int i = array.Length - 1; i >= 0; i--) {
				if (Equals(array[i], null)) DecreaseArray(ref array, i);
				else if (Equals(array[i].ToString(), "null")) DecreaseArray(ref array, i);
			}
		}
		public static void RemoveSameItems<T>(ref T[] array){
			for (int i = 0; i < array.Length; i++) {
				for (int n = array.Length - 1; n > i; n--) {
					if (Equals(array[i], array[n])) DecreaseArray(ref array, n);
				}
			}
		}
		public static int ObjectToIndex<T>(T[] array, T item){
			for (int i = 0; i < array.Length; i++) {
				if (Equals(array[i], item)) return i;
			}
			Debug.LogError("[ArrayUtility] can't find item \"" + item + "\" in array \"" + array + "\" !");
			return -1;
		}
		public static string ArrayItems<T>(T[] array){
			string s = "";
			for (int i = 0; i < array.Length; i++) {
				s += ((i == 0) ? "" : ", ") + ((array[i] == null) ? "null" : array[i].ToString());
			}
			return s;
		}
		public static string ArrayItemsLayouted<T>(T[] array){
			string s = "";
			for (int i = 0; i < array.Length; i++) {
				s += ((i == 0) ? "" : "\n") + "Element " + i.ToString() + ": " + ((array[i] == null) ? "null" : array[i].ToString());
			}
			return s;
		}
	}
}
