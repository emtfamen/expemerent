#ifndef __SCITER_X__
#define __SCITER_X__

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <oaidl.h>


/** \mainpage Terra Informatica Sciter engine.
 *
 * \section legal_sec In legalese
 *
 * The code and information provided "as-is" without
 * warranty of any kind, either expressed or implied.
 *
 * <a href="http://terrainformatica.com/Sciter">Sciter Home</a>
 *
 * (C) 2003-2006, Terra Informatica Software, Inc. and Andrew Fedoniouk
 *
 * \section structure_sec Structure of the documentation
 *
 * See <a href="files.html">Files</a> section.
 **/

/**\file sciter-x.h
 * Main include file.
 **/

/**DOM element handle.*/
typedef void*  HELEMENT;
/**DOM range handle.*/
typedef void*  HRANGE;
typedef struct hposition { HELEMENT he; INT pos; } HPOSITION;

#ifndef LPUINT
  typedef UINT FAR *LPUINT;
#endif

#ifndef LPCBYTE
  typedef const BYTE* LPCBYTE;
#endif


#define STDCALL __stdcall

#ifdef __cplusplus
#define EXTERN_C extern "C"
#else 
#define EXTERN_C extern
#endif /* __cplusplus **/

#ifndef SCITER_STATIC_LIB
  #ifdef  XSCITER
    #define SCAPI __declspec(dllexport) __stdcall
  #else
    #define SCAPI __declspec(dllimport) __stdcall
  #endif
#else
  #define SCAPI __stdcall
  void SCAPI SciterInit( HINSTANCE hModule, bool start);
#endif 

#define WM_REDRAW (WM_USER + 1)

#pragma pack(push,4)

/** Resource data type.
 *  Used by SciterDataReadyAsync() function.
 **/
enum SciterResourceType 
{ 
  RT_DATA_HTML = 0, 
  RT_DATA_IMAGE = 1, 
  RT_DATA_STYLE = 2, 
  RT_DATA_CURSOR = 3,
  RT_DATA_SCRIPT = 4,
};

#include "sciter-x-dom.h"

/**Get name of Sciter window class.
 *
 * \return \b LPCSTR, name of Sciter window class.
 *
 * Use this function if you wish to create ansi version of Sciter.
 * The returned name can be used in CreateWindow(Ex)A function.
 * You can use #SciterClassNameT macro.
 ***/
EXTERN_C LPCSTR  SCAPI SciterClassNameA();

/**Get name of Sciter window class.
 *
 * \return \b LPCWSTR, name of Sciter window class.
 *
 * Use this function if you wish to create unicode version of Sciter. 
 * The returned name can be used in CreateWindow(Ex)W function. 
 * You can use #SciterClassNameT macro.
 **/
EXTERN_C LPCWSTR SCAPI SciterClassNameW();

/**Returns name of Sciter window class.
 *
 * \return \b LPCTSTR, name of Sciter window class.
 *
 * This macro is used to select between #SciterClassNameW() and 
 * #SciterClassNameA() functions depending on whether UNICODE macro symbol 
 * is defined. 
 **/
#ifdef UNICODE
#define SciterClassNameT  SciterClassNameW
#else
#define SciterClassNameT  SciterClassNameA
#endif // !UNICODE

enum 
{ 
  LOAD_OK = 0,      // do default loading if data not set
  LOAD_DISCARD = 1, // discard request completely
  LOAD_DELAYED = 2, // data will be delivered later by the host
};

/**Notifies that HtmLayout is about to download a referred resource. 
 *
 * \param lParam #LPSCN_LOAD_DATA.
 * \return #LOAD_OK or #LOAD_DISCARD 
 *
 * This notification gives application a chance to override built-in loader and 
 * implement loading of resources in its own way (for example images may be loaded from 
 * database or other resource). To do this set #SCN_LOAD_DATA::outData and 
 * #SCN_LOAD_DATA::outDataSize members of SCN_LOAD_DATA. Sciter does not 
 * store pointer to this data. You can call #SciterDataReady() function instead 
 * of filling these fields. This allows you to free your outData buffer 
 * immediately.
**/
#define SC_LOAD_DATA       0x01

/**This notification indicates that external data (for example image) download process 
 * completed.
 *
 * \param lParam #LPSCN_DATA_LOADED
 *
 * This notifiaction is sent for each external resource used by document when 
 * this resource has been completely downloaded. Sciter will send this 
 * notification asynchronously.
 **/
