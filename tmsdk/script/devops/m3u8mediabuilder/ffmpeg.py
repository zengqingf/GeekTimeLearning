# -*- encoding: utf-8 -*-
import sys,os
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))

from comlib import com

import re,json,math
ffmpegpath = r'G:\tools\ffmpeg-20200610-9dfb19b-win64-static\bin\ffmpeg.exe'
ffprobepath = r'G:\tools\ffmpeg-20200610-9dfb19b-win64-static\bin\ffprobe.exe'
info_cmd = '{} -v quiet -print_format json -show_format -show_streams {}'
mp4path = r'G:\tools\ffmpeg-20200610-9dfb19b-win64-static\bin\test.mp4'
def getInfo(mp4path):
    '''
    return duration,bitrate,size,resolution
    '''
    out,code = com.cmd(info_cmd.format(ffprobepath,mp4path))
    info = json.loads(out.strip())

    duration = float(info['format']['duration'])

    bitrate = int(info['format']['bit_rate'])
    size = int(info['format']['size'])
    resolution = int(info['streams'][0]['height'])
    codec_name = info['streams'][0]['codec_name']
    return duration,bitrate,size,resolution,codec_name

def mp4_TO_h264(mp4path,outpath)->str:
    outfile = os.path.join(outpath,os.path.basename(mp4path).replace('.mp4','_h264.mp4'))

    h264_cmd = f'{ffmpegpath} -i {mp4path} -vcodec h264 -acodec copy {outfile}'
    com.cmd(h264_cmd,errException=Exception('h264转码失败'))
    return outfile
def audio_aac_compress(audiopath,outpath,bitrate_K):
    basename = os.path.basename(audiopath)
    ext = os.path.splitext(basename)[1]
    outfile = os.path.join(outpath,basename.replace(ext,'_%s')%bitrate_K+ext)

    acc_cmd = f'{ffmpegpath} -i {audiopath} -b:a {bitrate_K}k -vcodec copy -acodec aac -ar 44100 {outfile}' #  -ac 2
    com.cmd(acc_cmd,errException=Exception('acc压缩失败'))
def audio_aac_compress_range(audiopath,outpath,bitrates_K:list):
    for bitrate in bitrates_K:
        audio_aac_compress(audiopath,outpath,bitrate)
def mp4_TO_m3u8(mp4path,outpath,cutlenght)->int:
    duration,bitrate,size,resolution,codec_name = getInfo(mp4path)
    if codec_name != 'h264':
        mp4path = mp4_TO_h264(mp4path,outpath)
    
    tsfile = mp4path.replace('.mp4','.ts')
    outfile_m3u8 = os.path.join(outpath,os.path.basename(mp4path).replace('.mp4','.m3u8'))
    outfile_cut = os.path.join(outpath,os.path.basename(mp4path).replace('.mp4','%03d.ts'))

    ts_cmd=f'{ffmpegpath} -y -i {mp4path} -vcodec copy -acodec copy -vbsf h264_mp4toannexb {tsfile}'
    out,code = com.cmd(ts_cmd,errException=Exception('转ts失败'))
    m3u8_cmd = f'{ffmpegpath} -i {tsfile} -c copy -map 0 -f segment -segment_list {outfile_m3u8} -segment_time {cutlenght} {outfile_cut}'
    com.cmd(m3u8_cmd,errException=Exception('转m3u8失败'))
    with open(outfile_m3u8,'r+',encoding='utf-8') as fs:
        allstr = fs.read()
        allstr = allstr.replace('#EXTM3U','#EXTM3U\n#EXT-X-PLAYLIST-TYPE:VOD')
        fs.seek(0)
        fs.truncate()
        fs.write(allstr)
    if codec_name != 'h264':
        os.remove(mp4path)
    os.remove(tsfile)
    return bitrate,os.path.basename(outfile_m3u8)
def mp4s_TO_m3u8AutoAdaptor(mp4paths:list,outpath,main_m3u8name,cutlenght):
    main_m3u8file = os.path.join(outpath,main_m3u8name)
    with open(main_m3u8file,'w',encoding='utf-8') as fs:
        fs.write('#EXTM3U\n')
        block='#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH={}\n{}\n'
        for mp4 in mp4paths:
            bitrate,m3u8name = mp4_TO_m3u8(mp4,outpath,cutlenght)
            fs.write(block.format(bitrate,m3u8name))

