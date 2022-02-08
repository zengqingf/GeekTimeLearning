# 数据结构和算法3 - Geektime

* 常用排序算法 

  * 内部排序（使用内存）

    插入排序（直接插入排序、希尔排序）

    选择排序（简单选择排序、堆排序）

    交换排序（冒泡排序、快速排序）

    归并排序
  
    基数排序
  
  * 外部排序（内存外存结合使用）





* 复杂度分析

  | 排序方式 | 时间复杂度  |             |             | 空间复杂度 | 稳定性 | 复杂性 |
  | -------- | ----------- | ----------- | ----------- | ---------- | ------ | ------ |
  |          | 平均        | 最坏        | 最好        |            |        |        |
  | 插入排序 | O(n^2^)     | O(n^2^)     | O(n)        | O(1)       | 稳定   | 简单   |
  | 希尔排序 | O(n^3/2^)   |             |             | O(1)       | 不稳定 | 较复杂 |
  | 冒泡排序 | O(n^2^)     | O(n^2^)     | O(n)        | O(1)       | 稳定   | 简单   |
  | 快速排序 | O(nlog~2~n) | O(n^2^)     | O(nlog~2~n) | O(log~2~n) | 不稳定 | 较复杂 |
  | 选择排序 | O(n^2^)     | O(n^2^)     | O(n^2^)     | O(1)       | 不稳定 | 简单   |
  | 堆排序   | O(nlog~2~n) | O(nlog~2~n) | O(nlog~2~n) | O(1)       | 不稳定 | 较复杂 |
  | 归并排序 | O(nlog~2~n) | O(nlog~2~n) | O(nlog~2~n) | O(n)       | 稳定   | 较复杂 |
  | 基数排序 | O(d(n+rd))  | O(d(n+rd))  | O(d(n+rd))  | O(rd)      | 稳定   | 较复杂 |

  

* 说明

  * 稳定性

    ``` tex
    假定待排序的记录序列中，存在多个具有相同关键字的记录，如果经过排序，这些记录相对次序保持不变，则算法是稳定的
    
    衡量排序算法的重要考量要素
    ```

  * 注意点

    ``` tex
    1. 排序算法无绝对优劣
    	需要根据排序元素进行选择
    2. 快排
    	快排相对于堆排序和归并排序，最好情况下，渐进时间复杂度与堆排序和归并排序相同，但快排的常量系数较小
    3. 工程使用
    	a. 综合使用排序
    	b. 数组较小时，常使用插入排序
    	c. 数组较大时，常使用快排，或者其他 O(nlogn)
    ```

* 常考面试题

  1. 按照访问次数对10万条URL访问日志进行排序
  2. 两个字符串数组，每个数组有10万条字符串，如何快速找出两个数组中相同的字符串
  3. 有两个包含1亿行字符串的文件，如何快速找出相同的字符串



---



* 冒泡排序

  * 原理

    在要排序的一组数中，对当前还未排好序的范围内的全部数，自上而下对相邻的两个数依次进行比较和调整，让较大的数往下沉，较小的往上冒

    每当两相邻的数比较后发现它们的排序与排序要求相反时，就将它们互换

  * 复杂度

    时间：O(n^2^)

    空间：O(1)

  * 稳定性

    稳定

  * 实现
  
    * java
  
      ``` java
      public static void BubbleSort(int [] arr){
      
           int temp;//临时变量
           for(int i=0; i<arr.length-1; i++){   //表示趟数，一共arr.length-1次。
               for(int j=arr.length-1; j>i; j--){
      
                   if(arr[j] < arr[j-1]){
                       temp = arr[j];
                       arr[j] = arr[j-1];
                       arr[j-1] = temp;
                   }
               }
           }
       }
      
      //优化
      public static void BubbleSort1(int [] arr){
      
         int temp;//临时变量
         boolean flag;//是否交换的标志
         for(int i=0; i<arr.length-1; i++){   //表示趟数，一共 arr.length-1 次
      
             // 每次遍历标志位都要先置为false，才能判断后面的元素是否发生了交换
             flag = false;
             
             for(int j=arr.length-1; j>i; j--){ //选出该趟排序的最大值往后移动
      
                 if(arr[j] < arr[j-1]){
                     temp = arr[j];
                     arr[j] = arr[j-1];
                     arr[j-1] = temp;
                     flag = true;    //只要有发生了交换，flag就置为true
                 }
             }
             // 判断标志位是否为false，如果为false，说明后面的元素已经有序，就直接return
             if(!flag) break;
         }
      }
      ```
  
      





---



