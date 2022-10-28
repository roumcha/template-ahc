using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Globalization.CultureInfo;
using static System.Math;
// using bigint = System.Numerics.BigInteger;

static class Program {
  static void main() {
    Console.WriteLine(0);
  }


#pragma warning disable format
  const bool FLUSH = false; public const bool DEVFLUSH = false;
  static _I cin=new _I(Console.OpenStandardInput());public static void
  Dev()=>main();static void Main(){Console.SetOut(new _O(Console.
  OpenStandardOutput()){AutoFlush=FLUSH});main();Console.Out.Flush();}

  [MethodImpl(256)]static bool ChMax<T>(ref T a,T b)where T:IComparable
  {if(a.CompareTo(b)<0){a=b;return true;}return false;}[MethodImpl(256)
  ]public static bool ChMin<T>(ref T a,T b)where T:IComparable{if(a.
  CompareTo(b)>0){a=b;return true;}return false;}
}

class _I{public _I(Stream s){this.s=s;}Stream s;byte[]u=new byte[1024];
int l,p;bool e=false;public bool End{get{return e;}}[MethodImpl(256)]
byte R(){if(e)throw new EndOfStreamException();if(p>=l){p=0;if((l=s.
Read(u,0,1024))<=0){e=true;return 0;}}return u[p++];}[MethodImpl(256)
]public static implicit operator char(_I c){byte b=0;do b=c.R();while(
b<33||126<b);return(char)b;}[MethodImpl(256)]public static implicit
operator string(_I c){var t=new StringBuilder();for(char b=c;b>=33&&b
<=126;b=(char)c.R())t.Append(b);return t.ToString();}[MethodImpl(256)
]public static implicit operator long(_I c){long r=0;byte b=0;var
n=false;do b=c.R();while(b!='-'&&(b<'0'||'9'<b));if(b=='-'){n=true;b=
c.R();}for(;true;b=c.R()){if(b<'0'||'9'<b)return n?-r:r;else r=r*10+b
-'0';}}[MethodImpl(256)]public static implicit operator int(_I c)=>(
int)(long)c;[MethodImpl(256)]public static implicit operator float(_I
c)=>float.Parse(c,InvariantCulture);[MethodImpl(256)]public static
implicit operator double(_I c)=>double.Parse(c,InvariantCulture);[
MethodImpl(256)]public static implicit operator decimal(_I c)=>decimal
.Parse(c,InvariantCulture);/*[MethodImpl(256)]public static implicit
operator bigint(I c)=>bigint.Parse(c,InvariantCulture);*/}

class _O:StreamWriter{public override IFormatProvider FormatProvider{
get{return InvariantCulture;}}public _O(Stream s):base(s,new
UTF8Encoding(false,true)){}public _O(Stream s,Encoding e):base(s,e){}}
#pragma warning restore format
