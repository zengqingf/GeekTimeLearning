/**
 * QuickSort
 */
#include <vector>
using namespace std;

/*
https://leetcode-cn.com/problems/sort-an-array/solution/kuai-pai-de-zui-hao-ji-zui-hao-yong-de-xie-fa-by-t/
*/
class Solution {

public:

	int partition(vector<int>& nums, int start, int end) {
		int index = ( rand() % ( end - start + 1 ) ) + start;
		swap( nums[start], nums[index] );
		int pivot = nums[start];
		int left = start,  right = end;
		while (left != right) {
			while (left < right && nums[right] > pivot) --right;
			while (left < right && nums[left] <= pivot) ++left;
			if (left < right) swap(nums[left], nums[right]);
		}
		swap( nums[start], nums[left] );
		return left;
	}

	void quickSort(vector<int>& nums, int start, int end) {
		if (start == end) return;
		if (start < end) {
			int index = partition(nums, start, end);
			quickSort(nums, start, index - 1);
			quickSort(nums, index + 1, end);
		}
	}

	vector<int> sortArray(vector<int>& nums) {
		if (nums.size() == 0) return nums;
		int size = nums.size() - 1;
		quickSort(nums, 0, size);
		return nums;
	}


public:

	void QuickSort(vector<int>& nums, int left, int right)
	{
		if (left >= right)
			return;

		int i = left; 
		int j = right;
		int pv = nums[left];

		while (i < j)
		{
			while (i < j && nums[j] >= pv)
				--j;
			if (i < j)
			{
				nums[i] = nums[j];
				++i;
			}
			while (i < j && nums[i] < pv)
				++i;
			if (i < j)
			{
				nums[j] = nums[i];
				--j;
			}
		}
		nums[i] = pv;
		QuickSort(nums, left, i - 1);
		QuickSort(nums, i + 1, right);
	}
};