/*
 * Terra Informatica Sciter Engine
 * http://terrainformatica.com/sciter
 * 
 * Sciter value class. 
 * 
 * The code and information provided "as-is" without
 * warranty of any kind, either expressed or implied.
 * 
 * (C) 2003-2004, Andrew Fedoniouk (andrew@terrainformatica.com)
 */

/**\file
 * \brief value, aka variant, aka discriminated union
 **/

#ifndef __sciter_x_value_h__
#define __sciter_x_value_h__

#pragma once

#include <string>

#include "sciter-x-aux.h"
#include "json-value.h"

namespace sciter
{
  typedef json::value value_t;
  typedef json::named_value named_value_t;
}

typedef sciter::value_t SCITER_VALUE;
 

#endif
