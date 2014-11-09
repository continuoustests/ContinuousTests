#include "stdafx.h"
#include "FunctionProfiler.h"
#include "SimpleArray.h"

#include "sigformat.h"

void CFunctionProfiler::GetMetadataGenericSignature(mdToken token, IMetaDataImport2* metaDataImport2, std::wstring &name)
{
    HCORENUM hEnum = 0;
    mdGenericParam params[128] = {0}; 
    ULONG numParams = 128;
    
    COM_FAIL(metaDataImport2->EnumGenericParams(&hEnum, token, params, numParams, &numParams));

    if (numParams > 0)
    {
        std::wstring genericSignature(L"<");
        for(ULONG g = 0; g < numParams; ++g)
        {
            if (g > 0) genericSignature.append(L", ");
            
            ULONG nameLength = 512;
            WCHAR szName[512] = {};
            COM_FAIL(metaDataImport2->GetGenericParamProps(params[g], NULL, NULL, NULL, NULL, szName, nameLength, &nameLength));
            genericSignature.append(szName);
        }
        genericSignature.append(L">");
        name.append(genericSignature);
    }
}

void CFunctionProfiler::GetMetadataClassName(ModuleID moduleId, mdTypeDef tokenTypeDef, std::wstring &className)
{
    CComPtr<IMetaDataImport2> metaDataImport;
    COM_FAIL(m_profilerInfo2->GetModuleMetaData(moduleId, ofRead, IID_IMetaDataImport2, (IUnknown**) &metaDataImport));
    GetMetadataClassName(tokenTypeDef, metaDataImport, className);
}

void CFunctionProfiler::GetMetadataClassName(mdTypeDef tokenTypeDef, IMetaDataImport2* metaDataImport2, std::wstring &className)
{
    ULONG dwNameSize = 512;
    WCHAR szClassName[512] = {};
    DWORD typeDefFlags = 0;

    COM_FAIL(metaDataImport2->GetTypeDefProps(tokenTypeDef, szClassName, dwNameSize, &dwNameSize, &typeDefFlags, NULL));
    className = szClassName;

    GetMetadataGenericSignature(tokenTypeDef, metaDataImport2, className);

    if (!IsTdNested(typeDefFlags)) 
        return;
    
    mdTypeDef parentTypeDef;
    COM_FAIL(metaDataImport2->GetNestedClassProps(tokenTypeDef, &parentTypeDef));
    std::wstring parentClass;
    GetMetadataClassName(parentTypeDef, metaDataImport2, parentClass);
    className.insert(0, L"+");
    className.insert(0, parentClass);
}

void CFunctionProfiler::GetMetadataMethodName(IMetaDataImport2* metaDataImport2, mdMethodDef tokenMethodDef, std::wstring &methodName) 
{
	HRESULT hr;
    ULONG dwNameSize = 512;
    WCHAR szMethodName[512] = {0};
    ULONG cbSigBlob;

	COM_FAIL(metaDataImport2->GetMethodProps(tokenMethodDef, NULL, szMethodName, dwNameSize, &dwNameSize, NULL, NULL, NULL, NULL, NULL));

    methodName = szMethodName;

    GetMetadataGenericSignature(tokenMethodDef, metaDataImport2, methodName);
}

void CFunctionProfiler::GetMetadataClassName(ClassID classId, std::wstring &className)
{
    if (classId == 0) return;
    ModuleID moduleId;
	mdTypeDef typeDef;
	COM_FAIL(m_profilerInfo2->GetClassIDInfo(classId, &moduleId, &typeDef));
    GetMetadataClassName(moduleId, typeDef, className);
}

void CFunctionProfiler::GetMetadataFullMethodName(FunctionID funcId, COR_PRF_FRAME_INFO func, std::wstring &fullMethodName)
{
    ClassID funcClass;
    CComPtr<IMetaDataImport2> metaDataImport2;
	mdToken funcToken;
	GetClassIDForFunctionID(funcId, func, funcClass, funcToken, &metaDataImport2);

    std::wstring className;
    GetMetadataClassName(funcClass, className);
    std::wstring methodName;
    GetMetadataMethodName(metaDataImport2, funcToken, methodName);

    fullMethodName = className + L"::" + methodName;
}

void CFunctionProfiler::GetMetadataFullMethodNameWithSignature(FunctionID funcId, COR_PRF_FRAME_INFO func, std::wstring &fullMethodName)
{
    ClassID funcClass;
    CComPtr<IMetaDataImport2> metaDataImport2;
	mdToken funcToken;
	GetClassIDForFunctionID(funcId, func, funcClass, funcToken, &metaDataImport2);

    std::wstring className;
    GetMetadataClassName(funcClass, className);
    std::wstring methodName;
    GetMetadataMethodName(metaDataImport2, funcToken, methodName);

    std::wstring returnType;
    std::wstring parameters;
    GetMetadataMethodSignatureTypes(metaDataImport2, funcToken, returnType, parameters);

    fullMethodName = returnType + L" " + className + L"::" + methodName + parameters;
}

void CFunctionProfiler::GetMetadataMethodSignatureTypes(IMetaDataImport2* metaDataImport2, mdMethodDef tokenMethodDef, std::wstring &returnType, std::wstring &parameters) 
{
	HRESULT hr;
    PCCOR_SIGNATURE pvSigBlob = NULL;
    ULONG cbSigSize = 0;
    COM_FAIL(metaDataImport2->GetMethodProps(tokenMethodDef, NULL, NULL, 0, NULL, NULL, &pvSigBlob, &cbSigSize, NULL, NULL));

    SigFormat format(metaDataImport2, tokenMethodDef);
    format.Parse((sig_byte*)pvSigBlob, cbSigSize);
    returnType = format.returnType;
    parameters = format.parameters;
}
