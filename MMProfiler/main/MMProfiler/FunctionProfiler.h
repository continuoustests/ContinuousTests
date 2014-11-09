// FunctionProfiler.h : Declaration of the CFunctionProfiler

#pragma once
#include "resource.h"       // main symbols

#include "MMProfiler_i.h"

#include "Timer.h"
#include "Synchronization.h"

using namespace ATL;

#define COM_FAIL_RETURN(hr, ret) if (!SUCCEEDED(hr)) return (ret)
#ifdef DEBUG
#define COM_FAIL(hr) { HRESULT res = hr; if (!SUCCEEDED(res)) { USES_CONVERSION; ATLTRACE(_T("hr = %X (%s, %d)"), res, A2T(__FILE__), __LINE__); return; } }
#define COM_FAIL1(hr, val) { HRESULT res = hr; if (!SUCCEEDED(res)) { USES_CONVERSION; ATLTRACE(_T("hr = %X, val = %X (%s, %d)"), res, val, A2T(__FILE__), __LINE__); return; } }
#else
#define COM_FAIL(hr) if (!SUCCEEDED(hr)) return
#define COM_FAIL1(hr, val) if (!SUCCEEDED(hr)) return
#endif

#define COM_FAIL_MSG(hr, msg) if (!SUCCEEDED(hr)) { ATLTRACE(_T(msg)); return;}

struct FunctionInfo
{
public:
    FunctionInfo() : trace(true), assigned(false)
    {
    }
public:
    std::wstring metadataSignature;
    //std::wstring runtimeSignature;
    bool assigned;
    bool trace;
};

// CFunctionProfiler

class ATL_NO_VTABLE CFunctionProfiler :
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CFunctionProfiler, &CLSID_FunctionProfiler>,
	public ICorProfilerCallback3
{
public:
	CFunctionProfiler()
	{
        m_sequence = 0;
        m_AssignedFunctionId = 0;
        m_nShutdownCount = 0;
	}

DECLARE_REGISTRY_RESOURCEID(IDR_FUNCTIONPROFILER)

BEGIN_COM_MAP(CFunctionProfiler)
    COM_INTERFACE_ENTRY(ICorProfilerCallback)
    COM_INTERFACE_ENTRY(ICorProfilerCallback2)
    COM_INTERFACE_ENTRY(ICorProfilerCallback3)
END_COM_MAP()

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
        if (m_profilerInfo2!=NULL) m_profilerInfo2.Release();
        if (m_profilerInfo3!=NULL) m_profilerInfo3.Release();
	}

private:
    static UINT_PTR _stdcall FunctionMapper2(FunctionID functionId, void* clientData, BOOL* pbHookFunction);
    static UINT_PTR _stdcall FunctionMapper(FunctionID functionId, BOOL* pbHookFunction);

public:
    static CFunctionProfiler* g_pProfiler;
    void SharedShutdown();

private:
    CComQIPtr<ICorProfilerInfo2> m_profilerInfo2;
    CComQIPtr<ICorProfilerInfo3> m_profilerInfo3;
    CComAutoCriticalSection m_cs;
    std::vector<FunctionInfo> m_functions;
    int m_AssignedFunctionId;

    std::list<tstring> m_excludes;
    std::list<tstring> m_includes;