#define SC_DATA_LOADED     0x02 

/**This notification is sent when all external data (for example image) has been downloaded.
 *
 * This notification is sent when all external resources required by document 
 * have been completely downloaded. Sciter will send this notification 
 * asynchronously.
 **/
#define SC_DOCUMENT_COMPLETE 0x03 


/**This notification is sent on parsing the document and while processing 
 * elements having non empty style.behavior attribute value.
 *
 * \param lParam #LPSCN_ATTACH_BEHAVIOR
 * 
 * Application has to provide implementation of #sciter::behavior interface. 
 * Set #SCN_ATTACH_BEHAVIOR::impl to address of this implementation.
 **/
#define SC_ATTACH_BEHAVIOR 0x04


/**This notification is sent on  
 * 1) stdin, stdout and stderr operations and
 * 2) view.hostCallback(p1,p2) calls from script
 *
 * \param lParam #LPSCN_CALLBACK_HOST
 * 
 **/
#define SC_CALLBACK_HOST 0x05



/**Notification callback structure.
 **/
struct SCITER_CALLBACK_NOTIFICATION
{
  UINT code; /**< [in] one of the codes above.*/
  HWND hwnd; /**< [in] HWND of the window this callback was attached to.*/
};
typedef SCITER_CALLBACK_NOTIFICATION * LPSCITER_CALLBACK_NOTIFICATION; 

typedef UINT CALLBACK SciterHostCallback( LPSCITER_CALLBACK_NOTIFICATION pns, LPVOID callbackParam );

typedef SciterHostCallback * LPSciterHostCallback;


/**This structure is used by #SCN_LOAD_DATA notification.
 *\copydoc SCN_LOAD_DATA 
 **/

struct SCN_LOAD_DATA: SCITER_CALLBACK_NOTIFICATION
{
    LPCWSTR  uri;              /**< [in] Zero terminated string, fully qualified uri, for example "http://server/folder/file.ext".*/
    
    LPCBYTE  outData;          /**< [in,out] pointer to loaded data to return. if data exists in the cache then this field contain pointer to it*/
    UINT     outDataSize;      /**< [in,out] loaded data size to return.*/
    UINT     dataType;         /**< [in] SciterResourceType */

    LPVOID   request_id;
    
    HELEMENT principal;
    HELEMENT initiator;
    

};
typedef SCN_LOAD_DATA FAR * LPSCN_LOAD_DATA;

/**This structure is used by #SCN_DATA_LOADED notification.
 *\copydoc SCN_DATA_LOADED 
 **/
struct SCN_DATA_LOADED: SCITER_CALLBACK_NOTIFICATION
{
    LPCWSTR  uri;              /**< [in] zero terminated string, fully qualified uri, for example "http://server/folder/file.ext".*/
    LPCBYTE  data;             /**< [in] pointer to loaded data.*/
    DWORD    dataSize;         /**< [in] loaded data size (in bytes).*/
    UINT     dataType;         /**< [in] SciterResourceType */

}; 
typedef SCN_DATA_LOADED FAR * LPSCN_DATA_LOADED;

/**This structure is used by #SCN_ATTACH_BEHAVIOR notification.
 *\copydoc SCN_ATTACH_BEHAVIOR **/
struct SCN_ATTACH_BEHAVIOR: SCITER_CALLBACK_NOTIFICATION
{
    HELEMENT element;          /**< [in] target DOM element handle*/
    LPCSTR   behaviorName;     /**< [in] zero terminated string, string appears as value of CSS behavior:"???" attribute.*/
    
    ElementEventProc* elementProc;    /**< [out] pointer to ElementEventProc function.*/
    LPVOID            elementTag;     /**< [out] tag value, passed as is into pointer ElementEventProc function.*/

};
typedef SCN_ATTACH_BEHAVIOR FAR * LPSCN_ATTACH_BEHAVIOR;


/**This structure is used by #SC_CALLBACK_HOST notification.
 *\copydoc SC_CALLBACK_HOST **/
struct SCN_CALLBACK_HOST: SCITER_CALLBACK_NOTIFICATION
{
   UINT channel; // 0 - stdin, 1 - stdout, 2 - stderr
   sciter::value_t p1; // in, parameter #1
   sciter::value_t p2; // in, parameter #2
   sciter::value_t r;  // out, retval
};
typedef SCN_CALLBACK_HOST FAR * LPSCN_CALLBACK_HOST;

#include "sciter-x-behavior.h"

