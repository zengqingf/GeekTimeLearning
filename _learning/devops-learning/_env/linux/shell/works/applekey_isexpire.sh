#!/bin/bash
:<<!
function build_jenkins_dingding()
{
    sshtarget=$1
    leftspace=$2
    echo Jenkins Build
    java -jar ./jenkins-cli.jar -s http://192.168.2.65:8080/ build DNF_Robot_Send_Error_ToDingding \
    -p Type="CheckDiskSpace" -p DescTitle="检查版苹果证书" -p JenkinsJob=${JOB_NAME} -p JenkinsJobUrl=${BUILD_URL} \
    -p BuildResult="0" -p Desc="${sshtarget}证书在七天内过期了！！！，剩余：${leftspace}" -f
}
!

jenkinsroot=$1
keystoreroot1=$2
keystoreroot2=$3
keystorechannels=$4
keystoretypes=$5

function check_dir_exist()
{
   dir=$1
   if [ ! -d "${dir}" ]; then
 	echo "can not find dir path : ${dir} !!!"
	return 1
   else 
	return 0
   fi
}

function check_file_exist()
{
   file=$1
   if [ ! -f "${file}" ]; then
	echo "can not find file path : ${file} !!!"
	return 1
   else
	return 0
   fi
}

function check_ioskey_isexpire()
{
    echo "------------------------------------------解析p12证书----------------------------------------------------"
    #获取UID的值
   APPUID=`openssl pkcs12 -in "$1" -nodes -passin pass:"$3" | openssl x509 -noout -subject | sed 's/\(.*\)\/UID=\(.*\)\/CN=\(.*\)/\2/g'`
   echo "p12证书UID = $APPUID"

   #打印证书有效期
   EndDate=`openssl pkcs12 -in "$1" -nodes -passin pass:"$3" | openssl x509 -noout -enddate | cut -b 10-`
   EndDate2=`openssl pkcs12 -in "$1" -nodes -passin pass:"$3" | openssl x509 -noout -dates`
   echo "p12证书有效期至：${EndDate}"
   if [[ "$SystemVersion" == *Darwin* ]]; then
	#mac系统
	   LocalEndDateString=`date -j -f "%b %d %T %Y %Z" "$EndDate" +"%Y-%m-%d %T"`
	   LocalEndDate=`date -j -f "%b %d %T %Y %Z" "$EndDate" +"%s"`
	   echo "p12证书有效期：$EndDate" 	
   else
	   #linux 系统
	   LocalEndDateString=`date -d "$EndDate" +"%Y-%m-%d %T"`
	   LocalEndDate=`date -d "$EndDate" +"%s"`
	   echo "p12证书有效期：$LocalEndDateString" 
   fi

   NowDate=`date +"%s"`
   echo "当前时间：${NowDate}"
   echo "p12证书有效期：${LocalEndDate}"  
   if [[ $NowDate -gt $LocalEndDate ]]; then
	   echo "！！！！！！警告！！！！！！"
	   echo "该p12证书已经过期"
	   return
   fi
   LeftTime=`expr $LocalEndDate - $NowDate`
   NeedDealTime=`expr 60 \* 60 \* 24 \* 7`
   if [[ $NeedDealTime -gt $LeftTime ]]; then
	   echo "！！！！！！注意！！！！！！"
	   echo "该p12证书七天内会过期"
	   return
   else
       echo "证书七天内不会过期"    
   fi
}

function verify()
{
	p12file=""
	mpfile=""
	passfile=""
	check_dir_exist $1
	if [ x"$?" != x"0" ]; then
		exit 1
	fi
	echo "find for verifing ..."
	for file in $1/*
	do
		echo "verify file : ${file}"
		if [ ! -r ${file} ]; then
			echo "123456" | sudo -S chmod +r ${file}
		fi
		extension=$(echo ${file} | cut -d . -f2)
		echo extension is ${extension}
		if [ x"${extension}" == x"p12" ]; then
			p12file=$file
		elif [ x"${extension}" == x"mobileprovision" ]; then
			mpfile=$file
		elif [ x"${extension}" == x"pass" ]; then
			passfile=${file}
		fi
	done

	if [[ ${p12file} == "" || ${mpfile} == "" ]]; then
		echo "p12 or mobileprovision is not found"
		exit 1
	fi

	echo "p12 file : ${p12file}"
	echo "mp file : ${mpfile}"
	
	pwd=`cat ${passfile}`
	if [ -z "${pwd}" ]; then
		echo "pwd is empty !"		
	fi
	
	echo "start verify ..."
	check_ioskey_isexpire ${p12file} ${mpfile} "${pwd}"
}

keystorechannelarr=${keystorechannels//,/ }
keystoretypearr=${keystoretypes//,/ }
for channel in ${keystorechannelarr}
do
	channelpath=${keystoreroot2}/${channel}
	for type in ${keystoretypearr}
	do
		keystorepath=${channelpath}/${type}
		echo "#####################################  Start Verify  #########################################"
		echo "channel path : ${keystorepath} !!!"
		check_dir_exist ${keystorepath}
		if [ x"$?" != x"0" ]; then
			continue 1
		fi		
		echo "excute to verify ..."		
		verify ${keystorepath} ${keystoreroot1}/${channelpath}
		echo "#####################################  End Verify #############################################" 
	done
done
