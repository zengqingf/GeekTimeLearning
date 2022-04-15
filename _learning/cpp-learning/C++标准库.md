### C++  STL

* 序列容器

* 关联容器

  * map

    ``` tex
    1、在map中，由key查找value时，首先要判断map中是否包含key。
    
    2、如果不检查，直接返回map[key]，可能会出现意想不到的行为。如果map包含key，没有问题，如果map不包含key，使用下标有一个危险的副作用，会在map中插入一个key的元素，value取默认值，返回value。也就是说，map[key]不可能返回null。
    
    3、map提供了两种方式，查看是否包含key，m.count(key)，m.find(key)。
    
    4、m.count(key)：由于map不包含重复的key，因此m.count(key)取值为0，或者1，表示是否包含。
    
    5、m.find(key)：返回迭代器，判断是否存在。
    
    6.对于STL中的容器，有全局泛型算法find(begin，end，target)查找目标，map还提供了一个成员方法find(key)；如果容器自身提供了find方法，就是用它
    ```

    ``` c++
    //4. 需要执行两次查找
    if(m.count(key)>0)
    {
         return m[key];
    }
    return null;
    
    //5. 只需要执行一次查找
    iter = m.find(key);
    if(iter!=m.end())
    {
        return iter->second;
    }
    return null;
    ```

    