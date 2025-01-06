#!/bin/sh

########################################################################
# Author      :  
# Script Name : /mnt/mtdblock/auto.sh
# Description : 
# Version     : V1.0
# Usage       : 
# 
# Log         :
#
########################################################################

export FILE_PATH="/mnt/mmcblock/"
export BASE_SCRIPT_FILE="/mnt/mtdblock/script/base.sh"
export GPIO_SCRIPT_FILE="/mnt/mtdblock/script/gpio.sh"
export MG_ENV_DLCUSTOM_IAL=/mnt/mtdblock/lib/libcustom_procs.so
# 检测 auto.sh 是否开启调试模式
[ "$1" == 'D' ] && export DEBUG='yes';

. "$BASE_SCRIPT_FILE" # <-- 加载库脚本文件，不能删除
. "$GPIO_SCRIPT_FILE" # <-- 加载库脚本文件，不能删除

LOGSUFFIX=.log
LOGLEVEL=EIWD
PUSHLOGLEVEL=EIW

ConfigTool=initconfigtool
UFOServer=ufo
UDPServer=udpserver
LicdmService=licdm
HubService=hub
OptionsTool=initoptionstool
DevService=devs
BiometricService=biometric
MginitService=mginit
PushService=pushcomm
CLCService=clccomm
PullService=pullcomm
StandaloneService=standalonecomm
WebService=webcomm
MainApp=main
IpcDvr=ipc_dvr
OnvifServer=onvif_server
ZKSIP=sipcall
VisualTalk=visualtalk
ZKManager=manager
LOGDIR=/mnt/mtdblock/logdir

/mnt/mtdblock/mac.sh

[ "${DEBUG}" != "yes" ] && ifconfig eth0 192.168.1.201 up # <-- 调试模式不设置IP
if [ "${LOGTEST}" = "yes" ]; then
	$ECHO "zxr--------------OPEN DEBUG-------------------------LOGDIR=${LOGDIR}"  
	[ ! -d $LOGDIR ] && mkdir $LOGDIR -p
fi

if [ -f $DEST/logwrapper ]; then 	
        cd $DEST && mv logwrapper /usr/bin/logwrapper && sync
fi

if [ -f $DEST/runlog/dev.log ]; then 	
        cd $DEST && mv runlog/dev.log /mnt/ramdisk/ && sync
fi

if [ ! -d /var/tmp ]; then
	mkdir /var/tmp
fi

if [ ! -d /var/lock ]; then
	mkdir /var/lock
fi

if [ ! -d /var/nfftmp ]; then
	mkdir /var/nfftmp
fi

if [ ! -d $DEST2/capture/pass ]; then
	mkdir -p $DEST2/capture/pass
fi

if [ ! -d $DEST2/capture/bad ]; then
	mkdir -p $DEST2/capture/bad
fi

if [ ! -d $DEST2/photo ]; then
	mkdir -p $DEST2/photo
fi

if [ ! -d /usr/lib ]; then
	mkdir -p /usr/lib
fi