private:
    AssemblyID GetModuleName(ModuleID moduleId, std::wstring &moduleName);
    void GetAssemblyName(AssemblyID assemblyId, std::wstring &assemblyName);
    void GetClassIDForFunctionID(FunctionID funcId, COR_PRF_FRAME_INFO func, ClassID &funcClass, mdToken &funcToken, IMetaDataImport2** ppMeta);

    void AddRuntimeGenericParts(ULONG32 numArgs, ClassID * classArgs, std::wstring& name);
    void GetRuntimeClassSignature(ClassID classId, std::wstring& className);
    void GetRuntimeMethodSignature(FunctionID funcID, COR_PRF_FRAME_INFO func, IMetaDataImport2* metaDataImport2, std::wstring& methodName);
    void GetRuntimeFullMethodSignature(FunctionID funcID, COR_PRF_FRAME_INFO func, std::wstring& methodName);

    void GetMetadataGenericSignature(mdToken token, IMetaDataImport2* metaDataImport2, std::wstring &name);
    void GetMetadataClassName(ModuleID moduleId, mdTypeDef tokenTypeDef, std::wstring &className);
    void GetMetadataClassName(mdTypeDef tokenTypeDef, IMetaDataImport2* metaDataImport2, std::wstring &className);
    void GetMetadataMethodName(IMetaDataImport2* metaDataImport2, mdMethodDef tokenMethodDef, std::wstring &methodName);
    void GetMetadataClassName(ClassID classId, std::wstring &className);
    void GetMetadataFullMethodName(FunctionID funcId, COR_PRF_FRAME_INFO func, std::wstring &methodName);
    void GetMetadataFullMethodNameWithSignature(FunctionID funcId, COR_PRF_FRAME_INFO func, std::wstring &fullMethodName);
    void GetMetadataMethodSignatureTypes(IMetaDataImport2* metaDataImport2, mdMethodDef tokenMethodDef, std::wstring &returnType, std::wstring &parameters);
    
    bool Include(const std::wstring& methodName, bool* recordMeta);
    
private:

private:
    int m_nShutdownCount;

private: // map
    HANDLE m_hMapFile;
    BYTE* m_pView;
    CEvent m_hReadBufferEvent;
    CEvent m_hBufferReadEvent;

    CMutex m_hProcessMutex;

    void WriteData();
    
    int m_bufferSize;
    int m_thresholdSize;

private:
    bool static IsEmpty (const tstring &entry) { return entry == _T(""); }
    bool static Find(const std::wstring& assemblyName, std::list<std::wstring> &list);

private:
    Timer m_timer;
    ULONG m_sequence;
    BYTE* m_formatBuffer;
    BYTE* m_currentBuffer;

    template<typename value_type> 
    void Write(value_type value) {
        *(value_type*)(m_currentBuffer) = value;
        m_currentBuffer += sizeof(value_type);
    }

public:
    void FunctionEnter2(
    /*[in]*/FunctionID                          funcID, 
    /*[in]*/UINT_PTR                            clientData, 
    /*[in]*/COR_PRF_FRAME_INFO                  func, 
    /*[in]*/COR_PRF_FUNCTION_ARGUMENT_INFO      *argumentInfo);

    void FunctionLeave2(
    /*[in]*/FunctionID                          funcID, 
    /*[in]*/UINT_PTR                            clientData, 
    /*[in]*/COR_PRF_FRAME_INFO                  func, 
    /*[in]*/COR_PRF_FUNCTION_ARGUMENT_RANGE     *retvalRange);

    void FunctionTailcall2(
    /*[in]*/FunctionID                          funcID, 
    /*[in]*/UINT_PTR                            clientData, 
    /*[in]*/COR_PRF_FRAME_INFO                  func);