* 选择排序

  * 原理

    ``` tex
    1. 在要排序的一组数中，选出最小（或者最大）的一个数与第 1 个位置的数交换
    2. 然后在剩下的数当中再找最小（或者最大）的与第 2 个位置的数交换
    3. 依次类推，直到第 n - 1 个元素（倒数第二个数）和第 n 个元素（最后一个数）比较为止
    ```
  
  * 复杂度
  
    时间：O(n^2^)
  
    空间：O(1)
  
  * 稳定性
  
    不稳定
  
  * 实现
  
    java
  
    ``` java
    public static void SelectSort(int array[],int length){
    
       for(int i=0;i<length-1;i++){
    
           int minIndex = i;
           for(int j=i+1;j<length;j++){
              if(array[j]<array[minIndex]){
                  minIndex = j;
              }
           }
           if(minIndex != i){
               int temp = array[i];
               array[i] = array[minIndex];
               array[minIndex] = temp;
           }
       }
    }
    ```
  
    



---



* 插入排序

  * 原理

    ``` tex
    将一个记录插入到已排序好的有序表中，得到一个记录数增1的有序表
    即先将序列的第1个记录看成是一个有序的子序列，然后从第2个记录逐个进行插入，直到整个序列有序为止
    
    @注意：设立哨兵，作为临时存储和判断数组边界
    ```

  * 复杂度

    时间：O(n^2^)

    空间：O(1)

  * 稳定性

    稳定

  * 实现

    java

    ``` java
    public static void InsertSort(int array[],int length){
    
       int temp;
       for(int i=0;i<length-1;i++){
           for(int j=i+1;j>0;j--){
               if(array[j] < array[j-1]){
                   temp = array[j-1];
                   array[j-1] = array[j];
                   array[j] = temp;
               }else{         //不需要交换
                   break;
               }
           }
       }
    }
    ```
  
    



---



* 希尔排序

  * 原理

    ``` tex
    又称缩小增量排序
    先将整个待排序的记录序列分割成若干子序列，分别进行直接插入排序，待整个序列中的记录“基本有序”时，再对全体记录进行一次直接插入排序
    
    过程：
    1. 选择一个增量序列t1, t2，...，tk   其中ti > tj   tk = 1
    2. 按照步骤1的增量序列，对序列进行k趟排序
    3. 每趟排序，根据对应的增量ti，将待排序列分割成若干长度为m的子序列，分别对各子表进行直接插入排序
    	仅增量因子为1时，整个序列作为一个表来处理，表长度即为整个序列的长度
    ```

  * 复杂度

    时间：O(n^1.3^)

    空间：O(1)

  * 稳定性

    不稳定

  * 实现

    java

    ``` java
    public static void shell_sort(int array[],int length){
    
       int temp = 0;
       int incre = length;
    
       while(true){
           incre = incre/2;
    
           for(int k = 0; k<incre; k++){    //根据增量分为若干子序列
    
               //for(int i = k + incre; i<length; i+=incre){
               for(int i = k + incre; i< length; i++) {
                   //for(int j = i; j > k; j -= incre){
                   for(int j = i; j >= k; j -= incre) {
                       //if(array[j] < array[j-incre]){
                       if(j - incre >= k && arr[j] < arr[j - incre]){
                           temp = array[j-incre];
                           array[j-incre] = array[j];
                           array[j] = temp;
                       }else{
                           break;
                       }
                   }
               }
           }
    
           if(incre == 1){
               break;
           }
       }
    }
    ```
  
    



---



* 快速排序

  * 原理

    ``` tex
    1. 选择一个基准元素，通常选择第一个元素或者最后一个元素
    2. 通过一趟排序将待排序的记录分割成独立的两部分，其中一部分记录的元素值均比基准元素值小，另一部分记录的元素值比基准值大
    3. 此时基准元素在其排好序后的正确位置
    4. 然后分别对这两部分记录用同样的方法继续进行排序，直到整个序列有序
    ```
  
    分治，挖坑填数
  
  * 复杂度
  
    时间：O(nlog~2~n)
  
    空间：O(nlog~2~n)  需要递归调用 占用栈空间
  
  * 稳定性
  
    不稳定
  
  * 实现
  
    java
  
    ``` java
    public static void QuickSort(int a[],int l,int r){
         if(l>=r)
           return;
    
         int i = l; int j = r; int key = a[l];//选择第一个数为key //key还可以取中间数或者随机数，会影响算法复杂度
    
         while(i<j){
    
             while(i<j && a[j]>=key)//从右向左找第一个小于key的值
                 j--;
             if(i<j){
                 a[i] = a[j];
                 i++;
             }
    
             while(i<j && a[i]<key)//从左向右找第一个大于key的值
                 i++;
    
             if(i<j){
                 a[j] = a[i];
                 j--;
             }
         }
         //i == j
         a[i] = key;
         QuickSort(a, l, i-1);//递归调用
         QuickSort(a, i+1, r);//递归调用
     }
    ```
  
    



---



