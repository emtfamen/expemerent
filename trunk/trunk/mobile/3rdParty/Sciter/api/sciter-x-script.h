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

/**\file
 * \brief Application defined scripting classes.
 **/

#ifndef __sciter_x_scripting_h__
#define __sciter_x_scripting_h__

#include "sciter-x.h"
#include "sciter-x-value.h"

typedef void* HVM;

// Returns scripting VM assosiated with Sciter HWND.
EXTERN_C HVM SCAPI SciterGetVM( HWND hwnd );

//
// typedef of C/C++ "native" functions to be called from script.
// Shall return TRUE if retval contains valid value, return FALSE toerror.
//
typedef VOID CALLBACK SciterNativeMethod_t( HVM hvm, LPVOID*  p_data_slot, SCITER_VALUE* argv, INT argc, /*out*/ SCITER_VALUE* retval );
typedef VOID CALLBACK SciterNativeProperty_t( HVM hvm, LPVOID*  p_data_slot, BOOL set, /*in-set/out-get*/ SCITER_VALUE* val );
typedef VOID CALLBACK SciterNativeDtor_t( HVM hvm, LPVOID* p_data_slot_value );

struct SciterNativeMethodDef
{
  LPCSTR                  name;
  SciterNativeMethod_t*   method;
};

struct SciterNativePropertyDef
{
  LPCSTR                  name;
  SciterNativeProperty_t* property;
};

struct SciterNativeClassDef
{
  const char*               name;
  SciterNativeMethodDef*    methods;
  SciterNativePropertyDef*  properties;
  SciterNativeDtor_t*       dtor;
};



//
// SciterNativeDefineClass - register "native" class to for the script.
// 
// params: 
//   p_class_def - pointer to class defintion (above).
// returns:
//   TRUE - if class was added successfully to script namespace, FALSE otherwise.
//

EXTERN_C BOOL SCAPI SciterNativeDefineClass( HVM hvm, SciterNativeClassDef* pClassDef);

//
// TIS_throw - throw error from method or property implementation code.
// 
// params: 
//   error, LPCWSTR - error message.
//

EXTERN_C VOID SCAPI SciterNativeThrow( HVM hvm, LPCWSTR errorMsg);

#endif