/**This function is used in response to SCN_LOAD_DATA request. 
 *
 * \param[in] hwnd \b HWND, Sciter window handle.
 * \param[in] uri \b LPCWSTR, URI of the data requested by Sciter.
 * \param[in] data \b LPBYTE, pointer to data buffer.
 * \param[in] dataLength \b DWORD, length of the data in bytes.
 * \return \b BOOL, TRUE if Sciter accepts the data or \c FALSE if error occured 
 * (for example this function was called outside of #SCN_LOAD_DATA request).
 *
 * \warning If used, call of this function MUST be done ONLY while handling 
 * SCN_LOAD_DATA request and in the same thread. For asynchronous resource loading
 * use SciterDataReadyAsync
 **/
EXTERN_C BOOL SCAPI SciterDataReady(HWND hwnd,LPCWSTR uri,LPCBYTE data, DWORD dataLength);

/**Use this function outside of SCN_LOAD_DATA request. This function is needed when you
 * you have your own http client implemented in your application.
 *
 * \param[in] hwnd \b HWND, Sciter window handle.
 * \param[in] uri \b LPCWSTR, URI of the data requested by Sciter.
 * \param[in] data \b LPBYTE, pointer to data buffer.
 * \param[in] dataLength \b DWORD, length of the data in bytes.
 * \param[in] dataType \b UINT, type of resource to load. See SciterResourceType.
 * \return \b BOOL, TRUE if Sciter accepts the data or \c FALSE if error occured 
 **/

EXTERN_C BOOL SCAPI SciterDataReadyAsync(HWND hwnd,LPCWSTR uri, LPCBYTE data, DWORD dataLength, 
                                         LPVOID requestId);


/**Sciter Window Proc.*/
EXTERN_C LRESULT SCAPI SciterProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

/**Sciter Window Proc without call of DefWindowProc.*/
EXTERN_C LRESULT SCAPI SciterProcND(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam, BOOL* pbHandled);


/**Load HTML file.
 *
 * \param[in] hWndSciter \b HWND, Sciter window handle. 
 * \param[in] filename \b LPCWSTR, File name of an HTML file.
 * \return \b BOOL, \c TRUE if the text was parsed and loaded successfully, \c FALSE otherwise.
 **/
EXTERN_C BOOL SCAPI     SciterLoadFile(HWND hWndSciter, LPCWSTR filename);

/**Load HTML from in memory buffer with base.
 *
 * \param[in] hWndSciter \b HWND, Sciter window handle.
 * \param[in] html \b LPCBYTE, Address of HTML to load.
 * \param[in] htmlSize \b UINT, Length of the array pointed by html parameter.
 * \param[in] baseUrl \b LPCWSTR, base URL. All relative links will be resolved against 
 *                                this URL.
 * \return \b BOOL, \c TRUE if the text was parsed and loaded successfully, FALSE otherwise.
 **/
EXTERN_C BOOL SCAPI     SciterLoadHtml(HWND hWndSciter, LPCBYTE html, UINT htmlSize, LPCWSTR baseUrl);

/**Set \link #SCITER_NOTIFY() notification callback function \endlink.
 *
 * \param[in] hWndSciter \b HWND, Sciter window handle.
 * \param[in] cb \b SCITER_NOTIFY*, \link #SCITER_NOTIFY() callback function \endlink.
 * \param[in] cbParam \b LPVOID, parameter that will be passed to \link #SCITER_NOTIFY() callback function \endlink as vParam paramter.
 **/
EXTERN_C VOID SCAPI     SciterSetCallback(HWND hWndSciter, LPSciterHostCallback cb, LPVOID cbParam);

/**Set Master style sheet.
  See: http://www.terrainformatica.com/sciter/master_ss.css.txt
  Or resource section of the library for "master-css" HTML resource.
 *
 * \param[in] utf8 \b LPCBYTE, start of CSS buffer.
 * \param[in] numBytes \b UINT, number of bytes in utf8.
 **/

EXTERN_C BOOL SCAPI     SciterSetMasterCSS(LPCBYTE utf8, UINT numBytes);

/**Set (reset) style sheet of current document.
 Will reset styles for all elements according to given CSS (utf8)
 *
 * \param[in] hWndSciter \b HWND, Sciter window handle.
 * \param[in] utf8 \b LPCBYTE, start of CSS buffer.
 * \param[in] numBytes \b UINT, number of bytes in utf8.
 **/

