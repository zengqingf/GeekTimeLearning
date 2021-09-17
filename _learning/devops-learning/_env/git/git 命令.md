# git 命令

link:

[常用 Git 命令清单](http://www.ruanyifeng.com/blog/2015/12/git-cheat-sheet.html)

[Pro Git（中文版）](https://git.oschina.net/progit/)

[Git Community Book 中文版](http://gitbook.liuhui998.com/index.html)

[Git 教學](http://gogojimmy.net/2012/01/17/how-to-use-git-1-git-basic/)

[猴子都能懂得git](https://backlog.com/git-tutorial/cn/)

[git - 简明指南](http://rogerdudler.github.io/git-guide/index.zh.html)



* git revert

  ``` tex
  git revert 撤销 某次操作，此次操作之前和之后的commit和history都会保留，并且把这次撤销
  作为一次最新的提交
      * git revert HEAD                撤销前一次 commit
      * git revert HEAD^               撤销前前一次 commit
      * git revert commit （比如：fa042ce57ebbe5bb9c8db709f719cec2c58ee7ff）撤销指定的版本，撤销也会作为一次提交进行保存。
  git revert是提交一个新的版本，将需要revert的版本的内容再反向修改回去，
  版本会递增，不影响之前提交的内容
  ```

* git revert vs. git reset

  ``` tex
  1. git revert是用一次新的commit来回滚之前的commit，git reset是直接删除指定的commit。 
  2. 在回滚这一操作上看，效果差不多。但是在日后继续merge以前的老版本时有区别。因为git revert是用一次逆向的commit“中和”之前的提交，因此日后合并老的branch时，导致这部分改变不会再次出现，但是git reset是之间把某些commit在某个branch上删除，因而和老的branch再次merge时，这些被回滚的commit应该还会被引入。 
  3. git reset 是把HEAD向后移动了一下，而git revert是HEAD继续前进，只是新的commit的内容和要revert的内容正好相反，能够抵消要被revert的内容。
  ```

  