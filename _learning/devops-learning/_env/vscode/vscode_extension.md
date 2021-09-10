# VSCode 插件

* Remote SSH

  [配置Visual Studio Code的Remote - SSH插件进行远程开发](https://note.qidong.name/2019/08/vscode-remote-ssh/)



* plantuml

  UML画图




---



* 头部注释

  配置路径： “File”—“Preferences”—“User Snippets”

  ```  json
  {
  	// Place your snippets for python here. Each snippet is defined under a snippet name and has a prefix, body and 
  	// description. The prefix is what is used to trigger the snippet and the body will be expanded and inserted. Possible variables are:
  	// $1, $2 for tab stops, $0 for the final cursor position, and ${1:label}, ${2:another} for placeholders. Placeholders with the 
  	// same ids are connected.
  	// Example:
  	// "Print to console": {
  	// 	"prefix": "log",
  	// 	"body": [
  	// 		"console.log('$1');",
  	// 		"$2"
  	// 	],
  	// 	"description": "Log output to console"
  	// }
  
  	"HEADER": {
  		"prefix" : "header",
  		"body": [
              "#!/usr/bin/env python",
              "# -*- encoding: utf-8 -*-",
              "'''",
  			"@File           : $TM_FILENAME",
              "@Time           : $CURRENT_YEAR/$CURRENT_MONTH/$CURRENT_DATE $CURRENT_HOUR:$CURRENT_MINUTE:$CURRENT_SECOND",
  			"@Author         : mjx",
  			"@Version        : 1.0.0",
  			"@Desc           : ",
              "'''",
  			"",
  			"",
  			"$0"
  		],
  	}
  }
  ```
  
  ``` json
  {
  	// Place your snippets for shellscript here. Each snippet is defined under a snippet name and has a prefix, body and 
  	// description. The prefix is what is used to trigger the snippet and the body will be expanded and inserted. Possible variables are:
  	// $1, $2 for tab stops, $0 for the final cursor position, and ${1:label}, ${2:another} for placeholders. Placeholders with the 
  	// same ids are connected.
  	// Example:
  	// "Print to console": {
  	// 	"prefix": "log",
  	// 	"body": [
  	// 		"console.log('$1');",
  	// 		"$2"
  	// 	],
  	// 	"description": "Log output to console"
  	// }
  
  	"HEADER": {
  		"prefix" : "header",
  		"body": [
              "#!/bin/bash ",
              "###################################################################",
  			"#File           : $TM_FILENAME",
              "#Time           : $CURRENT_YEAR/$CURRENT_MONTH/$CURRENT_DATE $CURRENT_HOUR:$CURRENT_MINUTE:$CURRENT_SECOND",
  			"#Author         : mjx",
  			"#Version        : 1.0.0",
  			"#Desc           : ",
              "###################################################################",
  			"",
  			"",
  			"$0"
  		],
  	}
  }
  ```
  
  



---



### 配置

* windows

  ``` tex
  系统命令识别不到：无法将"xx"项识别为cmdlet...
  
  解决方案：vscode快捷方式 -> 兼容性 -> 以管理员身份运行
  设置后需要关闭所有打开的vscode窗口后重启
  ```

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202109081106648.jpg)



