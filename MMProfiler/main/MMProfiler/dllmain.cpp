// dllmain.cpp : Implementation of DllMain.

#include "stdafx.h"
#include "resource.h"
#include "MMProfiler_i.h"

#include "dllmain.h"
#include "xdlldata.h"

#include "FunctionProfiler.h"

CMMProfilerModule _AtlModule;

// DLL Entry Point
extern "C" BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
#ifdef _MERGE_PROXYSTUB
	if (!PrxDllMain(hInstance, dwReason, lpReserved))
		return FALSE;
#endif
	hInstance;

    if (dwReason == DLL_PROCESS_ATTACH)
    {
        DisableThreadLibraryCalls(hInstance);
    }
    else if ((lpReserved != NULL) && (dwReason == DLL_PROCESS_DETACH))
    {
        ATLTRACE(_T("DLL_PROCESS_DETACH"));
        if (CFunctionProfiler::g_pProfiler!=NULL)
        {
            ATLTRACE(_T("DLL_PROCESS_DETACH(NOTNULL)"));
            CFunctionProfiler::g_pProfiler->SharedShutdown();
        }
    }

	return _AtlModule.DllMain(dwReason, lpReserved); 
}
