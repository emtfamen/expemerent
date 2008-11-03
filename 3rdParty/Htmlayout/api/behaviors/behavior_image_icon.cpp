
#include "behavior_aux.h"

//////////////////////////////////////////////////////////////////////////
//Format in ico file
#pragma pack( push )
#pragma pack( 2 )
typedef struct
{
	BYTE        bwidth;          // width, in pixels, of the image
	BYTE        bheight;         // height, in pixels, of the image
	BYTE        bcolorcount;     // number of colors in image (0 if >=8bpp)
	BYTE        breserved;       // reserved ( must be 0)
	WORD        wplanes;         // color planes
	WORD        wbitcount;       // bits per pixel
	DWORD       dwbytesinres;    // how many bytes in this resource?
	DWORD       dwimageoffset;   // where in the file is this image?
} icondirentry, *lpicondirentry;
#pragma pack( pop )

#pragma pack( push )
#pragma pack( 2 )
typedef struct
{
	WORD           idreserved;   // reserved (must be 0)
	WORD           idtype;       // resource type (1 for icons)
	WORD           idcount;      // how many images?
	icondirentry   identries[1]; // an entry for each image (idcount of 'em)
} icondir, *lpicondir;
#pragma pack( pop )

//////////////////////////////////////////////////////////////////////////
//Format in dll and exe
#pragma pack( push )
#pragma pack( 2 )
typedef struct
{
	BYTE   bwidth;               // width, in pixels, of the image
	BYTE   bheight;              // height, in pixels, of the image
	BYTE   bcolorcount;          // number of colors in image (0 if >=8bpp)
	BYTE   breserved;            // reserved
	WORD   wplanes;              // color planes
	WORD   wbitcount;            // bits per pixel
	DWORD   dwbytesinres;         // how many BYTEs in this resource?
	WORD   nid;                  // the id
} grpicondirentry, *lpgrpicondirentry;
#pragma pack( pop )

// #pragmas are used here to insure that the structure's
// packing in memory matches the packing of the exe or dll.
#pragma pack( push )
#pragma pack( 2 )
typedef struct
{
	WORD            idreserved;   // reserved (must be 0)
	WORD            idtype;       // resource type (1 for icons)
	WORD            idcount;      // how many images?
	grpicondirentry   identries[1]; // the entries for each image
} grpicondir, *lpgrpicondir;
#pragma pack( pop )


namespace htmlayout 
{
struct image_icon: public event_handler
{
    // ctor
    image_icon(): event_handler(HANDLE_DRAW | HANDLE_BEHAVIOR_EVENT | HANDLE_DATA_ARRIVED | HANDLE_INITIALIZATION ), miIconX(0), miIconY(0), mhIcon(0) {}

	~image_icon()
	{
		if (mhIcon)
		{
			DestroyIcon(mhIcon);
		}
	}

	virtual void attached  (HELEMENT he ) 
    { 
		  dom::element el = he;
		  const wchar_t* filename = el.get_attribute("src");
	 	  HLDOM_RESULT hr = HTMLayoutRequestElementData(he, filename, HLRT_DATA_IMAGE, NULL);
    } 
	
    virtual void detached  (HELEMENT he ) 
    { 
		  delete this; // don't need it anymore.
    } 

	virtual BOOL on_data_arrived(HELEMENT he, HELEMENT initiator, LPCBYTE data, UINT dataSize, UINT dataType )
	{
		if (dataSize > 0 && dataType == HLRT_DATA_IMAGE)
		{
			dom::element el = he;
			miIconX = el.get_attribute_int("icon-x",16);
			miIconY = el.get_attribute_int("icon-y",16);
			if (miIconX <= 0 || miIconY <= 0)
				return FALSE;
			
			icondir loIconDirHead;
			memcpy(&loIconDirHead, data, sizeof(icondir));

			lpicondir lpIconDir = (lpicondir)new BYTE[(sizeof(WORD) * 3)  + (sizeof(icondirentry) * loIconDirHead.idcount )];		
			memcpy(lpIconDir->identries, (BYTE*)data + 6, (sizeof(icondirentry) * loIconDirHead.idcount));
			
			lpgrpicondir lpGroupIconDir = (lpgrpicondir)new BYTE[(sizeof(WORD) * 3) + (sizeof(grpicondirentry) * loIconDirHead.idcount) + 1];
			
			lpGroupIconDir->idreserved = loIconDirHead.idreserved;
			lpGroupIconDir->idcount = loIconDirHead.idcount;
			lpGroupIconDir->idtype = loIconDirHead.idtype;
			
			for(int i = 0; i < loIconDirHead.idcount; i++)
			{
				lpGroupIconDir->identries[i].bwidth			= lpIconDir->identries[i].bwidth;
				lpGroupIconDir->identries[i].bheight		= lpIconDir->identries[i].bheight;
				lpGroupIconDir->identries[i].bcolorcount	= lpIconDir->identries[i].bcolorcount;
				lpGroupIconDir->identries[i].breserved		= lpIconDir->identries[i].breserved;
				lpGroupIconDir->identries[i].wplanes		= lpIconDir->identries[i].wplanes;
				lpGroupIconDir->identries[i].wbitcount		= lpIconDir->identries[i].wbitcount;
				lpGroupIconDir->identries[i].dwbytesinres	= lpIconDir->identries[i].dwbytesinres;
				lpGroupIconDir->identries[i].nid			= i + 1;
			}
			int liId = LookupIconIdFromDirectoryEx((BYTE*)data, TRUE, miIconX, miIconY, LR_DEFAULTCOLOR);
			if (liId > 0)
			{
        icondir* t = (icondir*)data;
			  for(int i = 0; i < t->idcount; i++)
			  {
          if( t->identries[i].dwimageoffset == liId  )
          {
				    int liOffset = t->identries[i].dwimageoffset;
				    int liCount = t->identries[i].wbitcount;
				    mhIcon = CreateIconFromResourceEx((BYTE*)data + liOffset, liCount, TRUE, 0x00030000, miIconX, miIconY, LR_DEFAULTCOLOR);
          }
			  }
			}
			
			delete [] (BYTE*)lpIconDir;
			delete  [] (BYTE*)lpGroupIconDir;
			if (mhIcon)
				return TRUE;
		}
		return FALSE;
	}
    
    virtual BOOL on_draw (HELEMENT he, UINT draw_type, HDC hdc, const RECT& rc ) 
    {
	 	if (draw_type == DRAW_FOREGROUND && mhIcon)
		{
			int x = rc.left + (rc.right - rc.left - miIconX) / 2;
			int y = rc.top + (rc.bottom - rc.top - miIconY) / 2;
			
			if (DrawIconEx(hdc, x, y, mhIcon, miIconX, miIconY, 0, NULL, DI_NORMAL))
				return TRUE;
		}
		return FALSE;/*do default draw*/ 
    }

private:
	int miIconX;
	int miIconY;
	HICON mhIcon;
};

image_icon image_icon_in;

struct image_icon_factory :public behavior
{
    image_icon_factory(): behavior(HANDLE_BEHAVIOR_EVENT, "image-icon") {}
	
    // this behavior has unique instance for each element it is attached to
    virtual event_handler* attach (HELEMENT /*he*/ ) 
    { 
		return new image_icon(); 
    }
};
	
// instantiating and attaching it to the global list
image_icon_factory image_icon_factory_inst;


} // htmlayout namespace


