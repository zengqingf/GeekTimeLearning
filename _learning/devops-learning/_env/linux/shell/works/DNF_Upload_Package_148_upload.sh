#!/bin/bash
source /etc/profile

download_url=http://package.srccwl.com

APKROOT="apkroot"

num=1
VERSION=$1
NOW=`date  "+%Y%m%d"`
test -d ${APKROOT}/${NOW}/${VERSION} || mkdir -p ${APKROOT}/${NOW}/${VERSION}
PACKAGE_URL=`echo $2|xargs`
ssh root@119.23.219.229 "test -d /data/baodi/${NOW}/${VERSION} || mkdir -p /data/baodi/${NOW}/${VERSION}"
echo "                                   "
echo "                                   "
echo "                                   "
echo "                                   "
echo "---------------------------------------------------------------------------------------"
echo "---------------------------------------------------------------------------------------"
echo "---------------------------------------------------------------------------------------"
echo "                                   "
for a in ${PACKAGE_URL}
do
	a_name=`echo $a|awk -F"/" '{print $NF}'`
	echo "${download_url}/${NOW}/${VERSION}/${a_name}"
done

echo "                                   "
echo "---------------------------------------------------------------------------------------"
echo "---------------------------------------------------------------------------------------"
echo "---------------------------------------------------------------------------------------"

cd ${APKROOT}/${NOW}/${VERSION}
for i in ${PACKAGE_URL}
do
	echo "$num  download package..."
	b_name=`echo $i|awk -F"/" '{print $NF}'`
	#echo "$b_name"
	wget -q -O ${b_name} $i
	#echo "${NOW}/${VERSION}/$i" >> url.txt
	echo "   upload package...."
	scp ${b_name} root@119.23.219.229:/data/baodi/${NOW}/${VERSION}/
done

