�Թ���·

������ļ���
mazetower -u x:\xx\data.dat
�����Ŀ¼��
mazetower -r x:\xx\data

���ṹ��data.dat��
0x00 :
8 byte fix header : 50 53 33 46 53 5F 56 31 (PS3FS_V1)
1 int32 file count : 00 00 0A A6 
1 int32 0
0x10 :
48 byte string ,file name
1 int32 0
1 int32 file length
1 int32 0
1 int32 file offset (from 0)
....
�����㷨:
origin = fileOffset + fileLength;
next = origin + (0x200 - origin % 0x200);

���ṹ��script.sdt��
0x00 :
8 byte fix header : 44 53 41 52 43 20 46 4C (DSARC FL)
1 int32 file count : 00 00 00 04 
1 int32 0
0x10 :
40 byte string ,file name
1 int32 file length
1 int32 file offset (from 0)
...