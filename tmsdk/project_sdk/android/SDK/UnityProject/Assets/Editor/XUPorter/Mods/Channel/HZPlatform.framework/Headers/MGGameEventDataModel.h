//
//  MGGameEventDataModel.h
//  XYPlatformTest
//
//  Created by ZYZ on 2018/9/7.
//  Copyright © 2018年 xyzs. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface MGGameEventDataModel : NSObject
//上报类型(必传)
@property(nonatomic,strong)NSString *data_type;
//角色id(必传)
@property(nonatomic,strong)NSString *role_id;
//角色名称(必传)
@property(nonatomic,strong)NSString *role_name;
//区服id(必传)
@property(nonatomic,strong)NSString *server_id;
//区服名称(必传)
@property(nonatomic,strong)NSString *server_name;
//角色等级(选传)
@property(nonatomic,strong)NSString *level;
//职业(选传)
@property(nonatomic,strong)NSString *job;
//角色元宝数(选传)
@property(nonatomic,strong)NSString *diamonds;
//聊天内容(选传)
@property(nonatomic,strong)NSString *content;
//聊天类型(选传)
@property(nonatomic,strong)NSString *chat_type;



@end
