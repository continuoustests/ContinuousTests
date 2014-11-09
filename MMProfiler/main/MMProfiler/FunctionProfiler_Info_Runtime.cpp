#include "stdafx.h"
#include "FunctionProfiler.h"
#include "SimpleArray.h"

void CFunctionProfiler::AddRuntimeGenericParts(ULONG32 numArgs, ClassID * classArgs, std::wstring& name)
{
    if (numArgs > 0)
    {
        name.append(L"<");
        for (ULONG32 i = 0; i < numArgs; i++)
        {
            if (i > 0) name.append(L", ");
            std::wstring className;
            GetRuntimeClassSignature(classArgs[i], className);
            name.append(className);
        }
        name.append(L">");
    }
}

void CFunctionProfiler::GetRuntimeClassSignature(ClassID classId, std::wstring& className)
{
    ULONG32 numArgs = 0;
    ModuleID moduleId;
    ClassID parentId;           // NOTE: this IS the base class
    mdTypeDef classToken;
    HRESULT hr = m_profilerInfo2->GetClassIDInfo2(classId, NULL, NULL, NULL, 0, &numArgs, NULL);
    if (CORPROF_E_CLASSID_IS_ARRAY == hr)
    {
        ClassID arrayClassId;
        COM_FAIL(m_profilerInfo2->IsArrayClass(classId, NULL, &arrayClassId, NULL));
        std::wstring tempName;
        GetRuntimeClassSignature(arrayClassId, tempName);
        tempName += L"[]";
        className = tempName;
        return;
    }
    else 
    {
        COM_FAIL1(hr, classId);
    }

    SimpleArray<ClassID> classArgs(numArgs);
    COM_FAIL(m_profilerInfo2->GetClassIDInfo2(classId, &moduleId, &classToken, &parentId, numArgs, &numArgs, classArgs));
    CComPtr<IMetaDataImport2> metaDataImport2;
    COM_FAIL(m_profilerInfo2->GetModuleMetaData(moduleId, ofRead, IID_IMetaDataImport2, (IUnknown**) &metaDataImport2));
    WCHAR szClassName[512] = {};
    DWORD typeDefFlags = 0;
    COM_FAIL(metaDataImport2->GetTypeDefProps(classToken, szClassName, 512, NULL, &typeDefFlags, NULL));
    className = szClassName;
    AddRuntimeGenericParts(numArgs, classArgs, className);
}

void CFunctionProfiler::GetRuntimeMethodSignature(FunctionID funcID, COR_PRF_FRAME_INFO func, IMetaDataImport2* metaDataImport2, std::wstring& methodName)
{
    ULONG32 numArgs = 0;

    COM_FAIL(m_profilerInfo2->GetFunctionInfo2(funcID, NULL, NULL,NULL, NULL, 0, &numArgs, NULL)); 
    SimpleArray<ClassID> classArgs(numArgs);
    ModuleID moduleId;
    mdToken funcToken;
    COM_FAIL(m_profilerInfo2->GetFunctionInfo2(funcID, NULL, NULL, &moduleId, &funcToken, numArgs, &numArgs, classArgs)); 

    WCHAR szMethodName[512] = {};
    COM_FAIL(metaDataImport2->GetMethodProps(funcToken, NULL, szMethodName, 512, NULL, NULL, NULL, NULL, NULL, NULL));
    methodName = szMethodName;
    AddRuntimeGenericParts(numArgs, classArgs, methodName);
}

void CFunctionProfiler::GetRuntimeFullMethodSignature(FunctionID funcID, COR_PRF_FRAME_INFO func, std::wstring& methodName)
{
    ClassID funcClass;
    CComPtr<IMetaDataImport2> metaDataImport2;
	mdToken funcToken;
	GetClassIDForFunctionID(funcID, func, funcClass, funcToken, &metaDataImport2);

    std::wstring className;
    CFunctionProfiler::g_pProfiler->GetRuntimeClassSignature(funcClass, className);

    CFunctionProfiler::g_pProfiler->GetRuntimeMethodSignature(funcID, NULL, metaDataImport2, methodName);
        
    methodName.insert(0, L"::");
    methodName.insert(0, className);
}

