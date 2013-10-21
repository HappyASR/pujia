/*
  by Luigi Auriemma
*/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#ifdef WIN32
    #include <windows.h>
#endif



static int              msf_unpack_00405370;
static int              msf_unpack_00405378;
static int              msf_unpack_0040537C;
static int              msf_unpack_00405374;
static unsigned char    msf_unpack_004041ec[0x80];



static /*const*/ unsigned char msf_unpack_dump[] = {
    0x83,0xec,0x0c,0x8b,0x44,0x24,0x1c,0x53,0x8b,0x18,0x55,0x8b,0x6c,0x24,0x18,0x56,
    0xc7,0x00,0x00,0x00,0x00,0x00,0x81,0x7d,0x48,0x12,0x01,0x00,0x00,0x57,0x89,0x5c,
    0x24,0x10,0x0f,0x84,0x2b,0x02,0x00,0x00,0xeb,0x06,0x8d,0x9b,0x00,0x00,0x00,0x00,
    0x83,0x7d,0x4c,0x00,0x0f,0x84,0x90,0x00,0x00,0x00,0x85,0xdb,0x74,0x37,0x8b,0xff,
    0x83,0x7d,0x58,0x05,0x73,0x2b,0x8b,0x44,0x24,0x28,0x8b,0x74,0x24,0x20,0x8b,0x4e,
    0x58,0x3e,0x8a,0x10,0x3e,0x88,0x54,0x31,0x5c,0x8b,0x44,0x24,0x2c,0xb9,0x01,0x00,
    0x00,0x00,0x01,0x4d,0x58,0x01,0x08,0x01,0x4c,0x24,0x28,0x2b,0xd9,0x85,0xdb,0x75,
    0xcf,0x89,0x5c,0x24,0x10,0x83,0x7d,0x58,0x05,0x0f,0x82,0xb7,0x01,0x00,0x00,0x80,
    0x7d,0x5c,0x00,0x0f,0x85,0x9d,0x01,0x00,0x00,0x8b,0x45,0x5c,0x8b,0xc8,0xc1,0xe9,
    0x08,0x0f,0xb6,0xd1,0x8b,0xc8,0xc1,0xe9,0x10,0xc1,0xe2,0x08,0x0f,0xb6,0xc9,0x03,
    0xd1,0xc1,0xe2,0x08,0xc1,0xe8,0x18,0x03,0xd0,0x0f,0xb6,0x45,0x60,0xc1,0xe2,0x08,
    0x03,0xd0,0x89,0x55,0x20,0xc7,0x45,0x1c,0xff,0xff,0xff,0xff,0xc7,0x45,0x4c,0x00,
    0x00,0x00,0x00,0xc7,0x45,0x58,0x00,0x00,0x00,0x00,0x8b,0x4d,0x24,0x33,0xf6,0x3b,
    0x4c,0x24,0x24,0x72,0x1b,0x39,0x75,0x48,0x0f,0x85,0x48,0x01,0x00,0x00,0x39,0x75,
    0x20,0x0f,0x84,0x4f,0x01,0x00,0x00,0xba,0x01,0x00,0x00,0x00,0x8b,0xf2,0xeb,0x05,
    0xba,0x01,0x00,0x00,0x00,0x83,0x7d,0x50,0x00,0x74,0x2b,0x8b,0x45,0x10,0x89,0x44,
    0x24,0x18,0x8b,0x7c,0x24,0x18,0xb8,0x00,0x04,0x00,0x04,0xb9,0x9b,0x0f,0x00,0x00,
    0xf3,0xab,0x33,0xc0,0x89,0x55,0x44,0x89,0x55,0x40,0x89,0x55,0x3c,0x89,0x55,0x38,
    0x89,0x45,0x34,0x89,0x45,0x50,0x8b,0x45,0x58,0x33,0xc9,0x89,0x44,0x24,0x14,0x3b,
    0xc1,0x74,0x7f,0x89,0x4c,0x24,0x18,0x83,0xf8,0x14,0x73,0x22,0x8d,0x64,0x24,0x00,
    0x3b,0xcb,0x73,0x12,0x8b,0x54,0x24,0x28,0x8a,0x14,0x11,0x88,0x54,0x28,0x5c,0x40,
    0x41,0x83,0xf8,0x14,0x72,0xea,0x89,0x44,0x24,0x14,0x89,0x4c,0x24,0x18,0x89,0x45,
    0x58,0x83,0xf8,0x14,0x0f,0x82,0xc6,0x00,0x00,0x00,0x85,0xf6,0x0f,0x85,0xbe,0x00,
    0x00,0x00,0x8b,0x7c,0x24,0x24,0x8b,0x5d,0x5c,0x8d,0x45,0x5c,0x8b,0xf5,0x89,0x45,
    0x18,0xe8,0xea,0x00,0x00,0x00,0x85,0xc0,0x0f,0x85,0x98,0x00,0x00,0x00,0x8b,0x45,
    0x18,0x2b,0x44,0x24,0x14,0x8b,0x4c,0x24,0x18,0x2b,0xc5,0x8d,0x44,0x08,0xa4,0x8b,
    0x4c,0x24,0x2c,0x01,0x01,0x01,0x44,0x24,0x28,0xc7,0x45,0x58,0x00,0x00,0x00,0x00,
    0xeb,0x5e,0x83,0xfb,0x14,0x72,0x0e,0x3b,0xf1,0x75,0x0a,0x8b,0x54,0x24,0x28,0x8d,
    0x5c,0x13,0xec,0xeb,0x20,0x8b,0x7c,0x24,0x28,0x57,0x8b,0xc5,0x8b,0xcb,0xe8,0xdd,
    0x0e,0x00,0x00,0x83,0xc4,0x04,0x85,0xc0,0x74,0x66,0x85,0xf6,0x74,0x05,0x83,0xf8,
    0x02,0x75,0x43,0x8b,0xdf,0x8b,0x44,0x24,0x28,0x8b,0x7c,0x24,0x24,0x8b,0xf5,0x89,
    0x45,0x18,0xe8,0x79,0x00,0x00,0x00,0x85,0xc0,0x75,0x2b,0x8b,0x4c,0x24,0x28,0x8b,
    0x45,0x18,0x8b,0x54,0x24,0x2c,0x2b,0xc1,0x01,0x02,0x03,0xc8,0x89,0x4c,0x24,0x28,
    0x29,0x44,0x24,0x10,0x81,0x7d,0x48,0x12,0x01,0x00,0x00,0x74,0x36,0x8b,0x5c,0x24,
    0x10,0xe9,0x0a,0xfe,0xff,0xff,0x5f,0x5e,0x5d,0xb0,0x01,0x5b,0x83,0xc4,0x0c,0xc3,
    0x8b,0x44,0x24,0x2c,0x01,0x08,0x5f,0x5e,0x5d,0x32,0xc0,0x5b,0x83,0xc4,0x0c,0xc3,
    0x8b,0x44,0x24,0x2c,0x5f,0x89,0x5d,0x58,0x01,0x18,0x5e,0x5d,0x32,0xc0,0x5b,0x83,
    0xc4,0x0c,0xc3,0x5f,0x33,0xc0,0x39,0x45,0x20,0x5e,0x5d,0x0f,0x95,0xc0,0x5b,0x83,
    0xc4,0x0c,0xc3,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,
    0x55,0xbd,0x12,0x01,0x00,0x00,0x83,0x7e,0x30,0x00,0x8b,0xd7,0x75,0x19,0x8b,0x46,
    0x0c,0x8b,0x4e,0x24,0x2b,0x46,0x2c,0x8b,0xef,0x2b,0xe9,0x3b,0xe8,0xbd,0x12,0x01,
    0x00,0x00,0x76,0x03,0x8d,0x14,0x01,0x53,0x52,0x56,0xe8,0x51,0x00,0x00,0x00,0x83,
    0xc4,0x0c,0x85,0xc0,0x75,0x3b,0x8b,0x46,0x0c,0x39,0x46,0x2c,0x72,0x03,0x89,0x46,
    0x30,0x8b,0xce,0x8b,0xc7,0xe8,0xc6,0x13,0x00,0x00,0x39,0x7e,0x24,0x73,0x12,0x39,
    0x5e,0x18,0x73,0x0d,0x89,0x2d,0x70,0x53,0x40,0x00,0x39,0x6e,0x48,0x72,0xa7,0xeb,
    0x06,0x89,0x2d,0x70,0x53,0x40,0x00,0x39,0x6e,0x48,0x76,0x03,0x89,0x6e,0x48,0x33,
    0xc0,0x5d,0xc3,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,
    0x83,0xec,0x4c,0x8b,0x44,0x24,0x54,0x8b,0x4c,0x24,0x58,0xa3,0x78,0x53,0x40,0x00,
    0x53,0x8b,0x5c,0x24,0x54,0x8b,0x43,0x3c,0x8b,0x53,0x38,0x89,0x44,0x24,0x24,0x89,
    0x0d,0x7c,0x53,0x40,0x00,0x8b,0x4b,0x40,0x89,0x4c,0x24,0x20,0x8b,0x4b,0x08,0xb8,
    0x01,0x00,0x00,0x00,0xd3,0xe0,0x8b,0x4b,0x04,0x55,0x56,0x8b,0x73,0x20,0x48,0x89,
    0x44,0x24,0x54,0x57,0x8b,0xf8,0x8b,0x43,0x34,0x89,0x44,0x24,0x14,0xb8,0x01,0x00,
    0x00,0x00,0xd3,0xe0,0x8b,0x0b,0x89,0x54,0x24,0x20,0x8b,0x53,0x44,0x48,0x89,0x54,
    0x24,0x3c,0x8b,0x53,0x28,0x89,0x44,0x24,0x54,0x8b,0x43,0x14,0x89,0x4c,0x24,0x4c,
    0x8b,0x4b,0x2c,0x89,0x54,0x24,0x28,0x8b,0x53,0x24,0x89,0x44,0x24,0x34,0x8b,0x43,
    0x18,0x89,0x4c,0x24,0x1c,0x8b,0x4b,0x10,0x89,0x54,0x24,0x18,0x8b,0x53,0x30,0x89,
    0x44,0x24,0x10,0x8b,0x43,0x1c,0x89,0x1d,0x74,0x53,0x40,0x00,0x89,0x4c,0x24,0x38,
    0x89,0x54,0x24,0x44,0xc7,0x44,0x24,0x48,0x00,0x00,0x00,0x00,0x8d,0x64,0x24,0x00,
    0x8b,0x54,0x24,0x1c,0x23,0xd7,0x8b,0x7c,0x24,0x14,0xc1,0xe7,0x04,0x03,0xfa,0x8d,
    0x2c,0x79,0x89,0x54,0x24,0x40,0x0f,0xb7,0x55,0x00,0x3d,0x00,0x00,0x00,0x01,0x73,
    0x13,0x8b,0x7c,0x24,0x10,0x0f,0xb6,0x3f,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf7,
    0xff,0x44,0x24,0x10,0x8b,0xf8,0xc1,0xef,0x0b,0x0f,0xaf,0xfa,0x3b,0xf7,0x0f,0x83,
    0xd4,0x01,0x00,0x00,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,0xfa,0xc1,0xef,0x05,
    0x03,0xfa,0x83,0x7c,0x24,0x44,0x00,0x66,0x89,0x7d,0x00,0x8d,0xa9,0x6c,0x0e,0x00,
    0x00,0x89,0x6c,0x24,0x40,0x75,0x07,0x83,0x7c,0x24,0x1c,0x00,0x74,0x3c,0x8b,0x4c,
    0x24,0x18,0x85,0xc9,0x75,0x04,0x8b,0x4c,0x24,0x28,0x8b,0x54,0x24,0x34,0x0f,0xb6,
    0x54,0x11,0xff,0x8b,0x7c,0x24,0x1c,0x23,0x7c,0x24,0x54,0xb9,0x08,0x00,0x00,0x00,
    0x2a,0x4c,0x24,0x4c,0xd3,0xea,0x8b,0x4c,0x24,0x4c,0xd3,0xe7,0x03,0xd7,0x69,0xd2,
    0x00,0x06,0x00,0x00,0x03,0xea,0x89,0x6c,0x24,0x40,0x8b,0x4c,0x24,0x14,0x83,0xf9,
    0x07,0x0f,0x82,0xc6,0x00,0x00,0x00,0x8b,0x7c,0x24,0x20,0x8b,0x54,0x24,0x18,0x3b,
    0xd7,0x1b,0xed,0x23,0x6c,0x24,0x28,0xc7,0x44,0x24,0x14,0x00,0x01,0x00,0x00,0x2b,
    0xef,0x03,0x6b,0x14,0x83,0xf9,0x0a,0x0f,0xb6,0x14,0x2a,0x1b,0xff,0x83,0xe7,0xfd,
    0x83,0xc7,0x06,0x2b,0xcf,0x89,0x4c,0x24,0x24,0xb9,0x01,0x00,0x00,0x00,0x8b,0xff,
    0x8b,0x7c,0x24,0x14,0x03,0xd2,0x89,0x54,0x24,0x50,0x23,0xd7,0x8b,0xda,0x03,0xd1,
    0x03,0xd7,0x8b,0x7c,0x24,0x40,0x8d,0x2c,0x57,0x0f,0xb7,0x55,0x00,0x3d,0x00,0x00,
    0x00,0x01,0x73,0x13,0x8b,0x7c,0x24,0x10,0x0f,0xb6,0x3f,0xc1,0xe6,0x08,0xc1,0xe0,
    0x08,0x0b,0xf7,0xff,0x44,0x24,0x10,0x8b,0xf8,0xc1,0xef,0x0b,0x0f,0xaf,0xfa,0x3b,
    0xf7,0x72,0x15,0x2b,0xc7,0x2b,0xf7,0x8b,0xfa,0xc1,0xef,0x05,0x2b,0xd7,0x66,0x89,
    0x55,0x00,0x8d,0x4c,0x09,0x01,0xeb,0x16,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,
    0xfa,0xc1,0xef,0x05,0x03,0xfa,0x66,0x89,0x7d,0x00,0x03,0xc9,0xf7,0xd3,0x21,0x5c,
    0x24,0x14,0x81,0xf9,0x00,0x01,0x00,0x00,0x73,0x06,0x8b,0x54,0x24,0x50,0xeb,0x80,
    0x8b,0x54,0x24,0x24,0x89,0x54,0x24,0x14,0xe9,0x82,0x00,0x00,0x00,0x8b,0xd1,0x83,
    0xf9,0x04,0x72,0x05,0xba,0x03,0x00,0x00,0x00,0x2b,0xca,0x89,0x4c,0x24,0x14,0x89,
    0x4c,0x24,0x24,0xb9,0x01,0x00,0x00,0x00,0xeb,0x06,0x8d,0x9b,0x00,0x00,0x00,0x00,
    0x0f,0xb7,0x54,0x4d,0x00,0x3d,0x00,0x00,0x00,0x01,0x73,0x14,0x8b,0x7c,0x24,0x10,
    0x0f,0xb6,0x1f,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf3,0x47,0x89,0x7c,0x24,0x10,
    0x8b,0xf8,0xc1,0xef,0x0b,0x0f,0xaf,0xfa,0x3b,0xf7,0x72,0x16,0x2b,0xc7,0x2b,0xf7,
    0x8b,0xfa,0xc1,0xef,0x05,0x2b,0xd7,0x66,0x89,0x54,0x4d,0x00,0x8d,0x4c,0x09,0x01,
    0xeb,0x15,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,0xfa,0xc1,0xef,0x05,0x03,0xfa,
    0x66,0x89,0x7c,0x4d,0x00,0x03,0xc9,0x81,0xf9,0x00,0x01,0x00,0x00,0x72,0xa1,0x8b,
    0x54,0x24,0x18,0x8b,0x7c,0x24,0x34,0xff,0x44,0x24,0x1c,0x88,0x0c,0x3a,0x42,0x89,
    0x54,0x24,0x18,0xe9,0x83,0x09,0x00,0x00,0x8b,0x5c,0x24,0x10,0x2b,0xc7,0x2b,0xf7,
    0x8b,0xfa,0xc1,0xef,0x05,0x2b,0xd7,0x66,0x89,0x55,0x00,0x8b,0x6c,0x24,0x14,0x0f,
    0xb7,0xbc,0x69,0x80,0x01,0x00,0x00,0x3d,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,
    0x13,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf2,0x43,0x89,0x5c,0x24,0x10,0x8b,0xd0,
    0xc1,0xea,0x0b,0x0f,0xaf,0xd7,0x3b,0xf2,0x0f,0x82,0x5c,0x02,0x00,0x00,0x2b,0xc2,
    0x8b,0xe8,0x8b,0xc7,0xc1,0xe8,0x05,0x2b,0xf2,0x8b,0x54,0x24,0x14,0x2b,0xf8,0x83,
    0x7c,0x24,0x44,0x00,0x66,0x89,0xbc,0x51,0x80,0x01,0x00,0x00,0x75,0x0b,0x83,0x7c,
    0x24,0x1c,0x00,0x0f,0x84,0x37,0x09,0x00,0x00,0x8b,0x44,0x24,0x14,0x0f,0xb7,0x94,
    0x41,0x98,0x01,0x00,0x00,0x81,0xfd,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,0x03,
    0xc1,0xe6,0x08,0xc1,0xe5,0x08,0x0b,0xf0,0x43,0x89,0x5c,0x24,0x10,0x8b,0xc5,0xc1,
    0xe8,0x0b,0x0f,0xaf,0xc2,0x3b,0xf0,0x0f,0x82,0x30,0x01,0x00,0x00,0x2b,0xe8,0x2b,
    0xf0,0x8b,0xc2,0xc1,0xe8,0x05,0x2b,0xd0,0x8b,0x44,0x24,0x14,0x8b,0xfd,0x66,0x89,
    0x94,0x41,0x98,0x01,0x00,0x00,0x0f,0xb7,0x84,0x41,0xb0,0x01,0x00,0x00,0x81,0xff,
    0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,0x13,0xc1,0xe6,0x08,0xc1,0xe7,0x08,0x0b,
    0xf2,0x43,0x89,0x5c,0x24,0x10,0x8b,0xef,0xc1,0xed,0x0b,0x0f,0xaf,0xe8,0x3b,0xf5,
    0x0f,0x82,0xb8,0x00,0x00,0x00,0x2b,0xfd,0x8b,0xd7,0x8b,0xf8,0xc1,0xef,0x05,0x2b,
    0xc7,0x8b,0x7c,0x24,0x14,0x2b,0xf5,0x66,0x89,0x84,0x79,0xb0,0x01,0x00,0x00,0x0f,
    0xb7,0x84,0x79,0xc8,0x01,0x00,0x00,0x81,0xfa,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,
    0xb6,0x3b,0xc1,0xe6,0x08,0xc1,0xe2,0x08,0x0b,0xf7,0x43,0x89,0x5c,0x24,0x10,0x8b,
    0xfa,0xc1,0xef,0x0b,0x0f,0xaf,0xf8,0x3b,0xf7,0x72,0x3c,0x2b,0xd7,0x2b,0xf7,0x8b,
    0xf8,0xc1,0xef,0x05,0x2b,0xc7,0x8b,0x7c,0x24,0x14,0x66,0x89,0x84,0x79,0xc8,0x01,
    0x00,0x00,0x8b,0x44,0x24,0x2c,0x8b,0x7c,0x24,0x3c,0x89,0x44,0x24,0x3c,0x8b,0x44,
    0x24,0x30,0x89,0x44,0x24,0x2c,0x8b,0x44,0x24,0x20,0x89,0x44,0x24,0x30,0x89,0x7c,
    0x24,0x20,0xe9,0x1a,0x01,0x00,0x00,0xbd,0x00,0x08,0x00,0x00,0x2b,0xe8,0xc1,0xed,
    0x05,0x03,0xe8,0x8b,0x44,0x24,0x14,0x66,0x89,0xac,0x41,0xc8,0x01,0x00,0x00,0x8b,
    0x44,0x24,0x30,0x8b,0xd7,0x8b,0x7c,0x24,0x2c,0x89,0x44,0x24,0x2c,0x8b,0x44,0x24,
    0x20,0x89,0x44,0x24,0x30,0x89,0x7c,0x24,0x20,0xe9,0xe3,0x00,0x00,0x00,0x8b,0x7c,
    0x24,0x30,0x8b,0xd5,0xbd,0x00,0x08,0x00,0x00,0x2b,0xe8,0xc1,0xed,0x05,0x03,0xe8,
    0x8b,0x44,0x24,0x14,0x66,0x89,0xac,0x41,0xb0,0x01,0x00,0x00,0x8b,0x44,0x24,0x20,
    0x89,0x44,0x24,0x30,0x89,0x7c,0x24,0x20,0xe9,0xb4,0x00,0x00,0x00,0x8b,0x6c,0x24,
    0x14,0xbf,0x00,0x08,0x00,0x00,0x2b,0xfa,0xc1,0xef,0x05,0x03,0xfa,0x8d,0x55,0x0f,
    0xc1,0xe2,0x04,0x03,0x54,0x24,0x40,0x66,0x89,0xbc,0x69,0x98,0x01,0x00,0x00,0x0f,
    0xb7,0x3c,0x51,0x8d,0x1c,0x51,0x8b,0xd0,0x3d,0x00,0x00,0x00,0x01,0x73,0x15,0xc1,
    0xe0,0x08,0x8b,0xd0,0x8b,0x44,0x24,0x10,0x0f,0xb6,0x00,0xc1,0xe6,0x08,0x0b,0xf0,
    0xff,0x44,0x24,0x10,0x8b,0xc2,0xc1,0xe8,0x0b,0x0f,0xaf,0xc7,0x3b,0xf0,0x73,0x4f,
    0x8b,0x54,0x24,0x20,0xff,0x44,0x24,0x1c,0xb9,0x00,0x08,0x00,0x00,0x2b,0xcf,0xc1,
    0xe9,0x05,0x03,0xcf,0x8b,0x7c,0x24,0x34,0x66,0x89,0x0b,0x8b,0x4c,0x24,0x18,0x3b,
    0xca,0x1b,0xdb,0x23,0x5c,0x24,0x28,0x2b,0xda,0x03,0xd9,0x0f,0xb6,0x14,0x3b,0x88,
    0x14,0x39,0x83,0xfd,0x07,0x1b,0xd2,0x83,0xe2,0xfe,0x83,0xc2,0x0b,0x41,0x89,0x54,
    0x24,0x14,0x89,0x4c,0x24,0x18,0x89,0x54,0x24,0x24,0xe9,0x10,0x07,0x00,0x00,0x2b,
    0xd0,0x2b,0xf0,0x8b,0xc7,0xc1,0xe8,0x05,0x2b,0xf8,0x66,0x89,0x3b,0x8b,0x5c,0x24,
    0x10,0x83,0x7c,0x24,0x14,0x07,0x1b,0xc0,0x83,0xe0,0xfd,0x83,0xc0,0x0b,0x89,0x44,
    0x24,0x14,0x81,0xc1,0x68,0x0a,0x00,0x00,0xeb,0x21,0xb8,0x00,0x08,0x00,0x00,0x2b,
    0xc7,0xc1,0xe8,0x05,0x03,0xc7,0x66,0x89,0x84,0x69,0x80,0x01,0x00,0x00,0x83,0xc5,
    0x0c,0x89,0x6c,0x24,0x14,0x81,0xc1,0x64,0x06,0x00,0x00,0x8b,0x44,0x24,0x14,0x0f,
    0xb7,0x39,0x89,0x44,0x24,0x24,0x81,0xfa,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,
    0x03,0xc1,0xe6,0x08,0xc1,0xe2,0x08,0x0b,0xf0,0x43,0x89,0x5c,0x24,0x10,0x8b,0xc2,
    0xc1,0xe8,0x0b,0x0f,0xaf,0xc7,0x3b,0xf0,0x0f,0x82,0x87,0x00,0x00,0x00,0x2b,0xd0,
    0x2b,0xf0,0x8b,0xc7,0xc1,0xe8,0x05,0x2b,0xf8,0x66,0x89,0x39,0x0f,0xb7,0x79,0x02,
    0x81,0xfa,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,0x03,0xc1,0xe6,0x08,0xc1,0xe2,
    0x08,0x0b,0xf0,0x43,0x89,0x5c,0x24,0x10,0x8b,0xea,0xc1,0xed,0x0b,0x0f,0xaf,0xef,
    0x3b,0xf5,0x72,0x26,0x2b,0xd5,0x8b,0xc2,0x8b,0xd7,0x2b,0xf5,0xc1,0xea,0x05,0x2b,
    0xfa,0x66,0x89,0x79,0x02,0x8d,0xa9,0x04,0x02,0x00,0x00,0xc7,0x44,0x24,0x40,0x10,
    0x00,0x00,0x00,0xbb,0x00,0x01,0x00,0x00,0xeb,0x52,0xba,0x00,0x08,0x00,0x00,0x2b,
    0xd7,0x8b,0xc5,0x8b,0x6c,0x24,0x40,0xc1,0xea,0x05,0x03,0xd7,0xc1,0xe5,0x04,0xbb,
    0x08,0x00,0x00,0x00,0x66,0x89,0x51,0x02,0x8d,0xac,0x29,0x04,0x01,0x00,0x00,0x89,
    0x5c,0x24,0x40,0xeb,0x27,0x8b,0x6c,0x24,0x40,0xba,0x00,0x08,0x00,0x00,0x2b,0xd7,
    0xc1,0xea,0x05,0x03,0xd7,0xc1,0xe5,0x04,0x66,0x89,0x11,0x8d,0x6c,0x29,0x04,0xc7,
    0x44,0x24,0x40,0x00,0x00,0x00,0x00,0xbb,0x08,0x00,0x00,0x00,0xb9,0x01,0x00,0x00,
    0x00,0x0f,0xb7,0x54,0x4d,0x00,0x3d,0x00,0x00,0x00,0x01,0x73,0x13,0x8b,0x7c,0x24,
    0x10,0x0f,0xb6,0x3f,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf7,0xff,0x44,0x24,0x10,
    0x8b,0xf8,0xc1,0xef,0x0b,0x0f,0xaf,0xfa,0x3b,0xf7,0x72,0x16,0x2b,0xc7,0x2b,0xf7,
    0x8b,0xfa,0xc1,0xef,0x05,0x2b,0xd7,0x66,0x89,0x54,0x4d,0x00,0x8d,0x4c,0x09,0x01,
    0xeb,0x15,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,0xfa,0xc1,0xef,0x05,0x03,0xfa,
    0x66,0x89,0x7c,0x4d,0x00,0x03,0xc9,0x3b,0xcb,0x72,0xa6,0x2b,0xcb,0x03,0x4c,0x24,
    0x40,0x83,0x7c,0x24,0x14,0x0c,0x89,0x4c,0x24,0x50,0x0f,0x82,0xd1,0x04,0x00,0x00,
    0x83,0xf9,0x04,0x72,0x05,0xb9,0x03,0x00,0x00,0x00,0x8b,0x54,0x24,0x38,0x8b,0x5c,
    0x24,0x10,0xc1,0xe1,0x07,0x8d,0x8c,0x11,0x60,0x03,0x00,0x00,0x0f,0xb7,0x51,0x02,
    0x3d,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,0x3b,0xc1,0xe6,0x08,0xc1,0xe0,0x08,
    0x0b,0xf7,0x43,0x89,0x5c,0x24,0x10,0x8b,0xf8,0xc1,0xef,0x0b,0x0f,0xaf,0xfa,0x3b,
    0xf7,0x72,0x16,0x2b,0xc7,0x2b,0xf7,0x8b,0xfa,0xc1,0xef,0x05,0x2b,0xd7,0x66,0x89,
    0x51,0x02,0xba,0x03,0x00,0x00,0x00,0xeb,0x17,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,
    0x2b,0xfa,0xc1,0xef,0x05,0x03,0xfa,0x66,0x89,0x79,0x02,0xba,0x02,0x00,0x00,0x00,
    0x8d,0x2c,0x12,0x0f,0xb7,0x14,0x29,0x3d,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,
    0x3b,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf7,0x43,0x89,0x5c,0x24,0x10,0x8b,0xf8,
    0xc1,0xef,0x0b,0x0f,0xaf,0xfa,0x3b,0xf7,0x72,0x12,0x2b,0xc7,0x2b,0xf7,0x8b,0xfa,
    0xc1,0xef,0x05,0x2b,0xd7,0x66,0x89,0x14,0x29,0x45,0xeb,0x12,0x8b,0xc7,0xbf,0x00,
    0x08,0x00,0x00,0x2b,0xfa,0xc1,0xef,0x05,0x03,0xfa,0x66,0x89,0x3c,0x29,0x03,0xed,
    0x0f,0xb7,0x14,0x29,0x3d,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,0x3b,0xc1,0xe6,
    0x08,0xc1,0xe0,0x08,0x0b,0xf7,0x43,0x89,0x5c,0x24,0x10,0x8b,0xf8,0xc1,0xef,0x0b,
    0x0f,0xaf,0xfa,0x3b,0xf7,0x72,0x12,0x2b,0xc7,0x2b,0xf7,0x8b,0xfa,0xc1,0xef,0x05,
    0x2b,0xd7,0x66,0x89,0x14,0x29,0x45,0xeb,0x12,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,
    0x2b,0xfa,0xc1,0xef,0x05,0x03,0xfa,0x66,0x89,0x3c,0x29,0x03,0xed,0x0f,0xb7,0x14,
    0x29,0x3d,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,0x3b,0xc1,0xe6,0x08,0xc1,0xe0,
    0x08,0x0b,0xf7,0x43,0x89,0x5c,0x24,0x10,0x8b,0xf8,0xc1,0xef,0x0b,0x0f,0xaf,0xfa,
    0x3b,0xf7,0x72,0x12,0x2b,0xc7,0x2b,0xf7,0x8b,0xfa,0xc1,0xef,0x05,0x2b,0xd7,0x66,
    0x89,0x14,0x29,0x45,0xeb,0x12,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,0xfa,0xc1,
    0xef,0x05,0x03,0xfa,0x66,0x89,0x3c,0x29,0x03,0xed,0x0f,0xb7,0x14,0x29,0x3d,0x00,
    0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,0x3b,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf7,
    0x43,0x89,0x5c,0x24,0x10,0x8b,0xf8,0xc1,0xef,0x0b,0x0f,0xaf,0xfa,0x3b,0xf7,0x72,
    0x12,0x2b,0xc7,0x2b,0xf7,0x8b,0xfa,0xc1,0xef,0x05,0x2b,0xd7,0x66,0x89,0x14,0x29,
    0x45,0xeb,0x12,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,0xfa,0xc1,0xef,0x05,0x03,
    0xfa,0x66,0x89,0x3c,0x29,0x03,0xed,0x0f,0xb7,0x14,0x29,0x3d,0x00,0x00,0x00,0x01,
    0x73,0x10,0x0f,0xb6,0x3b,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf7,0x43,0x89,0x5c,
    0x24,0x10,0x8b,0xf8,0xc1,0xef,0x0b,0x0f,0xaf,0xfa,0x3b,0xf7,0x72,0x12,0x2b,0xc7,
    0x2b,0xf7,0x8b,0xfa,0xc1,0xef,0x05,0x2b,0xd7,0x66,0x89,0x14,0x29,0x45,0xeb,0x12,
    0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,0xfa,0xc1,0xef,0x05,0x03,0xfa,0x66,0x89,
    0x3c,0x29,0x83,0xc5,0xc0,0x83,0xfd,0x04,0x0f,0x82,0x82,0x02,0x00,0x00,0x8b,0xcd,
    0x8b,0xd5,0xd1,0xe9,0x83,0xe2,0x01,0x49,0x83,0xca,0x02,0x8b,0xfd,0x89,0x4c,0x24,
    0x48,0x83,0xfd,0x0e,0x0f,0x83,0x9c,0x00,0x00,0x00,0xd3,0xe2,0x8b,0xea,0x8b,0xcd,
    0x2b,0xcf,0x8b,0x7c,0x24,0x38,0xba,0x01,0x00,0x00,0x00,0x89,0x54,0x24,0x40,0x8d,
    0x9c,0x4f,0x5e,0x05,0x00,0x00,0xeb,0x08,0x8d,0xa4,0x24,0x00,0x00,0x00,0x00,0x90,
    0x0f,0xb7,0x0c,0x53,0x3d,0x00,0x00,0x00,0x01,0x73,0x13,0x8b,0x7c,0x24,0x10,0x0f,
    0xb6,0x3f,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf7,0xff,0x44,0x24,0x10,0x8b,0xf8,
    0xc1,0xef,0x0b,0x0f,0xaf,0xf9,0x3b,0xf7,0x72,0x19,0x0b,0x6c,0x24,0x40,0x2b,0xc7,
    0x2b,0xf7,0x8b,0xf9,0xc1,0xef,0x05,0x2b,0xcf,0x66,0x89,0x0c,0x53,0x8d,0x54,0x12,
    0x01,0xeb,0x14,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,0xf9,0xc1,0xef,0x05,0x03,
    0xf9,0x66,0x89,0x3c,0x53,0x03,0xd2,0x8b,0x4c,0x24,0x40,0x03,0xc9,0x89,0x4c,0x24,
    0x40,0x8b,0x4c,0x24,0x48,0x8b,0xf9,0x49,0x89,0x4c,0x24,0x48,0x83,0xff,0x01,0x75,
    0x8f,0xe9,0xca,0x01,0x00,0x00,0x8b,0x7c,0x24,0x10,0x8d,0x59,0xfc,0x8d,0x49,0x00,
    0x3d,0x00,0x00,0x00,0x01,0x73,0x0c,0x0f,0xb6,0x0f,0xc1,0xe6,0x08,0xc1,0xe0,0x08,
    0x0b,0xf1,0x47,0xd1,0xe8,0x2b,0xf0,0x8b,0xce,0xc1,0xe9,0x1f,0x03,0xd2,0x2b,0xd1,
    0xf7,0xd9,0x23,0xc8,0x03,0xce,0x42,0x83,0xeb,0x01,0x8b,0xf1,0x75,0xd2,0x8b,0x5c,
    0x24,0x38,0x0f,0xb7,0x8b,0x46,0x06,0x00,0x00,0xc1,0xe2,0x04,0x8b,0xea,0x89,0x7c,
    0x24,0x10,0x3d,0x00,0x00,0x00,0x01,0x73,0x10,0x0f,0xb6,0x17,0xc1,0xe6,0x08,0xc1,
    0xe0,0x08,0x0b,0xf2,0x47,0x89,0x7c,0x24,0x10,0x8b,0xd0,0xc1,0xea,0x0b,0x0f,0xaf,
    0xd1,0x3b,0xf2,0x72,0x1c,0x2b,0xc2,0x2b,0xf2,0x8b,0xd1,0xc1,0xea,0x05,0x2b,0xca,
    0x66,0x89,0x8b,0x46,0x06,0x00,0x00,0xb9,0x03,0x00,0x00,0x00,0x83,0xcd,0x01,0xeb,
    0x1a,0x8b,0xc2,0xba,0x00,0x08,0x00,0x00,0x2b,0xd1,0xc1,0xea,0x05,0x03,0xd1,0x66,
    0x89,0x93,0x46,0x06,0x00,0x00,0xb9,0x02,0x00,0x00,0x00,0x8d,0x3c,0x09,0x0f,0xb7,
    0x8c,0x4b,0x44,0x06,0x00,0x00,0x3d,0x00,0x00,0x00,0x01,0x73,0x13,0x8b,0x54,0x24,
    0x10,0x0f,0xb6,0x12,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf2,0xff,0x44,0x24,0x10,
    0x8b,0xd0,0xc1,0xea,0x0b,0x0f,0xaf,0xd1,0x3b,0xf2,0x72,0x19,0x2b,0xc2,0x2b,0xf2,
    0x8b,0xd1,0xc1,0xea,0x05,0x2b,0xca,0x66,0x89,0x8c,0x1f,0x44,0x06,0x00,0x00,0x47,
    0x83,0xcd,0x02,0xeb,0x16,0x8b,0xc2,0xba,0x00,0x08,0x00,0x00,0x2b,0xd1,0xc1,0xea,
    0x05,0x03,0xd1,0x66,0x89,0x94,0x1f,0x44,0x06,0x00,0x00,0x8d,0x14,0x3f,0x0f,0xb7,
    0x8c,0x1a,0x44,0x06,0x00,0x00,0x3d,0x00,0x00,0x00,0x01,0x73,0x13,0x8b,0x7c,0x24,
    0x10,0x0f,0xb6,0x3f,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf7,0xff,0x44,0x24,0x10,
    0x8b,0xf8,0xc1,0xef,0x0b,0x0f,0xaf,0xf9,0x3b,0xf7,0x72,0x19,0x2b,0xc7,0x2b,0xf7,
    0x8b,0xf9,0xc1,0xef,0x05,0x2b,0xcf,0x66,0x89,0x8c,0x1a,0x44,0x06,0x00,0x00,0x42,
    0x83,0xcd,0x04,0xeb,0x16,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,0xf9,0xc1,0xef,
    0x05,0x03,0xf9,0x66,0x89,0xbc,0x1a,0x44,0x06,0x00,0x00,0x0f,0xb7,0x8c,0x53,0x44,
    0x06,0x00,0x00,0x3d,0x00,0x00,0x00,0x01,0x73,0x13,0x8b,0x7c,0x24,0x10,0x0f,0xb6,
    0x3f,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf7,0xff,0x44,0x24,0x10,0x8b,0xf8,0xc1,
    0xef,0x0b,0x0f,0xaf,0xf9,0x3b,0xf7,0x72,0x18,0x2b,0xc7,0x2b,0xf7,0x8b,0xf9,0xc1,
    0xef,0x05,0x2b,0xcf,0x66,0x89,0x8c,0x53,0x44,0x06,0x00,0x00,0x83,0xcd,0x08,0xeb,
    0x16,0x8b,0xc7,0xbf,0x00,0x08,0x00,0x00,0x2b,0xf9,0xc1,0xef,0x05,0x03,0xf9,0x66,
    0x89,0xbc,0x53,0x44,0x06,0x00,0x00,0x83,0xfd,0xff,0x0f,0x84,0x1d,0x01,0x00,0x00,
    0x8b,0x54,0x24,0x2c,0x8b,0x4c,0x24,0x30,0x89,0x54,0x24,0x3c,0x8b,0x54,0x24,0x20,
    0x89,0x4c,0x24,0x2c,0x8b,0x4c,0x24,0x44,0x89,0x54,0x24,0x30,0x8d,0x55,0x01,0x89,
    0x54,0x24,0x20,0x85,0xc9,0x74,0x04,0x3b,0xe9,0xeb,0x04,0x3b,0x6c,0x24,0x1c,0x0f,
    0x92,0xc1,0x84,0xc9,0x0f,0x84,0xd6,0x00,0x00,0x00,0x83,0x7c,0x24,0x14,0x13,0x1b,
    0xc9,0x83,0xe1,0xfd,0x83,0xc1,0x0a,0x89,0x4c,0x24,0x24,0x89,0x4c,0x24,0x14,0xeb,
    0x04,0x8b,0x54,0x24,0x20,0x8b,0x6c,0x24,0x50,0x8b,0x7c,0x24,0x64,0x8b,0x5c,0x24,
    0x18,0x83,0xc5,0x02,0x3b,0xfb,0x0f,0x84,0xa4,0x00,0x00,0x00,0x2b,0xfb,0x3b,0xfd,
    0x72,0x08,0x8b,0xfd,0x89,0x6c,0x24,0x40,0xeb,0x04,0x89,0x7c,0x24,0x40,0x01,0x7c,
    0x24,0x1c,0x3b,0xda,0x1b,0xc9,0x23,0x4c,0x24,0x28,0x2b,0xef,0x2b,0xca,0x8d,0x14,
    0x19,0x89,0x6c,0x24,0x48,0x8d,0x2c,0x3a,0x89,0x5c,0x24,0x50,0x3b,0x6c,0x24,0x28,
    0x76,0x2c,0x8b,0x6c,0x24,0x34,0x8b,0xfb,0x8a,0x0c,0x2a,0x88,0x0c,0x2f,0x42,0x47,
    0x3b,0x54,0x24,0x28,0x75,0x02,0x33,0xd2,0x8b,0x4c,0x24,0x40,0x8b,0xd9,0x49,0x89,
    0x4c,0x24,0x40,0x83,0xfb,0x01,0x75,0xe0,0x89,0x7c,0x24,0x18,0xeb,0x1d,0x8b,0x54,
    0x24,0x34,0x8d,0x2c,0x13,0x03,0xdf,0x89,0x5c,0x24,0x18,0x03,0xfd,0x8d,0x49,0x00,
    0x8a,0x14,0x29,0x88,0x55,0x00,0x45,0x3b,0xef,0x75,0xf5,0x8b,0x4c,0x24,0x18,0x3b,
    0x4c,0x24,0x64,0x73,0x41,0x8b,0x4c,0x24,0x10,0x3b,0x4c,0x24,0x68,0x7d,0x37,0x8b,
    0x4c,0x24,0x38,0x8b,0x7c,0x24,0x58,0x8b,0x5c,0x24,0x60,0xe9,0x40,0xf4,0xff,0xff,
    0x5f,0x5e,0x5d,0xb8,0x01,0x00,0x00,0x00,0x5b,0x83,0xc4,0x4c,0xc3,0x8b,0x54,0x24,
    0x50,0x8b,0x4c,0x24,0x14,0x81,0xc2,0x12,0x01,0x00,0x00,0x83,0xc1,0xf4,0x89,0x54,
    0x24,0x48,0x89,0x4c,0x24,0x24,0x8b,0x54,0x24,0x10,0x3d,0x00,0x00,0x00,0x01,0x73,
    0x0c,0x0f,0xb6,0x0a,0xc1,0xe6,0x08,0xc1,0xe0,0x08,0x0b,0xf1,0x42,0x8b,0x4c,0x24,
    0x60,0x89,0x41,0x1c,0x8b,0x44,0x24,0x48,0x89,0x51,0x18,0x8b,0x54,0x24,0x18,0x89,
    0x41,0x48,0x8b,0x44,0x24,0x1c,0x89,0x51,0x24,0x8b,0x54,0x24,0x20,0x89,0x41,0x2c,
    0x8b,0x44,0x24,0x30,0x5f,0x89,0x51,0x38,0x8b,0x54,0x24,0x28,0x89,0x71,0x20,0x89,
    0x41,0x3c,0x8b,0x44,0x24,0x38,0x5e,0x89,0x51,0x40,0x8b,0x54,0x24,0x1c,0x5d,0x89,
    0x41,0x44,0x89,0x51,0x34,0x33,0xc0,0x5b,0x83,0xc4,0x4c,0xc3,0xcc,0xcc,0xcc,0xcc,
    0x51,0x8b,0x4c,0x24,0x08,0x55,0x56,0x8b,0x74,0x24,0x14,0x33,0xed,0x55,0x8d,0x44,
    0x24,0x0c,0x50,0x56,0x51,0x52,0xff,0x15,0x14,0x40,0x40,0x00,0x85,0xc0,0x75,0x16,
    0x68,0xec,0x41,0x40,0x00,0xff,0x15,0xbc,0x40,0x40,0x00,0x83,0xc4,0x04,0x6a,0xff,
    0xff,0x15,0x10,0x40,0x40,0x00,0x8b,0x4c,0x24,0x10,0xe8,0x31,0xef,0xff,0xff,0x68,
    0x6c,0x3e,0x00,0x00,0xc7,0x07,0x03,0x00,0x00,0x00,0x89,0x6f,0x04,0xc7,0x47,0x08,
    0x02,0x00,0x00,0x00,0xc7,0x47,0x0c,0x00,0x00,0x01,0x00,0xc7,0x47,0x54,0x36,0x1f,
    0x00,0x00,0xe8,0xad,0x06,0x00,0x00,0x53,0x89,0x47,0x10,0xe8,0xa4,0x06,0x00,0x00,
    0x53,0x8b,0xf0,0x55,0x56,0xe8,0x84,0x0e,0x00,0x00,0x8b,0x4c,0x24,0x24,0xb8,0x01,
    0x00,0x00,0x00,0x89,0x47,0x4c,0x89,0x47,0x50,0x8d,0x44,0x24,0x28,0x50,0x51,0x53,
    0x57,0x89,0x77,0x14,0x89,0x5f,0x28,0x89,0x6f,0x24,0x89,0x6f,0x48,0x89,0x6f,0x58,
    0x89,0x6f,0x2c,0x89,0x6f,0x30,0xe8,0x65,0xef,0xff,0xff,0x83,0xc4,0x24,0x5e,0x5d,
    0x59,0xc3,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,
    0x83,0xec,0x18,0x8b,0x50,0x34,0x53,0x55,0x8b,0x6c,0x24,0x24,0x03,0xcd,0x89,0x4c,
    0x24,0x24,0x8b,0x48,0x10,0x89,0x4c,0x24,0x08,0x8b,0x48,0x08,0xbb,0x01,0x00,0x00,
    0x00,0xd3,0xe3,0x8b,0x4c,0x24,0x08,0x56,0x8b,0x70,0x20,0x4b,0x23,0x58,0x2c,0x89,
    0x54,0x24,0x14,0xc1,0xe2,0x04,0x57,0x8b,0x78,0x1c,0x03,0xd3,0x0f,0xb7,0x0c,0x51,
    0x81,0xff,0x00,0x00,0x00,0x01,0x73,0x1d,0x3b,0x6c,0x24,0x2c,0x72,0x0a,0x5f,0x5e,
    0x5d,0x33,0xc0,0x5b,0x83,0xc4,0x18,0xc3,0x0f,0xb6,0x55,0x00,0xc1,0xe6,0x08,0xc1,
    0xe7,0x08,0x0b,0xf2,0x45,0x8b,0xd7,0xc1,0xea,0x0b,0x0f,0xaf,0xd1,0x3b,0xf2,0x0f,
    0x83,0x4d,0x01,0x00,0x00,0x8b,0x5c,0x24,0x10,0x81,0xc3,0x6c,0x0e,0x00,0x00,0x83,
    0x78,0x30,0x00,0x89,0x5c,0x24,0x14,0x75,0x06,0x83,0x78,0x2c,0x00,0x74,0x3f,0x8b,
    0x48,0x24,0x85,0xc9,0x75,0x03,0x8b,0x48,0x28,0x8b,0x78,0x14,0x0f,0xb6,0x7c,0x0f,
    0xff,0xb9,0x08,0x00,0x00,0x00,0x2a,0x08,0xbb,0x01,0x00,0x00,0x00,0xd3,0xef,0x8b,
    0x48,0x04,0xd3,0xe3,0x8b,0x08,0x4b,0x23,0x58,0x2c,0xd3,0xe3,0x03,0xfb,0x8b,0x5c,
    0x24,0x14,0x69,0xff,0x00,0x06,0x00,0x00,0x03,0xdf,0x89,0x5c,0x24,0x14,0x83,0x7c,
    0x24,0x18,0x07,0x0f,0x82,0x92,0x00,0x00,0x00,0x8b,0x78,0x24,0x8b,0x58,0x38,0x3b,
    0xfb,0x72,0x04,0x33,0xc9,0xeb,0x03,0x8b,0x48,0x28,0x8b,0x40,0x14,0x2b,0xc3,0x03,
    0xc1,0x0f,0xb6,0x04,0x38,0xbf,0x00,0x01,0x00,0x00,0xb9,0x01,0x00,0x00,0x00,0x90,
    0x8b,0x5c,0x24,0x14,0x03,0xc0,0x89,0x44,0x24,0x24,0x23,0xc7,0x89,0x44,0x24,0x20,
    0x03,0xc1,0x03,0xc7,0x0f,0xb7,0x1c,0x43,0x81,0xfa,0x00,0x00,0x00,0x01,0x73,0x17,
    0x3b,0x6c,0x24,0x2c,0x0f,0x83,0x24,0xff,0xff,0xff,0x0f,0xb6,0x45,0x00,0xc1,0xe6,
    0x08,0xc1,0xe2,0x08,0x0b,0xf0,0x45,0x8b,0xc2,0xc1,0xe8,0x0b,0x0f,0xaf,0xc3,0x3b,
    0xf0,0x72,0x0e,0x2b,0xd0,0x2b,0xf0,0x8b,0x44,0x24,0x20,0x8d,0x4c,0x09,0x01,0xeb,
    0x0a,0x8b,0xd0,0x8b,0x44,0x24,0x20,0x03,0xc9,0xf7,0xd0,0x23,0xf8,0x81,0xf9,0x00,
    0x01,0x00,0x00,0x73,0x50,0x8b,0x44,0x24,0x24,0xeb,0x95,0xb9,0x01,0x00,0x00,0x00,
    0x0f,0xb7,0x3c,0x4b,0x81,0xfa,0x00,0x00,0x00,0x01,0x73,0x17,0x3b,0x6c,0x24,0x2c,
    0x0f,0x83,0xc8,0xfe,0xff,0xff,0x0f,0xb6,0x45,0x00,0xc1,0xe6,0x08,0xc1,0xe2,0x08,
    0x0b,0xf0,0x45,0x8b,0xc2,0xc1,0xe8,0x0b,0x0f,0xaf,0xc7,0x3b,0xf0,0x72,0x0a,0x2b,
    0xd0,0x2b,0xf0,0x8d,0x4c,0x09,0x01,0xeb,0x04,0x8b,0xd0,0x03,0xc9,0x81,0xf9,0x00,
    0x01,0x00,0x00,0x72,0xbb,0xc7,0x44,0x24,0x20,0x01,0x00,0x00,0x00,0xe9,0xe8,0x03,
    0x00,0x00,0x8b,0x4c,0x24,0x18,0x2b,0xfa,0x2b,0xf2,0x8b,0x54,0x24,0x10,0x0f,0xb7,
    0x8c,0x4a,0x80,0x01,0x00,0x00,0x81,0xff,0x00,0x00,0x00,0x01,0x73,0x17,0x3b,0x6c,
    0x24,0x2c,0x0f,0x83,0x66,0xfe,0xff,0xff,0x0f,0xb6,0x45,0x00,0xc1,0xe6,0x08,0xc1,
    0xe7,0x08,0x0b,0xf0,0x45,0x8b,0xc7,0xc1,0xe8,0x0b,0x0f,0xaf,0xc1,0x3b,0xf0,0x73,
    0x1f,0x81,0xc2,0x64,0x06,0x00,0x00,0xc7,0x44,0x24,0x24,0x00,0x00,0x00,0x00,0x89,
    0x54,0x24,0x14,0xc7,0x44,0x24,0x20,0x02,0x00,0x00,0x00,0xe9,0x49,0x01,0x00,0x00,
    0x2b,0xf8,0x2b,0xf0,0xc7,0x44,0x24,0x20,0x03,0x00,0x00,0x00,0x81,0xff,0x00,0x00,
    0x00,0x01,0x73,0x17,0x3b,0x6c,0x24,0x2c,0x0f,0x83,0x10,0xfe,0xff,0xff,0x0f,0xb6,
    0x4d,0x00,0xc1,0xe6,0x08,0xc1,0xe7,0x08,0x0b,0xf1,0x45,0x8b,0x44,0x24,0x10,0x8b,
    0x54,0x24,0x18,0x0f,0xb7,0x8c,0x50,0x98,0x01,0x00,0x00,0x8b,0xc7,0xc1,0xe8,0x0b,
    0x0f,0xaf,0xc8,0x3b,0xf1,0x73,0x5e,0x81,0xf9,0x00,0x00,0x00,0x01,0x73,0x17,0x3b,
    0x6c,0x24,0x2c,0x0f,0x83,0xd5,0xfd,0xff,0xff,0x0f,0xb6,0x45,0x00,0xc1,0xe6,0x08,
    0xc1,0xe1,0x08,0x0b,0xf0,0x45,0x8b,0x44,0x24,0x10,0x83,0xc2,0x0f,0xc1,0xe2,0x04,
    0x03,0xd3,0x0f,0xb7,0x3c,0x50,0x8b,0xd1,0xc1,0xea,0x0b,0x0f,0xaf,0xfa,0x3b,0xf7,
    0x0f,0x83,0xa5,0x00,0x00,0x00,0x81,0xff,0x00,0x00,0x00,0x01,0x73,0x0a,0x3b,0x6c,
    0x24,0x2c,0x0f,0x83,0x96,0xfd,0xff,0xff,0x5f,0x5e,0x5d,0xb8,0x03,0x00,0x00,0x00,
    0x5b,0x83,0xc4,0x18,0xc3,0x8b,0x44,0x24,0x10,0x2b,0xf9,0x2b,0xf1,0x0f,0xb7,0x8c,
    0x50,0xb0,0x01,0x00,0x00,0x89,0x6c,0x24,0x1c,0x81,0xff,0x00,0x00,0x00,0x01,0x73,
    0x1b,0x3b,0x6c,0x24,0x2c,0x0f,0x83,0x63,0xfd,0xff,0xff,0x0f,0xb6,0x45,0x00,0xc1,
    0xe6,0x08,0xc1,0xe7,0x08,0x0b,0xf0,0x45,0x89,0x6c,0x24,0x1c,0x8b,0xc7,0xc1,0xe8,
    0x0b,0x0f,0xaf,0xc1,0x3b,0xf0,0x72,0x49,0x8b,0x4c,0x24,0x10,0x0f,0xb7,0x94,0x51,
    0xc8,0x01,0x00,0x00,0x2b,0xf8,0x8b,0xcf,0x2b,0xf0,0x81,0xf9,0x00,0x00,0x00,0x01,
    0x73,0x1b,0x8b,0x6c,0x24,0x1c,0x3b,0x6c,0x24,0x2c,0x0f,0x83,0x1e,0xfd,0xff,0xff,
    0x0f,0xb6,0x45,0x00,0xc1,0xe6,0x08,0xc1,0xe1,0x08,0x0b,0xf0,0x45,0x8b,0xc1,0xc1,
    0xe8,0x0b,0x0f,0xaf,0xc2,0x8b,0xf8,0x3b,0xf7,0x72,0x06,0x2b,0xcf,0x8b,0xc1,0x2b,
    0xf7,0x8b,0x4c,0x24,0x10,0x81,0xc1,0x68,0x0a,0x00,0x00,0xc7,0x44,0x24,0x24,0x0c,
    0x00,0x00,0x00,0x89,0x4c,0x24,0x14,0x8b,0xd1,0x3d,0x00,0x00,0x00,0x01,0x73,0x17,
    0x3b,0x6c,0x24,0x2c,0x0f,0x83,0xd4,0xfc,0xff,0xff,0x0f,0xb6,0x4d,0x00,0xc1,0xe6,
    0x08,0xc1,0xe0,0x08,0x0b,0xf1,0x45,0x0f,0xb7,0x3a,0x8b,0xc8,0xc1,0xe9,0x0b,0x0f,
    0xaf,0xcf,0x3b,0xf1,0x72,0x71,0x2b,0xc1,0x8b,0xf8,0x2b,0xf1,0x81,0xff,0x00,0x00,
    0x00,0x01,0x73,0x17,0x3b,0x6c,0x24,0x2c,0x0f,0x83,0xa0,0xfc,0xff,0xff,0x0f,0xb6,
    0x45,0x00,0xc1,0xe6,0x08,0xc1,0xe7,0x08,0x0b,0xf0,0x45,0x0f,0xb7,0x52,0x02,0x8b,
    0x44,0x24,0x14,0x8b,0xcf,0xc1,0xe9,0x0b,0x0f,0xaf,0xca,0x3b,0xf1,0x72,0x1d,0x2b,
    0xf9,0x2b,0xf1,0x8b,0xd7,0x05,0x04,0x02,0x00,0x00,0xc7,0x44,0x24,0x1c,0x10,0x00,
    0x00,0x00,0xc7,0x44,0x24,0x18,0x00,0x01,0x00,0x00,0xeb,0x38,0xbf,0x08,0x00,0x00,
    0x00,0xc1,0xe3,0x04,0x89,0x7c,0x24,0x1c,0x8b,0xd1,0x8d,0x84,0x03,0x04,0x01,0x00,
    0x00,0x89,0x7c,0x24,0x18,0xeb,0x1d,0x8b,0xd1,0x8b,0x4c,0x24,0x14,0xc1,0xe3,0x04,
    0x8d,0x44,0x0b,0x04,0xc7,0x44,0x24,0x1c,0x00,0x00,0x00,0x00,0xc7,0x44,0x24,0x18,
    0x08,0x00,0x00,0x00,0xbf,0x01,0x00,0x00,0x00,0x8d,0xa4,0x24,0x00,0x00,0x00,0x00,
    0x81,0xfa,0x00,0x00,0x00,0x01,0x73,0x17,0x3b,0x6c,0x24,0x2c,0x0f,0x83,0x0c,0xfc,
    0xff,0xff,0x0f,0xb6,0x4d,0x00,0xc1,0xe6,0x08,0xc1,0xe2,0x08,0x0b,0xf1,0x45,0x0f,
    0xb7,0x1c,0x78,0x8b,0xca,0xc1,0xe9,0x0b,0x0f,0xaf,0xcb,0x3b,0xf1,0x72,0x0a,0x2b,
    0xd1,0x2b,0xf1,0x8d,0x7c,0x3f,0x01,0xeb,0x04,0x8b,0xd1,0x03,0xff,0x8b,0x4c,0x24,
    0x18,0x3b,0xf9,0x72,0xbb,0x2b,0xf9,0x03,0x7c,0x24,0x1c,0x83,0x7c,0x24,0x24,0x04,
    0x0f,0x83,0x24,0x01,0x00,0x00,0x83,0xff,0x04,0x72,0x05,0xbf,0x03,0x00,0x00,0x00,
    0x8b,0x44,0x24,0x10,0xc1,0xe7,0x07,0x8d,0x9c,0x07,0x60,0x03,0x00,0x00,0xbf,0x01,
    0x00,0x00,0x00,0x81,0xfa,0x00,0x00,0x00,0x01,0x73,0x17,0x3b,0x6c,0x24,0x2c,0x0f,
    0x83,0x99,0xfb,0xff,0xff,0x0f,0xb6,0x4d,0x00,0xc1,0xe6,0x08,0xc1,0xe2,0x08,0x0b,
    0xf1,0x45,0x0f,0xb7,0x04,0x7b,0x8b,0xca,0xc1,0xe9,0x0b,0x0f,0xaf,0xc8,0x3b,0xf1,
    0x72,0x0a,0x2b,0xd1,0x2b,0xf1,0x8d,0x7c,0x3f,0x01,0xeb,0x04,0x8b,0xd1,0x03,0xff,
    0x83,0xff,0x40,0x72,0xbe,0x8d,0x47,0xc0,0x83,0xf8,0x04,0x0f,0x82,0xb9,0x00,0x00,
    0x00,0x8b,0xf8,0xd1,0xef,0x8d,0x4f,0xff,0x89,0x4c,0x24,0x1c,0x83,0xf8,0x0e,0x72,
    0x49,0x8d,0x47,0xfb,0x81,0xfa,0x00,0x00,0x00,0x01,0x73,0x17,0x3b,0x6c,0x24,0x2c,
    0x0f,0x83,0x38,0xfb,0xff,0xff,0x0f,0xb6,0x4d,0x00,0xc1,0xe6,0x08,0xc1,0xe2,0x08,
    0x0b,0xf1,0x45,0xd1,0xea,0x8b,0xce,0x2b,0xca,0xc1,0xe9,0x1f,0x49,0x23,0xca,0x2b,
    0xf1,0x83,0xe8,0x01,0x75,0xce,0x8b,0x4c,0x24,0x10,0x81,0xc1,0x44,0x06,0x00,0x00,
    0xc7,0x44,0x24,0x1c,0x04,0x00,0x00,0x00,0xeb,0x17,0x8b,0xf8,0x83,0xe7,0x01,0x83,
    0xcf,0x02,0xd3,0xe7,0x2b,0xf8,0x8b,0x44,0x24,0x10,0x8d,0x8c,0x78,0x5e,0x05,0x00,
    0x00,0xb8,0x01,0x00,0x00,0x00,0x81,0xfa,0x00,0x00,0x00,0x01,0x73,0x17,0x3b,0x6c,
    0x24,0x2c,0x0f,0x83,0xd6,0xfa,0xff,0xff,0x0f,0xb6,0x7d,0x00,0xc1,0xe6,0x08,0xc1,
    0xe2,0x08,0x0b,0xf7,0x45,0x0f,0xb7,0x1c,0x41,0x8b,0xfa,0xc1,0xef,0x0b,0x0f,0xaf,
    0xfb,0x3b,0xf7,0x72,0x0a,0x2b,0xd7,0x2b,0xf7,0x8d,0x44,0x00,0x01,0xeb,0x04,0x8b,
    0xd7,0x03,0xc0,0x83,0x6c,0x24,0x1c,0x01,0x75,0xbc,0x81,0xfa,0x00,0x00,0x00,0x01,
    0x73,0x0a,0x3b,0x6c,0x24,0x2c,0x0f,0x83,0x92,0xfa,0xff,0xff,0x8b,0x44,0x24,0x20,
    0x5f,0x5e,0x5d,0x5b,0x83,0xc4,0x18,0xc3,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,0xcc,
    0x51,0x8b,0x51,0x48,0x53,0x8b,0xd8,0x85,0xd2,0x74,0x67,0x81,0xfa,0x12,0x01,0x00,
    0x00,0x73,0x5f,0x8b,0x41,0x24,0x55,0x8b,0x69,0x14,0x56,0x8b,0x71,0x28,0x2b,0xd8,
    0x57,0x8b,0x79,0x38,0x89,0x74,0x24,0x10,0x8b,0xf2,0x3b,0xda,0x73,0x02,0x8b,0xf3,
    0x83,0x79,0x30,0x00,0x75,0x10,0x8b,0x59,0x0c,0x2b,0x59,0x2c,0x3b,0xde,0x77,0x06,
    0x8b,0x59,0x0c,0x89,0x59,0x30,0x01,0x71,0x2c,0x2b,0xd6,0x89,0x51,0x48,0x85,0xf6,
    0x74,0x1a,0x8b,0x54,0x24,0x10,0x4e,0x3b,0xc7,0x1b,0xdb,0x23,0xda,0x2b,0xdf,0x03,
    0xdd,0x8a,0x1c,0x03,0x88,0x1c,0x28,0x40,0x85,0xf6,0x75,0xea,0x5f,0x5e,0x89,0x41,
    0x24,0x5d,0x5b,0x59,0xc3,/*INT3*/ 0xcc };



