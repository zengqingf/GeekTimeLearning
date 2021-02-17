using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSort : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*
    private static void quickWangZheng(int[] a, int p, int r) {
        if (p >= r) {
            return;
        }
        int pivot = a[r];
        int i = p, j = p;
        while (j < r) {
            if (a[j] < pivot) {
                swap(a, i, j); // 交换
                i++;
            }
            j++;
        }
        swap(a, i, j); // 交换
        quickWangZheng(a, p, i - 1);
        quickWangZheng(a, i + 1, r);
    }
*/

    private static void quickSort(int[] a, int head, int tail) {

        int low = head;
        int high = tail;
        int pivot = a[low];
        if (low < high) {

            while (low<high) {
                while (low < high && pivot <= a[high]) high--;
                a[low] = a[high];
                while (low < high && pivot >= a[low]) low++;
                a[high]=a[low];
            }
            a[low] = pivot;

            if(low>head+1) quickSort(a,head,low-1);
            if(high<tail-1) quickSort(a,high+1,tail);
        }
    }
}
