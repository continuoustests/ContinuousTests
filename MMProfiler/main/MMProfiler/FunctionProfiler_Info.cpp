#include "stdafx.h"
#include "FunctionProfiler.h"

AssemblyID CFunctionProfiler::GetModuleName(ModuleID moduleId, std::wstring& moduleName)
{
    ULONG dwNameSize = 512;
    WCHAR szModuleName[512] = {};
    AssemblyID assemblyId = 0;
    COM_FAIL_RETURN(m_profilerInfo2->GetModuleInfo(moduleId, NULL, dwNameSize, &dwNameSize, szModuleName, &assemblyId), 0);
    moduleName = szModuleName;
    return assemblyId;
}

void CFunctionProfiler::GetAssemblyName(AssemblyID assemblyId, std::wstring& assemblyName)
{
    ULONG dwNameSize = 512; 
    WCHAR szAssemblyName[512] = {};
    COM_FAIL(m_profilerInfo2->GetAssemblyInfo(assemblyId, dwNameSize, &dwNameSize, szAssemblyName, NULL, NULL));
    assemblyName = szAssemblyName;
    return;
}

//
// encapsulate getting correct ClassID for FunctionID whilst wrking around known bug
//
// http://social.msdn.microsoft.com/Forums/en/netfxtoolsdev/thread/ed6f972f-712a-48df-8cce-74f8951503fa
void CFunctionProfiler::GetClassIDForFunctionID(FunctionID funcId, COR_PRF_FRAME_INFO func, ClassID &funcClass, mdToken &funcToken, IMetaDataImport2** ppMeta)
{
	ModuleID funcModule;
	
    mdToken token;
	COM_FAIL(m_profilerInfo2->GetFunctionInfo2(funcId, func, &funcClass, NULL, &token, 0, NULL, NULL));

    CComPtr<IMetaDataImport2> metaDataImport2;
    COM_FAIL(m_profilerInfo2->GetTokenAndMetaDataFromFunction(funcId, IID_IMetaDataImport, (IUnknown **)ppMeta, &funcToken));

    mdTypeDef classToken = mdTypeDefNil;
    COM_FAIL((*ppMeta)->GetMethodProps(funcToken, &classToken, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL));

    ClassID parentClassId;
    mdTypeDef typeDefToken;
    ClassID tempId = funcClass;

    while (funcClass != NULL)
    {
        COM_FAIL(m_profilerInfo2->GetClassIDInfo2(funcClass, NULL, &typeDefToken, &parentClassId, 0, NULL, NULL));
        if (typeDefToken == classToken)
            break;
        funcClass = parentClassId;
    }

    if (funcClass==NULL) // backup - plan
    {
        funcClass == tempId;
    }
}