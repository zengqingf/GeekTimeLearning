#include <vector>
#include <unordered_map>
#include <unordered_set>
using std::vector;
using std::unordered_map;
using std::unordered_multimap;
using std::unordered_multiset;

/*
link: https://leetcode-cn.com/problems/two-sum/


*/
class Solution {
public:
	//解法一
	vector<int> SumTwoNum(const vector<int>& nums, int target)
	{
		//这里用unodered_multimap也可以
		unordered_map<int, int> numMap;
		for (int i = 0; i < nums.size(); ++i)
		{
			numMap.emplace(i, nums[i]);
		}
		for (int i = 0; i < nums.size(); ++i)
		{
			int res = target - nums[i];
			for (auto iter = numMap.begin(); iter != numMap.end(); ++iter) {
				if (iter->first == i) {
					continue;
				}
				if (iter->second == res) {
					return { i,  iter->first };
				}
			}
		}
		return {};
	}

	vector<int> SumTwoNum2(const vector<int>& nums, int target)
	{
		for (int i = 0;  i < nums.size() - 1; ++i)
		{
			for (int j = i + 1; j < nums.size(); ++j) 
			{
				if (nums[i] + nums[j] == target)
				{
					return { i, j };
				}
			}
		}
		return {};
	}

	vector<int> SumTwoNum3(const vector<int>& nums, int target)
	{
		unordered_map<int, int> numMap;
		for (int i = 0; i < nums.size(); ++i)
		{
			int res = target - nums[i];
			if (numMap.find(res) != numMap.end()) {
				return { i, numMap[res] };
			}
			numMap[nums[i]] = i;
		}

		return {};
	}


	/*
	hash function
	散列、哈希

	散列函数（哈希函数）
	
	1.作用：把元素键值映射为数组下标，将数据存储在数组中对应下标位置
	按照键值查询元素时，用同样的散列函数，将键值转化数组下标，从对应数组下标的位置取数据

	2.特征：
	散列函数计算得到的散列值是一个非负整数；
	如果 key1 = key2，那 hash(key1) == hash(key2)；
	如果 key1 ≠ key2，那 hash(key1) ≠ hash(key2)。
	*/
};