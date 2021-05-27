package com.tm.designpatterns.oop.sample1;

/*

            接口鉴权功能  (只能说是单一模块)

* 1. 需求分析
* 调用方进行接口请求的时候，将 URL、AppID、密码、时间戳拼接在一起，通过加密算法生成 token，并且将 token、AppID、时间戳拼接在 URL 中，一并发送到微服务端。
* 微服务端在接收到调用方的接口请求之后，从请求中拆解出 token、AppID、时间戳。微服务端首先检查传递过来的时间戳跟当前时间，是否在 token 失效时间窗口内。
* 如果已经超过失效时间，那就算接口调用鉴权失败，拒绝接口调用请求。
* 如果 token 验证没有过期失效，微服务端再从自己的存储中，取出 AppID 对应的密码，通过同样的 token 生成算法，生成另外一个 token，与调用方传递过来的 token 进行匹配。
* 如果一致，则鉴权成功，允许接口调用；否则就拒绝接口调用。
*
*   精简：
*   把 URL、AppID、密码、时间戳拼接为一个字符串；
    对字符串通过加密算法加密生成 token；
    将 token、AppID、时间戳拼接到 URL 中，形成新的 URL；
    解析 URL，得到 token、AppID、时间戳等信息；
    从存储中取出 AppID 和对应的密码；
    根据时间戳判断 token 是否过期失效；
    验证两个 token 是否匹配；
*
* */

public class InterfaceAuth {

}


