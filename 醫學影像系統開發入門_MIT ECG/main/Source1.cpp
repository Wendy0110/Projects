#include <stdio.h>
#include <stdlib.h>

bool dataToJSArray(const char* filename, unsigned int readOffsetByte)
{                                        //從檔案的哪個位置開始讀取數據
    FILE* pFile; //要讀取的檔
    FILE* pFileOut; //要寫入的檔
    //size_t result;

    pFileOut = fopen("ECGdat2020.js", "w");
    pFile = fopen(filename, "rb");

    if (pFile == NULL) 
    { 
        fputs("File error", stderr); 
        exit(1); 
    }

    fseek(pFile, readOffsetByte, SEEK_SET); //將文件指針移到指定的讀取偏移位置(readOffsetByte)

    char buffer[24000];
    int readBytes; //需要讀取的字節數

    readBytes = 360 * 3 * 5;  //sample rate 360, 1.5 * 2 leads, 5 seconds
    fread(buffer, 1, readBytes, pFile); //從二進制檔案中讀取指定字節數(readBytes)的數據，並將結果存在buffer中
    buffer[readBytes] = 0; //最後一個字節設置為零，以確保JavaScript數組的結尾
   
    int i;
    unsigned char b1, b2, b3; //byte0 byte1 byte2
    int v; //chanel

    fprintf(pFileOut, "var ECGdat =[ \n");

    for (i = 0; i < readBytes; i = i + 3) //212格式
    {
        b1 = buffer[i];
        b2 = buffer[i + 1];
        b3 = buffer[i + 2];
        v = (b2 / 16) * 256 + b1; //chanel 1 1byte=8bits
        printf("%u, ", v);
        fprintf(pFileOut, "[ %u,", v);
        v = (b2 % 16) * 256 + b3; //chanel 2
        printf("%u \n ", v);
        fprintf(pFileOut, "%u ]", v);
        if (i + 3 < readBytes) fprintf(pFileOut, ",\n");
    }
    fprintf(pFileOut, "];");

    fclose(pFile);
    fclose(pFileOut);
}

int main(int argc, char* argv[])
{
    dataToJSArray("100.dat", 0);
    return 0;
}
