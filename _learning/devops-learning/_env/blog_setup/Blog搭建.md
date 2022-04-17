# 搭建个人blog

### 工具

[hugo](https://gohugo.io/)

[hexo](https://hexo.io/zh-cn/docs/)



[npm中文文档](https://www.npmjs.cn/)



---



### Hexo + Github 

* env

  git + node.js

* link

  [使用hexo+github搭建免费个人博客详细教程](http://blog.haoji.me/build-blog-website-by-hexo-github.html)

* npm

  ``` sh
  ## 如果npm源站下载速度慢，可使用淘宝镜像
  npm config set registry "https://registry.npm.taobao.org"
  
  npm install -g hexo  	#直接选择
  npm install -g hexo-cli #其他选择
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
  hexo new <layout> "postName" #新建文章  
      hexo new page "pageName" #新建页面
      hexo new posts "new article" #添加新文章	默认posts     
      							 #对应在posts或drafts文件夹 添加新文件.md
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
  hexo s --debug   #debug方式运行
  
  #hexo生成博文插入图片
  1.修改站点配置文件_config.yml 里的post_asset_folder:这个选项设置为true
  2.再运行hexo n "xxxx"来生成md博文时，/source/_posts文件夹内除了xxxx.md文件还有一个同名的文件夹
  	./XXX文件夹
  	./XXX.md
  3.引用图片语法：{% asset_img 图片名.jpg 你想输入的替代文字 %}
  
  选择安装 hexo-renderer-marked 插件
  1. npm install hexo-renderer-marked
  2. 修改 _config.yaml
  	post_asset_folder: true
      marked:
        prependRoot: true
        postAsset: true
  3.引用图片语法为标准md格式： ![你想输入的替代文字](图片名.jpg)
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
  
  
  #自定义git邮箱和用户名
  git config --global user.email "you@example.com"
  git config --global user.name "Your Name"
  
  #------------------------------------------------------------------------
  
  #创建github pages
  #https://docs.github.com/cn/pages/getting-started-with-github-pages/creating-a-github-pages-site
  
  #安装发布插件
  #在站点目录中运行命令行输入npm install hexo-deployer-git --save
  
  #本地目录绑定github，即SSH认证
  #命令行输入ssh-keygen -t rsa -C "邮箱地址"， 这里要输入之前注册Github时的邮箱，例如我之前注册用的是 example@163.com，那命令行就输入ssh-keygen -t rsa -C "example@163.com"输入后一直回车。
  #打开C:\Users\用户名，文件夹内寻找.ssh文件夹
  #打开Github 点击右上角的头像 Settings 选择SSH and GPG keys
  #点击New SSH key 将之前复制的内容粘帖到Key的框中。 上面的title 可以随意 点击add 完成添加。
  #此时回到命令行。 试一下是否跟Github连接成功。命令行输入ssh -T git@github.com ，弹出的内容输入yes，看到出现Hi <account name>! You've successfully authenticated, but GitHub doesnot provide shell access. 说明链接成功。此处这个<account name>应该是你Github的用户名。
  
  #修改hexo站点配置
  #进入博客文件夹，找到_config.yml文件。 这个是博客配置文件，后面的修改会多次用到它
  #修改一下title跟url等，url修改为https://<用户名>.github.io，例如我的用户名是example的话，就应该输入https://example.github.io
  #拉到文件最底部，在deploy下面添加一个repo项 一个branch项。填入如下代码：
  	type: git			#git方式
      repo: git@github.com:<Github用户名>/<github用户名>.github.io.git   #git地址
      branch: master		#git分支，注意格式: 后有一个空格
  
  ```
  
  ``` tex
  hexo仓库文件夹作用
  
  config.yml
  博客的配置文件，博客的名称、关键词、作者、语言、博客主题...设置都在里面。
  
  package.json
  应用程序信息，新添加的插件内容也会出现在这里面，我们可以不修改这里的内容。
  
  scaffolds
  scaffolds就是脚手架的意思，这里放了三个模板文件，分别是新添加博客文章（posts）、新添加博客页（page）和新添加草稿（draft）的目标样式。
  这部分可以修改的内容是，我们可以在模板上添加比如categories等自定义内容
  
  source
  source是放置我们博客内容的地方，也就是资源文件夹，里面初始只有一个文件夹，_posts（文章文件夹），之后我们通过命令新建tags（标签文件夹）还有categories（分类）页后，这里会相应地增加文件夹。
  
  themes
  放置主题文件包的地方。Hexo会根据这个文件来生成静态页面。
  ```
  
  ``` sh
  #使用主题
  
  #1.下载主题到非站点根目录下，放到themes/next
  	cd hexo本地文件夹
  	git clone https://github.com/theme-next/hexo-theme-next.git themes/next
  	
  #2.配置启用主题
  	打开 站点配置文件(即站点目录下的_config.yml文件)， 找到 theme 字段，并将其值更改为 next。
  	theme: next
  	
  #3.验证
  	hexo clean #清除hexo缓存
  	hexo s --debug #调试启动，观察日志，打开本地url: http://localhost:4000
  	
  #4. 修改nexT提供的scheme，调整为不同的外观
          #Muse - 默认 Scheme，这是 NexT 最初的版本，黑白主调，大量留白
          #Mist - Muse 的紧凑版本，整洁有序的单栏外观
          #Pisces - 双栏 Scheme，小家碧玉似的清新
     #scheme目录：/themes/next/_config.yml
     
  #5. 设置语言，修改站点配置文件及主题配置文件：language
  #站点根路径: `XXX`
  #站点配置文件路径  `XXX\_config.yml`
  #主题配置文件路径  `XXX\themes\next\_config.yml`
  	#修改zh-Hans 或 zh-CN
  	#themes/next/languages 确定文件
  	
  #6. 设置头像 avatar
  	#完整的互联网 URI	http://example.com/avatar.png
  	#站点内的地址	将头像放置主题目录下的 source/uploads/ （新建 uploads 目录若不存在） 
  	#配置为：avatar: /uploads/avatar.png 或者 
  			#放置在 source/images/ 目录下 配置为：avatar: /images/avatar.png
  
  #7. 添加分类
  	menu:
  		home: / || home #主页
  		tags: /tags/ || tags #标签页
  		categories: /categories/ || th #分类页
  		archives: /archives/ || archive #归档页
  		about: /about/ || user #关于页面
  		读书: /books || book			#新增
  		电影: /movies || film			#新增
  		
  	#添加tags categories  
  	#网站根目录下面的source文件夹会分别生成tags、categories以及about文件夹。
  	hexo new page 'name' # name分别为tags、categories  
  	hexo new page 'tags' #创建tags子目录
  	hexo new page 'categories' #创建categories子目录
  	
  	#分别修改这tags和categories文件夹中的index.md文件，新增type属性，如下：
  	---
      title: 标签
      date: 2019-01-14 20:56:48
      type: "tags" #新添加的内容
      ---
      ---
      title: 文章分类
      date: 2019-01-14 20:53:04
      type: "categories"   #这部分是新添加的
      ---
      
      #添加模板  hexo n test.md
      #scarffolds文件夹里的post.md文件，给它的头部加上categories:，
      #这样我们创建的所有新的文章都会自带这个属性，我们只需要往里填分类，就可以自动在网站上形成分类了
      title: {{ title }}
      date: {{ date }}
      categories:
      tags:
      
      #当你新建一篇博文的时候(我们写的博文到创建到站点目录/source/_posts下面)，
      #增加上tags和categories属性值，就能在tags和categories界面检索到你的文章了。
      #第一种是类似数组的写法，把标签放在中括号[]里，用英文逗号隔开
  	#第二种写法是用-短划线列出来
  	
  	#source/_posts/vim-node.md
  	---
      layout: posts
      title: vim-node.md
      date: 2019-01-14 22:23:43
      categories: 学习笔记
      tags: [vi, vim]
      ---
      
      # vim 笔记
      ## 01 学习 vi 的目的
      此处省略一堆 
      这是 很多 很多 很多 内容
      
      #source/_posts/Hello World.md
      ---
      layout: posts
      title: Hello World.md
      date: 2019-01-14 22:23:43
      categories: helloworld
      tags: [hello, world]
      ---
  
      这是 helloworld 文件的内容 后面省略一大堆内容
  
  #8. 个性化定制
      添加fork me on github 在博客的左上角或者右上角
      在http://tholman.com/github-cor...://github.com/blog/273-github-ribbons选择合适的样式复制代码到themes/next/layout/_layout.swig，在<div class="headband"></div>下面：
      注意：须手动输入style放置自己想要的位置
      <!-- GitHub-start -->
          <a href="https://github.com/bd3star" target="_blank">
            <img style="position: absolute; top: 0; right: 0; border: 0;"
             src="https://s3.amazonaws.com/github/ribbons/forkme_right_gray_6d6d6d.png" alt="Fork me on GitHub">
          </a>
      <!-- GitHub-end -->
  
  #9. 动态背景
      目前NexT主题最新的是V6+版本，这个版本中可以有4种动态背景：
          canvas-nest
          three_waves
          canvas_lines
          canvas_sphere
      # 切换到主题路径下
      $ cd themes/next
      # 注意 后面  下载到 主题next路径下的 source/lib/canvas-nest 文件夹里面
      $ git clone https://github.com/theme-next/theme-next-canvas-nest source/lib/canvas-nest
      #创建footer.swig文件 在 /source/_data目录下
      #编辑文件footer.swig
      <script type="text/javascript" color="0,0,255" opacity='0.7' zIndex="-2" count="99" src="dist/canvas-nest.js"></script>
  	#在NexT中_config.yml，取消注释footer此custom_file_path部分。
  	custom_file_path:
    		footer: source/_data/footer.swig
    	# 清理缓存
      $ hexo clean
      # 编译
      $ hexo g
      # 启动
      $ hexo s 
      # 部署到 github 上
      # $ hexo d
      
  #10. 侧边栏社交小图标设置
  	#打开主题配置文件_config.yml，搜索social:, ||之后是在图标库中对应的图标。注意空格就行。
  	social:
  		Github: http://xxx || github
  		E-Mail: xxx@xxx.com || envelope
  		Weibo: https://weibo.com || weibo
  	#图标库链接：http://fontawesome.io/icons/
  	
  #11. 设置网站图标
  	#默认的网站图标是一个N，当然是需要制定一个图了，
  	#在网上找到图后，将其放在/themes/next/source/images里面，然后在主题配置文件中修改下图所示图片位置
  ```





* hugo

  





---



### Markdown

ref: 

[markdown-intro](https://mazhuang.org/markdown-intro/)

[Learning-Markdown (Markdown 入门参考)](http://xianbai.me/learn-md/index.html)



* 生成目录

  ``` markdown
  方法1
  开头输入 [toc]
  凡是文章标题带有#（1-6个）的都会被捕获到目录中
  
  方法2
  npm安装doctoc
  npm i doctoc -g   //install 简写为 i
  
  xxx/demo.md
  cd xxx/
  doctoc demo.md
  ```

  





---



### 内容

* 小工具
  * 日历
  * 推荐点赞
  * 评论

