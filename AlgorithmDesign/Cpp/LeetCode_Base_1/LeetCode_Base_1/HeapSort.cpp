/**
 * Heap Sort
 */
#include <vector>
using namespace std;

/*
https://leetcode-cn.com/problems/sort-an-array/solution/kuai-pai-de-zui-hao-ji-zui-hao-yong-de-xie-fa-by-t/
*/
class Solution {

public:

	void MinHeapFixDown(vector<int>& nums, int i, int n)
	{
		int j = 2 * i + 1; //左子节点
		int temp = 0;

		while (j < n)
		{
			//在左右子节点中寻找最小的
			if (j + 1 < n && nums[j + 1] < nums[j])
			{
				++j;
			}

			if (nums[i] <= nums[j])
			{
				break;
			}

			temp = nums[i];
			nums[i] = nums[j];
			nums[j] = temp;

			i = j;
			j = 2 * i + 1;
		}
	}

	void MakeMinHeap(vector<int>& nums, int n)
	{
		for (int i = (n - 1) / 2; i >= 0; --i)
		{
			MinHeapFixDown(nums, i, n);
		}
	}

	void MinHeapSort(vector<int>& nums, int n)
	{
		int temp = 0;
		MakeMinHeap(nums, n);
		for (int i = n - 1; i > 0; --i)
		{
			temp = nums[0];
			nums[0] = nums[i];
			nums[i] = temp;
			MinHeapFixDown(nums, 0, i);
		}
	}
};