rm -rf /var/tmp/*
rm -rf /var/nfftmp/*
rm -rf /var/run/ppp* 
rm -rf /var/run/*.pid
rm -rf /mnt/mtdblock/auto1.sh

if [ -d $DEST/wav/wav ]; then 
	mv $DEST/wav/wav/* $DEST/wav/
	rm -rf $DEST/wav/wav
fi

if [ -d $DEST/wav ]; then
	mkdir -p /mnt/ramdisk/wav && cp $DEST/wav/?[^_]* /mnt/ramdisk/wav/ -rf && sync
fi

if [ ! -f /usr/sbin/pppd ]; then
	cp /mnt/mtdblock/drivers/gprs/* /usr/sbin/. -rf && sync
fi


#还原数据
if [ -f $DEST/data/restore/RestoreOK ]; then 
	tar -zxvf $DEST/data/restore/restore.tgz -C $DEST/data/ && sync && rm $DEST/data/restore/restore.tgz $DEST/data/restore/RestoreOK -rf
	echo 3 > /proc/sys/vm/drop_caches
fi

#U盘升级二进制文件
if [ -f $DEST/data/update.tgz ]; then 
	tar -zxvf $DEST/data/update.tgz -C / && sync && rm $DEST/data/update.tgz
	echo 3 > /proc/sys/vm/drop_caches
fi

#解压人脸算法升级包
#module_face_alg.tgz定义为人脸模块升级包，打包规则和mtdblock.tgz一样，只是该包只涉及人脸算法相关文件
#做成的emfw.tgz文件需要修改update.cfg配置，修改resourcecount0=mtdblock.tgz,/mnt/mtdblock/module_face_alg.tgz
if [ -f $DEST/module_face_alg.tgz ]; then 
	tar -zxvf $DEST/module_face_alg.tgz  -C $DEST/ && sync && rm $DEST/module_face_alg.tgz 
	
	if [ -f $DESTSERVICE/${OptionsTool} ]; then
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${OptionsTool} && $DESTSERVICE/${OptionsTool} ${LOGLEVEL} > ${LOGDIR}/${OptionsTool}${LOGSUFFIX}
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${OptionsTool} && $DESTSERVICE/${OptionsTool}
		fi
	fi
	
	reboot
fi

if [ -f $DEST/module_fp_alg.tgz ]; then 
	tar -zxvf $DEST/module_fp_alg.tgz -C $DEST/ && sync && rm $DEST/module_fp_alg.tgz
	
	if [ -f $DESTSERVICE/${OptionsTool} ]; then
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${OptionsTool} && $DESTSERVICE/${OptionsTool} ${LOGLEVEL} > ${LOGDIR}/${OptionsTool}${LOGSUFFIX}
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${OptionsTool} && $DESTSERVICE/${OptionsTool}
		fi
	fi
	reboot
fi

if [ -f $DEST/mtdblock.tgz ]; then 
	tar -zxvf $DEST/mtdblock.tgz -C $DEST/ && sync && rm $DEST/mtdblock.tgz
	echo 3 > /proc/sys/vm/drop_caches	
	reboot 
fi

if [ -f $DEST/script/decompress.sh ]; then 
	chmod u+x $DEST/script/decompress.sh && $DEST/script/decompress.sh
	
fi

####配网、更新prefile目录文件，放在这个位置是想解压包后，马上处理其他关联更新#####  add by nongbj 2021.06.09##########
Service=${ConfigTool}                
if [ -f $DESTSERVICE/${Service} ]; then  
        echo "${Service} run"                                                                                                                   
        if [ "${LOGTEST}" = "yes" ]; then                                                                                                       
                cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX}
        else                                                                                     
                cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service}
        fi                
fi                        
echo "${Service} run over"

if [ -f $DEST/script/autoset.sh ]; then 
	chmod u+x $DEST/script/autoset.sh && $DEST/script/autoset.sh
fi

if [ -f $DEST/language.tgz ]; then 
	tar -zxvf $DEST/language.tgz -C $DEST/ && sync && rm $DEST/language.tgz
fi

if [ -f $DEST/languagesig.tgz ]; then 
	tar -xvf $DEST/languagesig.tgz -C $DEST/ && sync && rm $DEST/languagesig.tgz
fi

if [ -d $DEST/app/mginit/language/lang ]; then
	rm -rf $DEST/app/mginit/language/lang
fi

if [ -f $DEST/lib/minires.tgz ]; then
	tar -xzf $DEST/lib/minires.tgz -C /usr/local/lib && sync && rm $DEST/lib/minires.tgz -f &&  sync
fi

#### 运行加载驱动前执行扩展脚本 --profile 状态 ################  add by jigc 2014.12.11##########
if [ -f "$MACHINE_EX_AUTOSH" ]; then 
	$ECHO "\nRuning machine exten script : $(basename ${MACHINE_EX_AUTOSH})  --profile ..."
	. $MACHINE_EX_AUTOSH --profile
	$ECHO "Endof machine exten script : $(basename ${MACHINE_EX_AUTOSH})  --profile ...\n"
fi
#############################################################################################

PorcAutoInsmodDriver # <-- 自动加载驱动，本函数在base.sh 中定义

if [ ! -c /dev/ppp ]; then 
	mknod /dev/ppp c 108 0 
fi

if [ ! -c /dev/zkboard ]; then
	mknod /dev/zkboard c 217 0
fi

#ALGOLIB=$DEST/lib/libzkfp.so.3.5.1
ln -s /mnt/mtdblock/lib/libfreetype.so $LIB/libfreetype.so.6 -f
ln -s /mnt/mtdblock/lib/libNE10.so $LIB/libNE10.so.10 -f
ln -s /mnt/mtdblock/lib/libasound.so $LIB/libasound.so.2 -f
ln -s /mnt/mtdblock/lib/libpng.so $LIB/libpng.so.3 -f
ln -s /mnt/mtdblock/lib/libsqlite3.so $LIB/libsqlite3.so.0 -f
ln -s /mnt/mtdblock/lib/libcrypto.so $LIB/libcrypto.so.1.0.0 -f
ln -s /mnt/mtdblock/lib/libssl.so $LIB/libssl.so.1.0.0 -f

if [ ! -f /etc/MiniGUI.cfg ]; then
	ln -snf $DEST/app/mginit/MiniGUI.cfg /etc/
fi

if [ -f $DEST/service/zkpalmveinlic.txt ]; then
	cp  $DEST/service/zkpalmveinlic.txt $DEST/
fi

if [ ! -f /lib/device ]; then
	ln -snf /mnt/mtdblock/lib/device/zkdev $LIB/device
fi

if [ -f $DEST/data/udhcpc ]; then
	chmod u+x /mnt/mtdblock/data/udhcpc
	ln -snf /mnt/mtdblock/data/udhcpc /usr/sbin/
fi

if [ -f /mnt/mtdblock/localtime ]; then
	cp /mnt/mtdblock/localtime /etc/
	chmod 777 /etc/localtime
	rm /mnt/mtdblock/localtime 
fi

#for secugen PIV
if [ -f $LIB/libsecugen.so ]; then
	ln -s $LIB/libsecugen.so $LIB/ -f
	ln -s $LIB/libsgfdu04.so $LIB/ -f
	ln -s $LIB/libusb.so $LIB/ -f
	ln -s $LIB/libusb.so $LIB/libusb-0.1.so.4 -f
fi

if [ ! -f $LIB/libzkfp.so.9 ]; then
	ln -snf $LIB/libzkfp.so.3.5.1 $LIB/libzkfp.so.3
fi

if [ ! -f $LIB/libzkfp.so.10 ]; then
	ln -snf $LIB/libzkfp.so.4.0.0 $LIB/libzkfp.so.10
fi

if [ ! -f $LIB/libstdc++.so.6 ]; then
	ln -snf $LIB/libc2.so $LIB/libstdc++.so.6
fi

if [ ! -f /usr/lib/libiale_custom.so ]; then
	ln -snf $LIB/libcustom_procs.so /usr/lib/libiale_custom.so
fi

if [ ! -f $LIB/libsoftkeywin.so.0 ]; then
	ln -snf $LIB/libsoftkeywin.so $LIB/libsoftkeywin.so.0
fi

if [ ! -f /lib/libzkweb.so ]; then
	ln -snf $LIB/libzkweb.so /lib/libzkweb.so
fi

if [ ! -f $LIB/libxml2.so.2 ]; then
	ln -snf $LIB/libxml2.so $LIB/libxml2.so.2
fi

if [ ! -f $LIB/libsqlite3.so ]; then
	ln -snf $LIB/libsqlite3.so $LIB/libsqlite3.so.0
fi

if [ ! -f $LIB/libpng.so.3 ]; then
	ln -snf $LIB/libpng.so.3.1.2.37 $LIB/libpng.so
	ln -snf $LIB/libpng.so $LIB/libpng.so.3
fi

if [ ! -f $LIB/libfreetype.so ]; then
	ln -snf $LIB/libfreetype.so.6.16.0 $LIB/libfreetype.so.6
	ln -snf $LIB/libfreetype.so.6 $LIB/libfreetype.so
fi

if [ ! -f $LIB/libz.so ]; then
	ln -snf $LIB/libz.so.1.2.7 $LIB/libz.so.1
	ln -snf $LIB/libz.so.1 $LIB/libz.so
fi

if [ ! -f $LIB/libjpeg.so.9 ]; then
	ln -snf $LIB/libjpeg.so $LIB/libjpeg.so.9
	ln -snf $LIB/libjpeg.so.9 $LIB/libjpeg.so.9.3.0
fi

if [ ! -f $LIB/libminigui_procs-3.0.so.12 ]; then
	ln -snf $LIB/libminigui_procs-3.0.so.12.0.0 $LIB/libminigui_procs-3.0.so.12
fi

if [ ! -f $LIB/libnopoll.so.0 ]; then
	ln -snf $LIB/libnopoll.so.0.0.0 $LIB/libnopoll.so.0
fi

if [ -f $LIB/libzbar.so ]; then
	ln -snf $LIB/libzbar.so  $LIB/libzbar.so.0
fi

if [ -f $LIB/libusb-1.0.so ]; then
	ln -snf $LIB/libusb-1.0.so $LIB/libusb-1.0.so.0
fi

if [ -f $LIB/libfaac.so ]; then
	ln -snf $LIB/libfaac.so $LIB/libfaac.so.0
fi

if [ -f $LIB/libgpac.so ]; then
	ln -snf $LIB/libgpac.so $LIB/libgpac.so.7
fi

if [ -f $LIB/libnanomsg.so ]; then
	ln -snf $LIB/libnanomsg.so $LIB/libnanomsg.so.5
fi

if [ -f $DEST/lib/device/zkdev/zdev_guide_temp_arm.so ]; then
	rm $DEST/lib/device/zkdev/zdev_guide_temp_arm.so && sync
fi

AutolinkApplib # <-- 自动建立app链接，本函数在base.sh 中定义

rm /dev/input/mice
rm /var/tmp/mginit

sync

if [ -f $DEST/st7701s_4inch_init ]; then
	chmod u+x $DEST/st7701s_4inch_init && $DEST/st7701s_4inch_init && sync
	echo "$st7701s_4inch_init run"
fi


# 运行固件前执行扩展脚本 --pre-runfirmware 状态################################
if [ -f "$MACHINE_EX_AUTOSH" ]; then 
	$ECHO "\nRuning machine exten script : $(basename ${MACHINE_EX_AUTOSH}) --pre-runfirmware ..."
	. $MACHINE_EX_AUTOSH --pre-runfirmware
	$ECHO "Endof machine exten script : $(basename ${MACHINE_EX_AUTOSH}) --pre-runfirmware ...\n"
fi
[ ! -z ${DEBUG} ] && echo "End of Debug..." && return 0
###############################################################################

Service=${UDPServer}                
if [ -f $DESTSERVICE/${Service} ]; then  
        echo "${Service} run"                                                                                                                   
        if [ "${LOGTEST}" = "yes" ]; then                                                                                                       
                cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
        else                                                                                     
                cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
        fi                
fi                        
echo "${Service} run over"

Service=${LicdmService}
if [ -f $DESTSERVICE/${Service} ]; then
	echo "${Service} run"
	if [ "${LOGTEST}" = "yes" ]; then
		cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
	else
		cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
	fi
fi
echo "${Service} run over"

#### licdm socket限制最大超时：1200*50000=1分钟 #################
#### 1分钟后还是无法起来，运行UFO保证能运行在线升级服务器 #######
#### 需要保证涉及参数的其他进程放在licdm后面运行 ################
i=1
while [ $i -lt 1200 ]; do
	
	if [ -e /var/tmp/commskt.socket ]; then
		break
	fi
	
	i=$(($i+1))
	sync
	usleep 50000
done;

usleep 50000

if [ -f $DESTSERVICE/${OptionsTool} ]; then
	if [ "${LOGTEST}" = "yes" ]; then
		cd $DESTSERVICE && chmod u+x $DESTSERVICE/${OptionsTool} && $DESTSERVICE/${OptionsTool} ${LOGLEVEL} > ${LOGDIR}/${OptionsTool}${LOGSUFFIX}
	else
		cd $DESTSERVICE && chmod u+x $DESTSERVICE/${OptionsTool} && $DESTSERVICE/${OptionsTool}
	fi
fi

usleep 50000

Service=${UFOServer}                
if [ -f $DESTSERVICE/${Service} ]; then  
        echo "${Service} run"                                                                                                                   
        if [ "${LOGTEST}" = "yes" ]; then                                                                                                       
                cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
        else                                                                                     
                cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
        fi                
fi                        
echo "${Service} run over"

#初始化双固件参数
CityRayInitPrm

#此次修改为生产合并映像中U盘升级优化使用########################
#将机器初始化上电后重启两次的操作提前到U盘升级过程中############
#后续打target包固件时需要注意###################################
if [ -f /mnt/mtdblock/tags_usb.txt ]; then
	rm /mnt/mtdblock/tags_usb.txt -rf
	sync
	control_led_for_update_firmware
fi

usleep 50000

Service=${HubService}
if [ -f $DESTSERVICE/${Service} ]; then
	echo "${Service} run"
	if [ "${LOGTEST}" = "yes" ]; then
		cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
	else
		cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
	fi
fi
echo "${Service} run over"

while [ ! -e /var/nfftmp/hub ]; do
	usleep 50000
done;



if [ -f $DESTSERVICE/initdataencdec ]; then                                           
	cd $DESTSERVICE && chmod u+x $DESTSERVICE/initdataencdec && $DESTSERVICE/initdataencdec
fi

# 连接可见光库
Linklibfacesdk


usleep 500000

#start devs 
Service=${DevService}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
	
	while [ ! -e /var/nfftmp/dev ]; do
		usleep 50000
	done;
fi

echo "${Service} run over"

usleep 50000

Service=${MginitService}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTAPP/mginit/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTAPP/mginit && chmod u+x $DESTAPP/mginit/${Service} && $DESTAPP/mginit/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTAPP/mginit && chmod u+x $DESTAPP/mginit/${Service} && $DESTAPP/mginit/${Service} &
		fi
	fi
fi
echo "${Service} run over"

if [ "${LOGTEST}" != "yes" ]; then 
	while [ ! -e /var/nfftmp/main ]; do
		usleep 50000
	done;
fi


usleep 50000 && usleep 50000
usleep 50000 && usleep 50000
usleep 50000 && usleep 50000
usleep 50000 && usleep 50000
usleep 50000 && usleep 50000
usleep 50000 && usleep 50000
usleep 50000

if [ "${LOGTEST}" = "yes" ]; then
	while [ ! -e /var/tmp/minigui ]; do
		usleep 50000
	done;
	
	sleep 1 # <-- 休眠1秒解决U盘抓日志开启后main启动卡在 JoinLayer 接口的问题
	
	pgrep ${MainApp} > /dev/nul
	if [ "$?" != 0 ]; then
		if [ -f $DESTAPP/main/${MainApp} ]; then
			echo "${MainApp} run"
			cd $DESTAPP/main && chmod u+x $DESTAPP/main/${MainApp} && $DESTAPP/main/${MainApp} ${LOGLEVEL} > ${LOGDIR}/${MainApp}${LOGSUFFIX} &
		fi
	fi
fi
echo "${MainApp} run over"


if [ -f $DEST/script/autoinitoptions.sh ]; then 
	chmod u+x $DEST/script/autoinitoptions.sh && $DEST/script/autoinitoptions.sh;
	rm -rf $DEST/script/autoinitoptions.sh;
fi


if [ -f $DEST/data/wdt ]; then
	cd $DEST/data && chmod u+x $DEST/data/wdt && $DEST/data/wdt -p 5 -t 3600 -m "$DEST/app/main/main"
fi

rm -rf /mnt/mtdblock/service/ipcamera.init && sync
rm -rf /mnt/mtdblock/service/bioliveface.init && sync
Service=${BiometricService}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ -f "$USER_DB_FILE" ]; then # <-- 条件成立则开启 biometric 调试模式并统计通过率(需配合U盘抓取日志功能使用)
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service}  
			$DESTSERVICE/${Service} EWDI | analysisBiolog-V1.0.awk SAVE_BMP_FILE_DIR=$FINGER_BMP_FILE_DIR RAW_BIO_LOG_FILE=$RAW_BIO_LOG_FILE "$USER_DB_FILE" -  &
		elif [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
fi
echo "${Service} run over"
usleep 50000

Service=${CLCService}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} protocol=best-w > ${LOGDIR}/${Service}-best${LOGSUFFIX} &
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} protocol=ufo > ${LOGDIR}/${Service}-ufo${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} protocol=all &
		fi
	fi
fi
echo "${Service} run over"

usleep 50000

while [ ! -e /mnt/mtdblock/service/bioliveface.init ]; do
	usleep 50000
done;

while [ ! -e /mnt/mtdblock/service/ipcamera.init ]; do
	usleep 50000
done;

rm -rf /mnt/mtdblock/service/bioliveface.init && sync
rm -rf /mnt/mtdblock/service/ipcamera.init && sync

echo 3 > /proc/sys/vm/drop_caches

Service=${PushService}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${PUSHLOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
fi
echo "${Service} run over"

usleep 50000

Service=${PullService}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
fi
echo "${Service} run over"

usleep 50000


Service=${IpcDvr}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
fi
echo "${Service} run over"

usleep 50000

Service=${OnvifServer}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
fi
echo "${Service} run over"

usleep 50000


Service=${ZKSIP}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
fi
echo "${Service} run over"

usleep 50000


Service=${VisualTalk}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
fi
echo "${Service} run over"

usleep 50000

Service=${ZKManager}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
fi
echo "${Service} run over"

usleep 50000

Service=${StandaloneService}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DESTSERVICE/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} ${LOGLEVEL} > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DESTSERVICE && chmod u+x $DESTSERVICE/${Service} && $DESTSERVICE/${Service} &
		fi
	fi
fi
echo "${Service} run over"

usleep 50000

Service=${WebService}
pgrep ${Service} > /dev/nul
if [ "$?" != 0 ]; then
	if [ -f $DEST/webcomm/${Service} ]; then
		echo "${Service} run"
		if [ "${LOGTEST}" = "yes" ]; then
			cd $DEST/webcomm && chmod u+x $DEST/webcomm/${Service} && $DEST/webcomm/${Service} --log stdout:15  > ${LOGDIR}/${Service}${LOGSUFFIX} &
		else
			cd $DEST/webcomm && chmod u+x $DEST/webcomm/${Service} && $DEST/webcomm/${Service} &
		fi
	fi
fi
echo "${Service} run over"

ulimit -c unlimited
