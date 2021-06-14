# 搭建个人blog

### 工具

[hugo](https://gohugo.io/)

[hexo](https://hexo.io/zh-cn/docs/)



[npm中文文档](https://www.npmjs.cn/)



---



### Hexo + Github 

* link

  [使用hexo+github搭建免费个人博客详细教程](http://blog.haoji.me/build-blog-website-by-hexo-github.html)

* npm

  ``` sh
  npm install -g hexo
  npm uninstall -g hexo
  
  cd new_hexp_path
  npm install hexo-deployer-git --save
  ```

  

* hexo

  ``` sh
  mkdir new_hexo_path & cd new_hexo_path
  hexo init #创建目录并进入，初始化生成hexo的配置等文件
  hexo init 项目名 #作为根目录
  
  hexo s -p 4100  #指定端口启动hexo服务（本地预览）
  
  hexo clean #异常问题、清理内容
  
  hexo g #生成、重新生成
  
  
  #@注意：hexo上传到git时，会先删除之前提交，注意备份
  hexo d
  ```

  ``` sh
  hexo new "postName" #新建文章
  hexo new page "pageName" #新建页面
  hexo generate #生成静态页面至public目录
  hexo server #开启预览访问端口（默认端口4000，'ctrl + c'关闭server）
  hexo deploy #部署到GitHub
  hexo help  # 查看帮助
  hexo version  #查看Hexo的版本
  
  #缩写
  hexo n == hexo new
  hexo g == hexo generate
  hexo s == hexo server
  hexo d == hexo deploy
  
  #组合
  hexo s -g #生成并本地预览
  hexo d -g #生成并上传
  ```

  ``` sh
  #上传md源文件到 source分支
  
  git init
  git checkout -b source
  git add -A
  git commit -m "init blog"
  git remote add origin git@github.com:{username}/{username}.github.io.git
  git push origin source
  
  #作者：程序员吴师兄
  #链接：https://www.zhihu.com/question/23934523/answer/1882886859
  #来源：知乎
  #著作权归作者所有。商业转载请联系作者获得授权，非商业转载请注明出处。
  
  
  #配置文件 _config.yml
  
  #修改支持中文
  language: zh-CN
  
  
  ```

  





---



### 内容

* 小工具
  * 日历
  * 推荐点赞
  * 评论