char (__cdecl *msf_unpack)(void *out_struct, int outsz, void *in, int *insz) = NULL;



// anti DEP limitation, allocation is necessary
void *msf_unpack_alloc(void *dump, int dumpsz) {
    int     pagesz;
    void    *ret;

    pagesz = (dumpsz + 4095) & (~4095); // useful for pages? mah

#ifdef WIN32
    ret = VirtualAlloc(
        NULL,
        pagesz,
        MEM_COMMIT | MEM_RESERVE,
        PAGE_EXECUTE_READWRITE);    // write for memcpy
#else
    ret = malloc(pagesz);
    mprotect(
        ret,
        pagesz,
        PROT_EXEC | PROT_WRITE);    // write for memcpy
#endif
    memcpy(ret, dump, dumpsz);
    return(ret);
}



void msf_unpack_init(void) {
    if(!msf_unpack) {
        msf_unpack = msf_unpack_alloc(msf_unpack_dump, sizeof(msf_unpack_dump));

        #define PATCHIT(X,Y) *(int *)((char *)msf_unpack + X) = (int)(Y);
        PATCHIT(0x000002c6, &msf_unpack_00405370) // mov dword ptr [405370], ebp
        PATCHIT(0x000002d3, &msf_unpack_00405370) // mov dword ptr [405370], ebp
        PATCHIT(0x000002fc, &msf_unpack_00405378) // mov dword ptr [405378], eax
        PATCHIT(0x00000311, &msf_unpack_0040537C) // mov dword ptr [40537C], ecx
        PATCHIT(0x00000388, &msf_unpack_00405374) // mov dword ptr [405374], ebx
        PATCHIT(0x00001011, msf_unpack_004041ec)  // push 4041EC
        #undef PATCHIT
    }
}



