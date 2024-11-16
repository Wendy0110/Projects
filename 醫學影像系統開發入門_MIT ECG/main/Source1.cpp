#include <stdio.h>
#include <stdlib.h>

bool dataToJSArray(const char* filename, unsigned int readOffsetByte)
{                                        //�q�ɮת����Ӧ�m�}�lŪ���ƾ�
    FILE* pFile; //�nŪ������
    FILE* pFileOut; //�n�g�J����
    //size_t result;

    pFileOut = fopen("ECGdat2020.js", "w");
    pFile = fopen(filename, "rb");

    if (pFile == NULL) 
    { 
        fputs("File error", stderr); 
        exit(1); 
    }

    fseek(pFile, readOffsetByte, SEEK_SET); //�N�����w������w��Ū��������m(readOffsetByte)

    char buffer[24000];
    int readBytes; //�ݭnŪ�����r�`��

    readBytes = 360 * 3 * 5;  //sample rate 360, 1.5 * 2 leads, 5 seconds
    fread(buffer, 1, readBytes, pFile); //�q�G�i���ɮפ�Ū�����w�r�`��(readBytes)���ƾڡA�ñN���G�s�bbuffer��
    buffer[readBytes] = 0; //�̫�@�Ӧr�`�]�m���s�A�H�T�OJavaScript�Ʋժ�����
   
    int i;
    unsigned char b1, b2, b3; //byte0 byte1 byte2
    int v; //chanel

    fprintf(pFileOut, "var ECGdat =[ \n");

    for (i = 0; i < readBytes; i = i + 3) //212�榡
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