class MediaHelper(object):
    def __init__(self,ffmpegpath,ffprobepath,mp4path):
        super().__init__()
        self.ffmpegpath = ffmpegpath
        self.ffprobepath = ffprobepath
        self.mp4path = mp4path

        # out,code = com.cmd(info_cmd.format(ffprobepath,mp4path))
        # info = json.loads(out.strip())

        # self.duration = float(info['format']['duration'])

        # self.bitrate = int(info['format']['bit_rate'])
        # self.size = int(info['format']['size'])
        # self.resolution = int(info['streams'][0]['height'])
        self.duration,self.bitrate,self.size,self.resolution,self.codec_name = MediaHelper.getInfo(self.mp4path)
        print(f'{self.duration} {self.bitrate} {self.size} {self.resolution}')
        print(math.ceil(self.duration*self.bitrate/8))
        # tmp = duration.split(':')
        
    def rebuild_3type(self,target_size,outpath,target_resolution=None)->int:
        target_bitrate = target_size * 8 / self.duration
        bitrate_low = int(target_bitrate * 0.7)
        bitrate_mid = target_bitrate
        bitrate_high = int(target_bitrate * 1.3)
        outfile_low = os.path.join(outpath,os.path.basename(self.mp4path).replace('.mp4','low.mp4'))
        outfile_mid = os.path.join(outpath,os.path.basename(self.mp4path).replace('.mp4','mid.mp4'))
        outfile_high = os.path.join(outpath,os.path.basename(self.mp4path).replace('.mp4','high.mp4'))

        self.rebuild(bitrate_low,outfile_low,target_resolution)
        self.rebuild(bitrate_mid,outfile_mid,target_resolution)
        self.rebuild(bitrate_high,outfile_high,target_resolution)
    def rebuild_test(self,target_size:list,outpath,target_resolution=None)->int:
        for size in target_size:
            target_bitrate = size * 8 / self.duration
            outfile_high = os.path.join(outpath,os.path.basename(self.mp4path).replace('.mp4','_%s.mp4'%int(size/1024/1024)))
            self.rebuild(target_bitrate,outfile_high,target_resolution)

    def rebuild(self,target_bitrate,outfile,target_resolution='')->int:
        # target_bitrate = target_size * 8 / self.duration
        bitrate_max = int(target_bitrate * 1.3)
        res_cmd = ''
        if target_resolution not in (None,''):
            res_cmd = f'-vf scale=-1:{target_resolution}'
        
        # outfile = os.path.join(outpath,os.path.basename(self.mp4path))
        # -c:a copy 拷贝音频流不进行转码
        # http://git.oschina.net/abcfy2/simple_video_compress_build
        rebuild_cmd = f'{self.ffmpegpath} -i {self.mp4path} -b:v {target_bitrate} -bufsize {target_bitrate} -maxrate {bitrate_max} {res_cmd} {outfile}'
        out,code = com.cmd(rebuild_cmd,errException=Exception('重建失败'))
        print(f'{outfile} {os.path.getsize(outfile)/1024/1024}')



if __name__ == "__main__":
    # getDesc()
    # b = MediaHelper(ffmpegpath,ffprobepath,mp4path)
    # b.rebuild_test([x*1024*1024 for x in range(1,20,2)],r'G:\tools\ffmpeg-20200610-9dfb19b-win64-static\bin\alltest')
    # mp4_TO_m3u8(mp4path,r'D:\_WorkSpace\_sdk\LazyScripts\test',10)

    # -------------------------------------------
    paths = [r'G:\tools\ffmpeg-20200610-9dfb19b-win64-static\bin\alltest\test_1.0.mp4',
    r'G:\tools\ffmpeg-20200610-9dfb19b-win64-static\bin\alltest\test_3.0.mp4',
    r'G:\tools\ffmpeg-20200610-9dfb19b-win64-static\bin\alltest\test_5.0.mp4',
    r'G:\tools\ffmpeg-20200610-9dfb19b-win64-static\bin\alltest\test_7.0.mp4',]
    # paths = [r'D:\_WorkSpace\_sdk\SDK_Projects\2.0\LeBianUI_Android_Project_Root\Lebian2021_UI_AndroidProject\lbui\src\main\assets\bg.mp4']
    # out = r'D:\_WorkSpace\_sdk\LazyScripts\test'
    # out = r'D:\_WorkSpace\_sdk\LazyScripts\test2'
    
    # mp4s_TO_m3u8AutoAdaptor(paths,out,'main.m3u8',10)

    # ------------------------------------

    ap = r'G:\tools\ffmpeg-20200610-9dfb19b-win64-static\bin\alltest\test_7.0.mp4'
    out = r'D:\_WorkSpace\_sdk\LazyScripts\acc'

    # audio_aac_compress(ap,out,32)

    # ----------------------------------------------------
    rate = [x*16 for x in range(1,8)]
    audio_aac_compress_range(ap,out,rate)






