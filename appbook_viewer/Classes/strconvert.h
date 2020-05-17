#ifndef __STR_CONV_H__
#define __STR_CONV_H__
#include <iostream>
#include <string>
using std::string;

#if defined( WIN32 )
#include "win32-specific/icon/include/iconv.h"
#pragma comment(lib, "libiconv.lib")
#else
#include <iconv.h>
#endif

class StrConv {
public:
	static int code_convert(const char * from_charset, const char * to_charset, const char * inbuf, size_t inlen, char * outbuf, size_t outlen) {
		iconv_t cd;
		const char * temp = inbuf;
		const char ** pin = &temp;
		char **pout = &outbuf;
		memset(outbuf, 0, outlen);
		cd = iconv_open(to_charset, from_charset);
		if (cd == 0) return -1;
		if (iconv(cd, pin, &inlen, pout, &outlen) == -1) return -1;
		iconv_close(cd);
		return 0;
	}

	static string u2a(const char * inbuf) {
		size_t inlen = strlen(inbuf);
		char * outbuf = new char[inlen * 2 + 2];
		string strRet;
		if (StrConv::code_convert("utf-8", "euc-kr", inbuf, inlen, outbuf, inlen * 2 + 2) == 0 ) {
			strRet = outbuf;
		}
		delete[] outbuf;
		return strRet;
	}
	/* GB2312 To UTF8 */
	static string a2u(const char * inbuf) {
		size_t inlen = strlen(inbuf);
		char * outbuf = new char[inlen * 2 + 2];
		string strRet;
		if (StrConv::code_convert("euc-kr", "utf-8", inbuf, inlen, outbuf, inlen * 2 + 2) == 0) {
			strRet = outbuf;
		}
		delete[] outbuf;
		return strRet;
	}
};

#define __TA(str) StrConv::u2a(str)
#define __TX(str) StrConv::a2u(str)
#define _TA(str)  StrConv::u2a(str).c_str()
#define _TX(str)  StrConv::a2u(str).c_str()
#endif