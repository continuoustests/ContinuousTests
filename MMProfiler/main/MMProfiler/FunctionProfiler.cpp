// FunctionProfiler.cpp : Implementation of CFunctionProfiler

#include "stdafx.h"
#include "FunctionProfiler.h"
#include "NativeCallback.h"

#include <boost\algorithm\string.hpp>

#define LIST_SIZE 32767

#pragma pack(push)
#pragma pack(1)

typedef struct ProfilerData
{
    int bufferSize;
    int thresholdSize;
    WCHAR includes[32768];
    WCHAR excludes[32768];
};

#pragma pack(pop)

CFunctionProfiler* CFunctionProfiler::g_pProfiler = NULL;
// CFunctionProfiler

HRESULT STDMETHODCALLTYPE CFunctionProfiler::Initialize( 
    /* [in] */ IUnknown *pICorProfilerInfoUnk) 
{
    ATLTRACE(_T("::Initialize"));

    m_profilerInfo2 = pICorProfilerInfoUnk;
    if (m_profilerInfo2 != NULL) ATLTRACE(_T("    ::Initialize (m_profilerInfo2 OK)"));
    if (m_profilerInfo2 == NULL) return E_FAIL;
    m_profilerInfo3 = pICorProfilerInfoUnk;
    if (m_profilerInfo3 != NULL) ATLTRACE(_T("    ::Initialize (m_profilerInfo3 OK)"));

    TCHAR tag[100];
    ::ZeroMemory(tag, 100);
    ::GetEnvironmentVariable(_T("MMProfiler_Tag"), tag, 100);
    tstring wTag(tag);

    HANDLE hMapControlFile = OpenFileMapping(
        FILE_MAP_ALL_ACCESS,
        false,
        (_T("Local\\MMProfilerMapControl") + wTag).c_str()
        );

    if (hMapControlFile == NULL)
    {
        ATLTRACE(_T("    Failed to open control map file (GetLastError => %d)"), ::GetLastError());
        return E_FAIL;
    }

    m_hMapFile = OpenFileMapping(
        FILE_MAP_ALL_ACCESS,
        false,
        (_T("Local\\MMProfilerMapBuffer") + wTag).c_str()
        );

    if(m_hMapFile == NULL)
    {
        ATLTRACE(_T("    Failed to open buffer map file (GetLastError => %d)"), ::GetLastError());
        return E_FAIL;
    }

    m_hReadBufferEvent.Initialise((_T("Local\\MM_ReadBuffer") + wTag).c_str());
    if (!m_hReadBufferEvent.IsValid())
    {
        ATLTRACE(_T("failed to open event m_hReadBufferEvent (GetLastError => %d)"), ::GetLastError());
        return E_FAIL;
    }

    m_hBufferReadEvent.Initialise((_T("Local\\MM_BufferRead") + wTag).c_str());
    if (!m_hBufferReadEvent.IsValid())
    {
        ATLTRACE(_T("failed to open event m_hBufferReadEvent (GetLastError => %d)"), ::GetLastError());
        return E_FAIL;
    }
    m_hBufferReadEvent.Reset();

    m_hProcessMutex.Initialise((_T("Local\\MM_Profiler_BufferAccess") + wTag).c_str());
    if (!m_hProcessMutex.IsValid())
    {
        ATLTRACE(_T("failed to open mutex m_hProcessMutex (GetLastError => %d)"), ::GetLastError());
        return E_FAIL;
    }

    ProfilerData *profilerData = (ProfilerData*)MapViewOfFile(
        hMapControlFile,
        FILE_MAP_ALL_ACCESS,
        0,
        0,
        sizeof(ProfilerData)
        ); 

    boost::split(m_excludes, profilerData->excludes, boost::is_any_of(L", "));
    m_excludes.remove_if(IsEmpty);
    m_excludes.push_front(L"System");
    m_excludes.push_front(L"mscorlib");

    boost::split(m_includes, profilerData->includes, boost::is_any_of(L", "));
    m_includes.remove_if(IsEmpty);

    m_bufferSize = profilerData->bufferSize;
    m_thresholdSize = profilerData->thresholdSize;

    for (std::list<tstring>::iterator it = m_includes.begin(); it != m_includes.end(); ++it)
    {
        ATLTRACE(_T("include => %s"), (*it).c_str());
    }
    for (std::list<tstring>::iterator it = m_excludes.begin(); it != m_excludes.end(); ++it)
    {
        ATLTRACE(_T("exclude => %s"), (*it).c_str());
    }

    if (profilerData!=NULL) 
    {
        UnmapViewOfFile(profilerData);
    }
    CloseHandle(hMapControlFile);

    m_pView = (BYTE*)MapViewOfFile(
        m_hMapFile,
        FILE_MAP_ALL_ACCESS,
        0,
        0,
        m_bufferSize + m_thresholdSize + sizeof(int)
        );

    if (m_pView == NULL)
    {
        ATLTRACE(_T("Failed to map view file (GetLastError => %d)"), ::GetLastError());
        return E_FAIL;
    }

    ATLTRACE(_T("LISTS - Include %d, Exclude %d"), m_includes.size(), m_excludes.size());

    DWORD dwMask = 0;
    dwMask |= COR_PRF_MONITOR_ENTERLEAVE;           // Controls the FunctionEnter, FunctionLeave, and FunctionTailcall callbacks.
    dwMask |= COR_PRF_ENABLE_FRAME_INFO;            // ensures that we have frameinfo in the callback (important for generics)

    m_profilerInfo2->SetEventMask(dwMask);

    if(m_profilerInfo3 != NULL)
        m_profilerInfo3->SetFunctionIDMapper2(FunctionMapper2, this);
    else
        m_profilerInfo2->SetFunctionIDMapper(FunctionMapper);

    m_formatBuffer = m_pView + sizeof(long);
    m_currentBuffer = m_formatBuffer;

    g_pProfiler = this;

    m_profilerInfo2->SetEnterLeaveFunctionHooks2(
        _FunctionEnter2, 
        _FunctionLeave2, 
        _FunctionTailcall2);

    return S_OK;
}