* 归并排序

  * 原理

    将两个（或两个以上）有序表合并成一个新的有序表，即把待排序序列分为若干个子序列，每个子序列都是有序的，然后再把有序子序列合并为整体有序序列

  * 复杂度

    时间：O(nlog~2~n)

    空间：O(n)

  * 稳定性

    稳定

  * 实现

    java

    ``` java
    public static void MergeSort(int a[],int first,int last,int temp[]){
    
      if(first < last){
          int middle = (first + last)/2;
          MergeSort(a,first,middle,temp);//左半部分排好序
          MergeSort(a,middle+1,last,temp);//右半部分排好序
          MergeArray(a,first,middle,last,temp); //合并左右部分
      }
    }
    
    //合并 ：将两个序列a[first-middle],a[middle+1-end]合并
    public static void MergeArray(int a[],int first,int middle,int end,int temp[]){    
      int i = first;
      int m = middle;
      int j = middle+1;
      int n = end;
      int k = 0;
      while(i<=m && j<=n){
          if(a[i] <= a[j]){
              temp[k] = a[i];
              k++;
              i++;
          }else{
              temp[k] = a[j];
              k++;
              j++;
          }
      }    
      while(i<=m){
          temp[k] = a[i];
          k++;
          i++;
      }    
      while(j<=n){
          temp[k] = a[j];
          k++;
          j++;
      }
    
      for(int ii=0;ii<k;ii++){
          a[first + ii] = temp[ii];
      }
    }
    ```

    



---



* 堆排序

  * 原理

  * 复杂度

    时间：O(nlog~2~n)

    空间：O(1)

  * 稳定性

    不稳定

  * 实现

    java

    ``` java
    //构建最小堆
    public static void MakeMinHeap(int a[], int n){
     for(int i=(n-1)/2 ; i>=0 ; i--){
         MinHeapFixdown(a,i,n);
     }
    }
    
    //从i节点开始调整,n为节点总数 从0开始计算 i节点的子节点为 2*i+1, 2*i+2  
    public static void MinHeapFixdown(int a[],int i,int n){
    
       int j = 2*i+1; //子节点
       int temp = 0;
    
       while(j<n){
           //在左右子节点中寻找最小的
           if(j+1<n && a[j+1]<a[j]){  
               j++;
           }
    
           if(a[i] <= a[j])
               break;
    
           //较大节点下移
           temp = a[i];
           a[i] = a[j];
           a[j] = temp;
    
           i = j;
           j = 2*i+1;
       }
    }
    
    public static void MinHeapSort(int a[],int n){
      int temp = 0;
      MakeMinHeap(a,n);
    
      for(int i=n-1;i>0;i--){
          temp = a[0];
          a[0] = a[i];
          a[i] = temp;
          MinHeapFixdown(a,0,i);
      }    
    }
    ```
  
    



---





* 基数排序

  * 原理

    属于分配式（distribution sort）
    n个记录的关键字进行排序，每个关键字堪称一个d元组：k=(k~i1~,k~i2~,...,k~id~)
  
    其中c~0~ <= k~ij~ <= C~r-1~ (1 <= i <= n, 1 <= j <= d)
  
    排序时先按k~id~的值，从小到大将记录分到r (称为基数)个盒子中，再依次收集；
  
    然后按 k~id-1~ 的值再重复，直至按k~i1~分配和收集序列为止，排序结束
  
    在关键字为数字时，r=10, 0 << c~i~ <= 9, 1 <= i <= d
  
    在关键字为字母时，r=26, ‘A’ << c~i~ <= 'Z', 1  <= i <= d
  
  * 复杂度
  
    时间：O(nlogn)  、 O(d(r + n))     r表示关键字基数，d代表长度，n代表关键字个数
  
    空间：O(rd + n)   
  
  * 稳定性
  
    稳定
  
  * 实现
  
    java
  
    ``` java
    public static void RadixSort(int A[],int temp[],int n,int k,int r,int cnt[]){
    
       //A:原数组
       //temp:临时数组
       //n:序列的数字个数
       //k:最大的位数2
       //r:基数10
       //cnt:存储bin[i]的个数
    
       for(int i=0 , rtok=1; i<k ; i++ ,rtok = rtok*r){
    
           //初始化
           for(int j=0;j<r;j++){
               cnt[j] = 0;
           }
           //计算每个箱子的数字个数
           for(int j=0;j<n;j++){
               cnt[(A[j]/rtok)%r]++;
           }
           //cnt[j]的个数修改为前j个箱子一共有几个数字
           for(int j=1;j<r;j++){
               cnt[j] = cnt[j-1] + cnt[j];
           }
           for(int j = n-1;j>=0;j--){      //重点理解
               cnt[(A[j] / rtok) % r]--;
               temp[cnt[(A[j] / rtok) % r]] = A[j];
           }
           for(int j=0;j<n;j++){
               A[j] = temp[j];
           }
       }
    }
    ```
  
    