int unmsf(void *in, int insz, void *out, int outsz) {
    int             ret;
    unsigned char   esi[0x1b0],
                    *tmp;

    msf_unpack_init();
    memset(esi, 0, sizeof(esi));
    tmp = calloc(0x3e6c, 1);
    if(!tmp) return(-1);
    *(int   *)(esi)         = 3;
    *(int   *)(esi + 0x04)  = 0;
    *(int   *)(esi + 0x08)  = 2;
    *(int   *)(esi + 0x0c)  = 0x10000;
    *(int   *)(esi + 0x54)  = 0x1f36;
    *(void **)(esi + 0x10)  = tmp;
    *(void **)(esi + 0x14)  = out;
    *(int   *)(esi + 0x28)  = outsz;
    *(int   *)(esi + 0x24)  = 0;
    *(int   *)(esi + 0x4c)  = 1;
    *(int   *)(esi + 0x48)  = 0;
    *(int   *)(esi + 0x58)  = 0;
    *(int   *)(esi + 0x2c)  = 0;
    *(int   *)(esi + 0x30)  = 0;
    *(int   *)(esi + 0x50)  = 1;    
    ret = msf_unpack(esi, outsz, in, &insz);
    free(tmp);
    if(ret) return(-1);
    return(*(int   *)(esi + 0x24));
}