HRESULT STDMETHODCALLTYPE CFunctionProfiler::Shutdown( void) 
{ 
    ATLTRACE(_T("::Shutdown"));
    g_pProfiler = NULL;
    SharedShutdown();
    return S_OK; 
}

void CFunctionProfiler::SharedShutdown()
{
    ATLTRACE(_T("::SharedShutdown"));
    if ((++m_nShutdownCount)==1)
    {
        if (m_pView!=NULL) 
        {
            UnmapViewOfFile(m_pView);
        }
        CloseHandle(m_hMapFile);
    }
}

bool CFunctionProfiler::Find(const std::wstring& methodName, std::list<std::wstring>&list)
{
    for (std::list<std::wstring>::iterator it = list.begin(); it != list.end(); ++it)
    {
         if (methodName.find(*it)==0) return true;
    }
    return false;
}

bool CFunctionProfiler::Include(const std::wstring& methodName, bool* recordMeta)
{
    *recordMeta = false;
    if (methodName.find(L"::")==0) return true; // method of innerclass - known issues until we have a proper stack frame
    *recordMeta = true;
    if (m_includes.size()>0)
    {
        return Find(methodName, m_includes);
    }
    return !Find(methodName, m_excludes);
}

UINT_PTR CFunctionProfiler::FunctionMapper2(FunctionID functionId, void* clientData, BOOL* pbHookFunction)
{
    UINT_PTR retVal = functionId;
    *pbHookFunction = FALSE;
    CFunctionProfiler* profiler = static_cast<CFunctionProfiler*>(clientData);
    if(profiler == NULL)
        return 0;

    CComQIPtr<ICorProfilerInfo2> profilerInfo2 = profiler->m_profilerInfo2;

    std::wstring methodName;
    profiler->GetMetadataFullMethodName(functionId, NULL, methodName);
    
    bool recordMeta;
    if (!profiler->Include(methodName, &recordMeta)) 
        return 0;

    CComCritSecLock<CComAutoCriticalSection> lock(profiler->m_cs);

    FunctionInfo fi;
    if (recordMeta) fi.metadataSignature = methodName;
    profiler->m_functions.push_back(fi);
    retVal = profiler->m_AssignedFunctionId++;

    *pbHookFunction = TRUE;
    
    return retVal;
}

UINT_PTR CFunctionProfiler::FunctionMapper(FunctionID functionId, BOOL* pbHookFunction)
{
    return FunctionMapper2(functionId, g_pProfiler, pbHookFunction);
}

void CFunctionProfiler::WriteData()
{
    DWORD size = m_currentBuffer - m_formatBuffer;
    *((DWORD*)m_pView) = size;

    if (size >= m_bufferSize)
    {
        m_hReadBufferEvent.SignalAndWait(m_hBufferReadEvent);
        m_hBufferReadEvent.Reset();
        *((DWORD*)m_pView) = 0;
    }
}

