// dllmain.h : Declaration of module class.

class CMMProfilerModule : public ATL::CAtlDllModuleT< CMMProfilerModule >
{
public :
	DECLARE_LIBID(LIBID_MMProfilerLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_MMPROFILER, "{D904D1BF-A5F2-48C8-A188-60AE3E1C2DE6}")
};

extern class CMMProfilerModule _AtlModule;