// ICorProfilerCallback
public:
    virtual HRESULT STDMETHODCALLTYPE Initialize( 
        /* [in] */ IUnknown *pICorProfilerInfoUnk);
        
    virtual HRESULT STDMETHODCALLTYPE Shutdown( void);
     
    virtual HRESULT STDMETHODCALLTYPE AppDomainCreationStarted( 
        /* [in] */ AppDomainID appDomainId)	
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE AppDomainCreationFinished( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE AppDomainShutdownStarted( 
        /* [in] */ AppDomainID appDomainId)	
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE AppDomainShutdownFinished( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE AssemblyLoadStarted( 
        /* [in] */ AssemblyID assemblyId) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE AssemblyLoadFinished( 
        /* [in] */ AssemblyID assemblyId,
        /* [in] */ HRESULT hrStatus) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE AssemblyUnloadStarted( 
        /* [in] */ AssemblyID assemblyId) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE AssemblyUnloadFinished( 
        /* [in] */ AssemblyID assemblyId,
        /* [in] */ HRESULT hrStatus) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ModuleLoadStarted( 
        /* [in] */ ModuleID moduleId) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ModuleLoadFinished( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ModuleUnloadStarted( 
        /* [in] */ ModuleID moduleId) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ModuleUnloadFinished( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ModuleAttachedToAssembly( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ AssemblyID assemblyId) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ClassLoadStarted( 
        /* [in] */ ClassID classId)	
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ClassLoadFinished( 
        /* [in] */ ClassID classId,
        /* [in] */ HRESULT hrStatus) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ClassUnloadStarted( 
        /* [in] */ ClassID classId)	
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ClassUnloadFinished( 
        /* [in] */ ClassID classId,
        /* [in] */ HRESULT hrStatus) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE FunctionUnloadStarted( 
        /* [in] */ FunctionID functionId) 
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE JITCompilationStarted( 
        /* [in] */ FunctionID functionId,
        /* [in] */ BOOL fIsSafeToBlock)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE JITCompilationFinished( 
        /* [in] */ FunctionID functionId,
        /* [in] */ HRESULT hrStatus,
        /* [in] */ BOOL fIsSafeToBlock)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE JITCachedFunctionSearchStarted( 
        /* [in] */ FunctionID functionId,
        /* [out] */ BOOL *pbUseCachedFunction)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE JITCachedFunctionSearchFinished( 
        /* [in] */ FunctionID functionId,
        /* [in] */ COR_PRF_JIT_CACHE result)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE JITFunctionPitched( 
        /* [in] */ FunctionID functionId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE JITInlining( 
        /* [in] */ FunctionID callerId,
        /* [in] */ FunctionID calleeId,
        /* [out] */ BOOL *pfShouldInline)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ThreadCreated( 
        /* [in] */ ThreadID threadId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ThreadDestroyed( 
        /* [in] */ ThreadID threadId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ThreadAssignedToOSThread( 
        /* [in] */ ThreadID managedThreadId,
        /* [in] */ DWORD osThreadId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RemotingClientInvocationStarted( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RemotingClientSendingMessage( 
        /* [in] */ GUID *pCookie,
        /* [in] */ BOOL fIsAsync)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RemotingClientReceivingReply( 
        /* [in] */ GUID *pCookie,
        /* [in] */ BOOL fIsAsync)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RemotingClientInvocationFinished( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RemotingServerReceivingMessage( 
        /* [in] */ GUID *pCookie,
        /* [in] */ BOOL fIsAsync)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RemotingServerInvocationStarted( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RemotingServerInvocationReturned( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RemotingServerSendingReply( 
        /* [in] */ GUID *pCookie,
        /* [in] */ BOOL fIsAsync)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE UnmanagedToManagedTransition( 
        /* [in] */ FunctionID functionId,
        /* [in] */ COR_PRF_TRANSITION_REASON reason )
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ManagedToUnmanagedTransition( 
        /* [in] */ FunctionID functionId,
        /* [in] */ COR_PRF_TRANSITION_REASON reason)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RuntimeSuspendStarted( 
        /* [in] */ COR_PRF_SUSPEND_REASON suspendReason)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RuntimeSuspendFinished( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RuntimeSuspendAborted( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RuntimeResumeStarted( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RuntimeResumeFinished( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RuntimeThreadSuspended( 
        /* [in] */ ThreadID threadId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RuntimeThreadResumed( 
        /* [in] */ ThreadID threadId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE MovedReferences( 
        /* [in] */ ULONG cMovedObjectIDRanges,
        /* [size_is][in] */ ObjectID oldObjectIDRangeStart[  ],
        /* [size_is][in] */ ObjectID newObjectIDRangeStart[  ],
        /* [size_is][in] */ ULONG cObjectIDRangeLength[  ])
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ObjectAllocated( 
        /* [in] */ ObjectID objectId,
        /* [in] */ ClassID classId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ObjectsAllocatedByClass( 
        /* [in] */ ULONG cClassCount,
        /* [size_is][in] */ ClassID classIds[  ],
        /* [size_is][in] */ ULONG cObjects[  ])
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ObjectReferences( 
        /* [in] */ ObjectID objectId,
        /* [in] */ ClassID classId,
        /* [in] */ ULONG cObjectRefs,
        /* [size_is][in] */ ObjectID objectRefIds[  ])
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RootReferences( 
        /* [in] */ ULONG cRootRefs,
        /* [size_is][in] */ ObjectID rootRefIds[  ])
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionThrown( 
        /* [in] */ ObjectID thrownObjectId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionSearchFunctionEnter( 
        /* [in] */ FunctionID functionId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionSearchFunctionLeave( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionSearchFilterEnter( 
        /* [in] */ FunctionID functionId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionSearchFilterLeave( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionSearchCatcherFound( 
        /* [in] */ FunctionID functionId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionOSHandlerEnter( 
        /* [in] */ UINT_PTR __unused)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionOSHandlerLeave( 
        /* [in] */ UINT_PTR __unused)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionUnwindFunctionEnter( 
        /* [in] */ FunctionID functionId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionUnwindFunctionLeave( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionUnwindFinallyEnter( 
        /* [in] */ FunctionID functionId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionUnwindFinallyLeave( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionCatcherEnter( 
        /* [in] */ FunctionID functionId,
        /* [in] */ ObjectID objectId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionCatcherLeave( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE COMClassicVTableCreated( 
        /* [in] */ ClassID wrappedClassId,
        /* [in] */ REFGUID implementedIID,
        /* [in] */ void *pVTable,
        /* [in] */ ULONG cSlots)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE COMClassicVTableDestroyed( 
        /* [in] */ ClassID wrappedClassId,
        /* [in] */ REFGUID implementedIID,
        /* [in] */ void *pVTable)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionCLRCatcherFound( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ExceptionCLRCatcherExecute( void)
    { return S_OK; }

// ICorProfilerCallback2
public:
    virtual HRESULT STDMETHODCALLTYPE ThreadNameChanged( 
        /* [in] */ ThreadID threadId,
        /* [in] */ ULONG cchName,
        /* [in] */ 
        __in_ecount_opt(cchName)  WCHAR name[  ])
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE GarbageCollectionStarted( 
        /* [in] */ int cGenerations,
        /* [size_is][in] */ BOOL generationCollected[  ],
        /* [in] */ COR_PRF_GC_REASON reason)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE SurvivingReferences( 
        /* [in] */ ULONG cSurvivingObjectIDRanges,
        /* [size_is][in] */ ObjectID objectIDRangeStart[  ],
        /* [size_is][in] */ ULONG cObjectIDRangeLength[  ])
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE GarbageCollectionFinished( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE FinalizeableObjectQueued( 
        /* [in] */ DWORD finalizerFlags,
        /* [in] */ ObjectID objectID)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE RootReferences2( 
        /* [in] */ ULONG cRootRefs,
        /* [size_is][in] */ ObjectID rootRefIds[  ],
        /* [size_is][in] */ COR_PRF_GC_ROOT_KIND rootKinds[  ],
        /* [size_is][in] */ COR_PRF_GC_ROOT_FLAGS rootFlags[  ],
        /* [size_is][in] */ UINT_PTR rootIds[  ])
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE HandleCreated( 
        /* [in] */ GCHandleID handleId,
        /* [in] */ ObjectID initialObjectId)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE HandleDestroyed( 
        /* [in] */ GCHandleID handleId)
    { return S_OK; }

// ICorProfilerCallback3
public:
    virtual HRESULT STDMETHODCALLTYPE InitializeForAttach( 
        /* [in] */ IUnknown *pCorProfilerInfoUnk,
        /* [in] */ void *pvClientData,
        /* [in] */ UINT cbClientData)
    { return E_FAIL; }
        
    virtual HRESULT STDMETHODCALLTYPE ProfilerAttachComplete( void)
    { return S_OK; }
        
    virtual HRESULT STDMETHODCALLTYPE ProfilerDetachSucceeded( void)
    { return S_OK; }
};

OBJECT_ENTRY_AUTO(__uuidof(FunctionProfiler), CFunctionProfiler)