void CFunctionProfiler::FunctionEnter2(
    /*[in]*/FunctionID                          funcID, 
    /*[in]*/UINT_PTR                            clientData, 
    /*[in]*/COR_PRF_FRAME_INFO                  func, 
    /*[in]*/COR_PRF_FUNCTION_ARGUMENT_INFO      *argumentInfo)
{
    ThreadID threadId;
    m_profilerInfo2->GetCurrentThreadID(&threadId);
    DWORD windowThreadId;
    m_profilerInfo2->GetThreadInfo(threadId, &windowThreadId);

    CComCritSecLock<CComAutoCriticalSection> lock(m_cs);
    FunctionInfo & fi = m_functions[clientData];
    if (!fi.trace) return;

    if (!fi.assigned)
    {
        fi.assigned = true;
        std::wstring szMethodName;
        GetMetadataFullMethodName(funcID, func, szMethodName);
        bool temp;
        if (!Include(szMethodName, &temp)) {fi.trace=false; return; }                                                           
        GetMetadataFullMethodNameWithSignature(funcID, func, fi.metadataSignature);
    }
    std::wstring runtimeSignature;
    GetRuntimeFullMethodSignature(funcID, func, runtimeSignature);
    
    CScopedLock<CMutex> lock2(m_hProcessMutex);
    m_currentBuffer = m_formatBuffer + (*((DWORD*)m_pView));

    BYTE type = 1;
    double elapsed = m_timer.Elapsed();

    Write(type);
    Write(m_sequence);
    Write(elapsed);
    Write(windowThreadId);
    Write<ULONGLONG>(funcID);

    m_sequence++;

    DWORD len1 = (DWORD)runtimeSignature.length();
    Write(len1);

    memcpy(m_currentBuffer, runtimeSignature.c_str(), len1 * 2);
    m_currentBuffer += (len1 * 2);

    DWORD len2 = (DWORD)fi.metadataSignature.length();
    Write(len2);

    memcpy(m_currentBuffer, fi.metadataSignature.c_str(), len2 * 2);
    m_currentBuffer += (len2 * 2);
    
    WriteData();
}

void CFunctionProfiler::FunctionLeave2(
    /*[in]*/FunctionID                          funcID, 
    /*[in]*/UINT_PTR                            clientData, 
    /*[in]*/COR_PRF_FRAME_INFO                  func, 
    /*[in]*/COR_PRF_FUNCTION_ARGUMENT_RANGE     *retvalRange)
{
    CComCritSecLock<CComAutoCriticalSection> lock(m_cs);
    FunctionInfo & fi = m_functions[clientData];
    if (!fi.trace) return;

    ThreadID threadId;
    m_profilerInfo2->GetCurrentThreadID(&threadId);
    DWORD windowThreadId;
    m_profilerInfo2->GetThreadInfo(threadId, &windowThreadId);
    
    CScopedLock<CMutex> lock2(m_hProcessMutex);
    m_currentBuffer = m_formatBuffer + (*((DWORD*)m_pView));

    BYTE type = 2;
    double elapsed = m_timer.Elapsed();

    Write(type);
    Write(m_sequence);
    Write(elapsed);
    Write(windowThreadId);
    Write<ULONGLONG>(funcID);

    m_sequence++;
    
    WriteData();
}

void CFunctionProfiler::FunctionTailcall2(
    /*[in]*/FunctionID                          funcID, 
    /*[in]*/UINT_PTR                            clientData, 
    /*[in]*/COR_PRF_FRAME_INFO                  func)
{
    CComCritSecLock<CComAutoCriticalSection> lock(m_cs);
    FunctionInfo & fi = m_functions[clientData];
    if (!fi.trace) return;

    ThreadID threadId;
    m_profilerInfo2->GetCurrentThreadID(&threadId);
    DWORD windowThreadId;
    m_profilerInfo2->GetThreadInfo(threadId, &windowThreadId);
    
    CScopedLock<CMutex> lock2(m_hProcessMutex);
    m_currentBuffer = m_formatBuffer + (*((DWORD*)m_pView));

    BYTE type = 3;
    double elapsed = m_timer.Elapsed();

    Write(type);
    Write(m_sequence);
    Write(elapsed);
    Write(windowThreadId);
    Write<ULONGLONG>(funcID);

    m_sequence++;
    
    WriteData();
}