'''
{
    "streams": [
        {
            "index": 0,
            "codec_name": "h264",
            "codec_long_name": "H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10",
            "profile": "Main",
            "codec_type": "video",
            "codec_time_base": "1/50",
            "codec_tag_string": "avc1",
            "codec_tag": "0x31637661",
            "width": 640,
            "height": 360,
            "coded_width": 640,
            "coded_height": 368,
            "closed_captions": 0,
            "has_b_frames": 0,
            "sample_aspect_ratio": "1:1",
            "display_aspect_ratio": "16:9",
            "pix_fmt": "yuv420p",
            "level": 30,
            "color_range": "tv",
            "chroma_location": "left",
            "refs": 1,
            "is_avc": "true",
            "nal_length_size": "4",
            "r_frame_rate": "25/1",
            "avg_frame_rate": "25/1",
            "time_base": "1/25000",
            "start_pts": 0,
            "start_time": "0.000000",
            "duration_ts": 2733000,
            "duration": "109.320000",
            "bit_rate": "617299",
            "bits_per_raw_sample": "8",
            "nb_frames": "2733",
            "disposition": {
                "default": 1,
                "dub": 0,
                "original": 0,
                "comment": 0,
                "lyrics": 0,
                "karaoke": 0,
                "forced": 0,
                "hearing_impaired": 0,
                "visual_impaired": 0,
                "clean_effects": 0,
                "attached_pic": 0,
                "timed_thumbnails": 0
            },
            "tags": {
                "creation_time": "2016-11-24T23:23:39.000000Z",
                "language": "eng",
                "handler_name": "Mainconcept MP4 Video Media Handler",
                "encoder": "AVC Coding"
            }
        },
        {
            "index": 1,
            "codec_name": "aac",
            "codec_long_name": "AAC (Advanced Audio Coding)",
            "profile": "LC",
            "codec_type": "audio",
            "codec_time_base": "1/44100",
            "codec_tag_string": "mp4a",
            "codec_tag": "0x6134706d",
            "sample_fmt": "fltp",
            "sample_rate": "44100",
            "channels": 2,
            "channel_layout": "stereo",
            "bits_per_sample": 0,
            "r_frame_rate": "0/0",
            "avg_frame_rate": "0/0",
            "time_base": "1/44100",
            "start_pts": 0,
            "start_time": "0.000000",
            "duration_ts": 4825088,
            "duration": "109.412426",
            "bit_rate": "61642",
            "max_bit_rate": "226701",
            "nb_frames": "4712",
            "disposition": {
                "default": 1,
                "dub": 0,
                "original": 0,
                "comment": 0,
                "lyrics": 0,
                "karaoke": 0,
                "forced": 0,
                "hearing_impaired": 0,
                "visual_impaired": 0,
                "clean_effects": 0,
                "attached_pic": 0,
                "timed_thumbnails": 0
            },
            "tags": {
                "creation_time": "2016-11-24T23:23:39.000000Z",
                "language": "eng",
                "handler_name": "Mainconcept MP4 Sound Media Handler"
            }
        }
    ],
    "format": {
        "filename": "test.mp4",
        "nb_streams": 2,
        "nb_programs": 0,
        "format_name": "mov,mp4,m4a,3gp,3g2,mj2",
        "format_long_name": "QuickTime / MOV",
        "start_time": "0.000000",
        "duration": "109.412422",
        "size": "9313358",
        "bit_rate": "680972",
        "probe_score": 100,
        "tags": {
            "major_brand": "mp42",
            "minor_version": "0",
            "compatible_brands": "isommp42",
            "creation_time": "2016-11-24T23:23:39.000000Z"
        }
    }
}
'''