EXTERN_C BOOL SCAPI     SciterSetCSS(HWND hWndSciter, LPCBYTE utf8, UINT numBytes, LPCWSTR baseUrl, LPCWSTR mediaType);

/**Set media type of this sciter instance.
 *
 * \param[in] hWndSciter \b HWND, Sciter window handle.
 * \param[in] mediaType \b LPCWSTR, media type name.
 *
 * For example media type can be "handheld", "projection", "screen", "screen-hires", etc.
 * By default sciter window has "screen" media type.
 * 
 * Media type name is used while loading and parsing style sheets in the engine so
 * you should call this function *before* loading document in it.
 *
 **/

EXTERN_C BOOL SCAPI     SciterSetMediaType(HWND hWndSciter, LPCWSTR mediaType);

EXTERN_C UINT SCAPI     SciterGetMinWidth(HWND hWndSciter);
EXTERN_C UINT SCAPI     SciterGetMinHeight(HWND hWndSciter, UINT width);

EXTERN_C BOOL SCAPI     SciterCall(HWND hWnd, LPCSTR functionName, UINT argc, const SCITER_VALUE* argv, SCITER_VALUE* retval);

/** Set sciter home url.
 *  home url is used for resolving sciter: urls 
 *  If you will set it like SciterSetHomeURL(hwnd,"http://sciter.com/modules/")
 *  then <script src="sciter:lib/root-extender.tis"> will load 
 *  root-extender.tis from http://sciter.com/modules/lib/root-extender.tis
 *
 * \param[in] hWndSciter \b HWND, Sciter window handle.
 * \param[in] baseUrl \b LPCWSTR, URL of sciter home.
 *
 **/

EXTERN_C BOOL SCAPI     SciterSetHomeURL(HWND hWndSciter, LPCWSTR baseUrl);

/** SciterSetupDebugOutput - setup debug output function.
 *
 *  This output function will be used for reprting problems 
 *  found while loading html and css documents.
 *
 **/

typedef VOID (CALLBACK* DEBUG_OUTPUT_PROC)(LPVOID param, INT character);

EXTERN_C VOID SCAPI SciterSetupDebugOutput(
                LPVOID                param,    // param to be passed "as is" to the pfOutput
                DEBUG_OUTPUT_PROC     pfOutput  // output function, output stream alike thing.
                );


#pragma pack(pop)


#if !defined(SCITER_EXTENDER_MODULE)

#if defined(__cplusplus) && !defined( PLAIN_API_ONLY )

  namespace sciter 
  {
    struct debug_output
    {
      debug_output()
      {
        ::SciterSetupDebugOutput(0, _output_debug);
      }
      static VOID CALLBACK _output_debug(LPVOID, INT ch)
      {
        WCHAR m[2] = {0,0};
        m[0] = ch;
        OutputDebugStringW(m);
      }
    };

#if !defined(_WIN32_WCE)
    struct debug_output_console
    {
      
      debug_output_console()
      {
        ::SciterSetupDebugOutput(0, _output_debug);
      }
      static VOID CALLBACK _output_debug(LPVOID, INT ch)
      {
        static bool initialized = false;
        if(!initialized)
        {
          AllocConsole();
          freopen("conin$", "r", stdin);
          freopen("conout$", "w", stdout);
          freopen("conout$", "w", stderr);
          initialized = true;
        }
        putwchar(ch);
      }

      void printf( const char* fmt, ... )
      {
        char buffer [ 2049 ];
        va_list args;
        va_start ( args, fmt );
        int len = _vsnprintf( buffer, 2048, fmt, args );
        va_end ( args );
        buffer [ 2048 ] = 0;
        for( const char *p = buffer; *p; ++p)
          _output_debug(0, *p);
      }
    };
#endif

    // standard implementation of SCN_ATTACH_BEHAVIOR notification
    inline bool create_behavior( LPSCN_ATTACH_BEHAVIOR lpab )
    {
      event_handler *pb = behavior_factory::create(lpab->behaviorName, lpab->element);
      if(pb) 
      {
        lpab->elementTag  = pb;
        lpab->elementProc = event_handler::element_proc;
        return true;
      }
      return false;
    }
  }

#endif

// link against xsciter library
#if !defined(SCITER_STATIC_LIB) && !defined(XSCITER)
#ifdef UNDER_CE
#pragma comment(lib, "mosciter-x.lib")
#else
#pragma comment(lib, "sciter-x.lib")
#endif
#endif

#endif // defined(SCITER_EXTENDER_MODULE)

#endif

