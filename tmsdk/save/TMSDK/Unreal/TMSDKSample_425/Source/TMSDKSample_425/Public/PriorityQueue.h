#pragma once

template <class T>
class PriorityQueue
{
public:
	PriorityQueue<T>(std::function<bool(T&, T&)> compare)
	{
		_compare = compare;
	}

	bool IsEmpty()
	{
		return _container.empty();
	}

	void Push(T obj)
	{
		_container.push_back(obj);
		_heapifyUp(_container.size() - 1);
	}

	void RebulidElement(T obj) 
	{
		int size = _container.size();
		for (int i = 0; i < size; ++i) {
			T other = _container[i];
			if (other == obj) {
				int newIdx = _heapifyUp(i);
				if (i == newIdx) {
					_heapifyDown(i);
					return;
				}
			}
		}
	}

	void Pop()
	{
		if (!_container.empty())
		{
			_container[0] = _container[_container.size() - 1];
			_container.pop_back();
			if (!_container.empty())
			{
				_heapifyDown(0);
			}
		}
	}

	T& Top()
	{
		assert(!_container.empty());
		return _container[0];
	}

	void Clear() {
		_container.clear();
	}

private:
	std::function<bool(T&, T&)> _compare;
	std::vector<T> _container;

	//向上堆化
	int _heapifyUp(int idx)
	{
		int parentIdx = parent(idx);
		if (parentIdx < 0)
			return idx;
		if (_compare(_container[parentIdx], _container[idx]))
		{
			std::swap(_container[parentIdx], _container[idx]);
			idx = _heapifyUp(parentIdx);
		}
		return idx;
	}

	//向下堆化
	int _heapifyDown(int idx)
	{
		int largeIdx = idx;
		int leftChildIdx = leftChild(idx), rightChildIdx = rightChild(idx);
		if (leftChildIdx < _container.size())
		{
			if (_compare(_container[largeIdx], _container[leftChildIdx]))
				largeIdx = leftChildIdx;
		}
		if (rightChildIdx < _container.size())
		{
			if (_compare(_container[largeIdx], _container[rightChildIdx]))
				largeIdx = rightChildIdx;
		}

		if (largeIdx != idx)
		{
			std::swap(_container[largeIdx], _container[idx]);
			idx = _heapifyDown(largeIdx);
		}
		return idx;
	}

	int leftChild(int i)
	{
		return 2 * i + 1;			//左子树结点 从1开始，获取在数组中的序号
	}

	int rightChild(int i)
	{
		return 2 * i + 2;
	}

	int parent(int i)
	{
		return (i - 1) / 2;		  //根节点从 0 开始
	}
};