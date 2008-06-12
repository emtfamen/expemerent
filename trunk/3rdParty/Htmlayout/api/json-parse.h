//|
//|
//| Copyright (c) 2001-2007
//| Andrew Fedoniouk - andrew@terrainformatica.com
//|
//|


#ifndef __json_parse_h__
#define __json_parse_h__

#include <ctype.h>
#include <assert.h>

#include "json-aux.h"

namespace aux
{

 // simple tokenizer
template <typename T >
    class tokens
    {
      const T* delimeters;
      const T* p;
      const T* tail;
      const T* start;
      const T* end;
      const bool  is_delimeter(T el)  { for(const T* t = delimeters;t && *t; ++t) if(el == *t) return true;  return false; }
      const T*    tok()               { for(;p < tail; ++p) if(is_delimeter(*p)) return p++; return p; }
    public:

      tokens(const T *text, size_t text_length, const T* separators): delimeters(separators)
      {
        start = p = text;
        tail = p + text_length;
        end = tok();
      }

      tokens(const aux::slice<T> s, const T* separators): delimeters(separators)
      {
        start = p = s.start;
        tail = p + s.length;
        end = tok();
      }

      bool next(slice<T>& v)
      {
        if(start < tail)
        {
          v.start = start;
          v.length = uint(end - start);
          start = p;
          end = tok();
          return true;
        }
        return false;
      }
    };


typedef tokens<char> atokens;
typedef tokens<wchar_t> wtokens;

inline chars chars_of(const char* str)
{
   if( str ) return chars( str, uint(strlen(str)) );
   return chars();
}

inline wchars chars_of(const wchar_t* str)
{
   if( str ) return wchars( str, uint(wcslen(str)) );
   return wchars();
}

template <typename CS>
  inline slice<CS> trim(slice<CS> str)
{
  for( unsigned i = 0; i < str.length; ++i )
    if( isspace(str[0]) ) { ++str.start; --str.length; }
    else break;
  for( unsigned j = str.length - 1; j >= 0; --j )
    if( isspace(str[j]) ) --str.length;
    else break;
  return str;
}


//int match ( chars cr, const char *pattern );
//int match ( wchars cr, const wchar *pattern );

  /****************************************************************************/
  //
  // idea was taken from Konstantin Knizhnik's FastDB
  // see http://www.garret.ru/
  // extended by [] operations
  //

template <typename CT, CT sep = '-', CT end = ']' >
  struct charset
  {
    #define SET_SIZE (1 << (sizeof(CT) * 8))
    
    unsigned char codes[ SET_SIZE >> 3 ];
    
    void set ( unsigned from, unsigned to, bool v )    
    {
       for ( unsigned i = from; i <= to; ++i )
       {
         unsigned int bit = i & 7;
         unsigned int octet = i >> 3;
         if( v ) codes[octet] |= 1 << bit; else codes[octet] &= ~(1 << bit);
       }
    } 
    void init ( unsigned char v )  { memset(codes,v,(SET_SIZE >> 3)); }

    void parse ( const CT* &pp )
    {
      //assert( sizeof(codes) == sizeof(CT) * sizeof(bool));
      const CT *p = (const CT *) pp;
      unsigned char inv = *p == '^'? 0xff:0;
      if ( inv ) { ++p; }
      init ( inv );
      if ( *p == sep ) set(unsigned(sep),unsigned(sep),inv == 0);
      while ( *p )
      {
        if ( p[0] == end ) { p++; break; }
        if ( p[1] == sep && p[2] != 0 ) { set ( unsigned(p[0]), unsigned(p[2]), inv == 0 );  p += 3; }
        else { unsigned t = *p++; set(t,t, inv == 0); }
      }
      pp = (const CT *) p;
    }

    bool valid ( CT c )
    {
      unsigned int bit = unsigned(c) & 7;
      unsigned int octet = unsigned(c) >> 3;
      return (codes[octet] & (1 << bit)) != 0;
    } 
    #undef SET_SIZE
  };

template <typename CT >
  inline int match ( slice<CT> cr, const CT *pattern )
  {
    const CT AnySubstring = '*';
    const CT AnyOneChar = '?';
    const CT AnyOneDigit = '#';

    const CT    *str = cr.start;
    const CT    *wildcard = 0;
    const CT    *strpos = 0;
    const CT    *matchpos = 0;

    charset<CT> cset;

    while ( true )
    {
      if ( *pattern == AnySubstring )
      {
        wildcard = ++pattern;
        strpos = str;
        if ( !matchpos ) matchpos = str;
      }
      else if ( *str == '\0' || str >= cr.end() )
      {
        return ( *pattern == '\0' ) ? int( matchpos - cr.start ) : -1;
      }
      else if ( *pattern == '[' )
      {
        pattern++;
        cset.parse ( pattern );
        if ( !cset.valid ( *str ) )
          return -1;
        if ( !matchpos )
          matchpos = str;
        str += 1;
      }
      else if ( *str == *pattern || *pattern == AnyOneChar )
      {
        if ( !matchpos ) matchpos = str;
        str += 1;
        pattern += 1;
      }
      else if ( *str == *pattern || *pattern == AnyOneDigit )
      {
        if ( !isdigit(*str )) return -1;
        if ( !matchpos ) matchpos = str;
        str += 1;
        pattern += 1;
      }
      else if ( wildcard )
      {
        str = ++strpos;
        pattern = wildcard;
      }
      else
        break;
    }
    return -1;
  }

template <typename T >
  inline bool slice<T>::like ( const T *pattern ) const
  {
    return match<T>(*this,pattern) >= 0;
  }

// chars to uint
// chars to int

template <typename T>
    inline unsigned int to_uint(slice<T>& span, unsigned int base = 10)
{
   unsigned int result = 0,value;
   const T *cp = span.start;
   const T *pend = span.end();

   while ( cp < pend && isspace(*cp) ) ++cp;

   if (!base)
   {
       base = 10;
       if (*cp == '0') {
           base = 8;
           cp++;
           if ((toupper(*cp) == 'X') && isxdigit(cp[1])) {
                   cp++;
                   base = 16;
           }
       }
   }
   else if (base == 16)
   {
       if (cp[0] == '0' && toupper(cp[1]) == 'X')
           cp += 2;
   }
   while ( cp < pend && isxdigit(*cp) &&
          (value = isdigit(*cp) ? *cp-'0' : toupper(*cp)-'A'+10) < base) {
           result = result*base + value;
           cp++;
   }
   span.length = uint(cp - span.start);
   return result;
}

template <typename T>
    int to_int(slice<T>& span, unsigned int base = 10)
{

   while (span.length > 0 && isspace(span[0]) ) { ++span.start; --span.length; }
   if(span[0] == '-')
   {
      ++span.start; --span.length;
      return - int(to_uint(span,base));
   }
   return to_uint(span,base);
}


}

#endif
