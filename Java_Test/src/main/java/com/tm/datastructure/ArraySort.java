package com.tm.datastructure;

import java.util.Arrays;
import java.util.Comparator;

public class ArraySort {

    /*
    * 冒泡排序的特点是，每一轮循环后，最大的一个数被交换到末尾，因此，下一轮循环就可以“刨除”最后的数，每一轮循环都比上一轮循环的结束位置靠前一位。

      必须注意，对数组排序实际上修改了数组本身。
    * */
    public static void bubbleSort() {
        int[] ns = {28, 12, 89, 73, 65, 18, 96, 50, 8, 36};
        System.out.println(Arrays.toString(ns));

        for (int i = 0; i < ns.length - 1; i++) {
            boolean flag = true;
            for (int j = 0; j < ns.length - i - 1; j++) {
                if (ns[j] > ns[j + 1]) {
                    // 交换ns[j]和ns[j+1]:
                    int tmp = ns[j];
                    ns[j] = ns[j + 1];
                    ns[j + 1] = tmp;
                    flag = false;
                }
            }
            if(flag)
                break;
        }

        // 排序后:
        System.out.println(Arrays.toString(ns));
    }

    //必须注意，对数组排序实际上修改了数组本身。
    /*
     * 例：
     * int[] ns = { 9, 3, 6, 5 };
     *
     * Array.sort(ns);
     *
     * 变为 {3, 5, 6, 9}
     *
     * 例：
     * String[] ns = { "banana", "apple", "pear" };
     *
     * Array.sort(ns);
     *
     * 变为 {"apple" , "banana", "pear"}
     * */


    //降序排序
    public static void descendingBubbleSort() {
        int[] ns = { 28, 12, 89, 73, 65, 18, 96, 50, 8, 36 };
        // 排序前:
        System.out.println(Arrays.toString(ns));

        for (int i = 0; i < ns.length - 1; i++) {
            boolean flag = true;
            for (int j = 0; j < ns.length - i - 1; j++) {
                if (ns[j] < ns[j + 1]) {
                    // 交换ns[j]和ns[j+1]:
                    int tmp = ns[j];
                    ns[j] = ns[j + 1];
                    ns[j + 1] = tmp;
                    flag = false;
                }
            }
            if(flag)
                break;
        }

        // 排序后:
        System.out.println(Arrays.toString(ns));
    }

    //内置Array.sort扩展
    public static void test_array_sort() {
        //降序
        Integer[] a = {9, 8, 7, 2, 3, 4, 1, 0, 6, 5};
        //定义一个自定义类MyComparator的对象
        Comparator cmp = new MyComparator();
        Arrays.sort(a,cmp);
        for(int arr:a) {
            System.out.print(arr + " ");
        }
    }
}

//内置Array.sort扩展
//实现Comparator接口
class MyComparator implements Comparator<Integer> {
    @Override
    public int compare(Integer o1, Integer o2) {
    		        /*如果o1小于o2，我们就返回正值，如果o1大于o2我们就返回负值，
    		         这样颠倒一下，就可以实现降序排序了,反之即可自定义升序排序了*/
        return o2-o1;
    }
}
