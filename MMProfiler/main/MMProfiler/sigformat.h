// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
#include "sigparse.h"

#define dimensionof(a) 		(sizeof(a)/sizeof(*(a)))
#define MAKE_CASE(__elt) case __elt: return L ## #__elt;
#define MAKE_CASE_OR(__elt) case __elt: return L ##  #__elt L"|";

#ifdef DEBUG_XXXXX
#define DEBUG_PRINT Print 
#else
#define DEBUG_PRINT(x)  
#endif

class SigFormat : public SigParser
{
public:
    std::wstring returnType;
    std::wstring parameters;

private:
    std::wstring buffer;
    bool m_isFirstParam;
   	bool m_isArrayType;
    bool m_isPointerType;
    bool m_isReferenceType;
    IMetaDataImport2* m_metaDataImport2;
    mdMethodDef m_functionToken;
    mdTypeDef m_typeToken;

public:
    SigFormat(IMetaDataImport2* metaDataImport2, mdMethodDef functionToken) : 
        m_isFirstParam(true), 
        m_isArrayType(false), 
        m_isPointerType(false), 
        m_isReferenceType(false), 
        m_metaDataImport2(metaDataImport2),
        m_functionToken(functionToken)
    {
        metaDataImport2->GetMethodProps(functionToken, &m_typeToken, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL); 
    }

protected:
    LPCWSTR SigIndexTypeToString(sig_index_type sit);

    LPCWSTR SigMemberTypeOptionToString(sig_elem_type set);

    LPCWSTR SigMemberTypeToString(sig_elem_type set);

    LPCWSTR SigElementTypeToString(sig_elem_type set);

    // Simple wrapper around printf that prints the indenting spaces for you
    void Print(const WCHAR* format, ...)
    {
        va_list argList;
        va_start(argList, format);
        WCHAR buffer[1024] = {0};
        wvsprintf(buffer, format, argList);
        ATLTRACE(_T("%s"), W2T(buffer));
    }

    // a method with given elem_type
    virtual void NotifyBeginMethod(sig_elem_type elem_type)
    {
        DEBUG_PRINT(L"BEGIN METHOD");
    }

    virtual void NotifyEndMethod()
    {
        DEBUG_PRINT(L"END METHOD");
        if (m_isFirstParam) buffer += L"(";
        buffer += L")";
        parameters = buffer;
        buffer.clear();
    }

    // total parameters for the method
    virtual void NotifyParamCount(sig_count count)
    {
        DEBUG_PRINT(L"Param count = '%d'", count);
    }

    // starting a return type
    virtual void NotifyBeginRetType()
    {
        DEBUG_PRINT(L"BEGIN RET TYPE");
    }

    virtual void NotifyEndRetType()
    {
        DEBUG_PRINT(L"END RET TYPE");
        returnType = buffer;
        buffer.clear();
    }

    // starting a parameter
    virtual void NotifyBeginParam()
    {
        DEBUG_PRINT(L"BEGIN PARAM");
        if (m_isFirstParam)
        {
            m_isFirstParam = false;
            buffer += L"(";
        }
        else
        {
            buffer += L", ";
        }
    }

    virtual void NotifyEndParam()
    {
        DEBUG_PRINT(L"END PARAM");
    }

    // sentinel indication the location of the "..." in the method signature
    virtual void NotifySentinal()
    {
        DEBUG_PRINT(L"...");
        buffer += L"...";
    }

    // number of generic parameters in this method signature (if any)
    virtual void NotifyGenericParamCount(sig_count count)
    {
        DEBUG_PRINT(L"Generic param count = '%d'", count);
    }

    //----------------------------------------------------

    // a field with given elem_type
    virtual void NotifyBeginField(sig_elem_type elem_type)
    {
        DEBUG_PRINT(L"BEGIN FIELD: '%s%s'", SigMemberTypeOptionToString(elem_type), SigMemberTypeToString(elem_type));
    }

    virtual void NotifyEndField()
    {
        DEBUG_PRINT(L"END FIELD");
    }

    //----------------------------------------------------

    // a block of locals with given elem_type (always just LOCAL_SIG for now)
    virtual void NotifyBeginLocals(sig_elem_type elem_type)
    {
        DEBUG_PRINT(L"BEGIN LOCALS: '%s%s'", SigMemberTypeOptionToString(elem_type), SigMemberTypeToString(elem_type));
    }

    virtual void NotifyEndLocals()
    {
        DEBUG_PRINT(L"END LOCALS");
    }


    // count of locals with a block
    virtual void NotifyLocalsCount(sig_count count)
    {
        DEBUG_PRINT(L"Locals count: '%d'", count);
    }

    // starting a new local within a local block
    virtual void NotifyBeginLocal()
    {
        DEBUG_PRINT(L"BEGIN LOCAL");
    }

    virtual void NotifyEndLocal()
    {
        DEBUG_PRINT(L"END LOCAL");
    }


    // the only constraint available to locals at the moment is ELEMENT_TYPE_PINNED
    virtual void NotifyConstraint(sig_elem_type elem_type)
    {
        DEBUG_PRINT(L"Constraint: '%s%s'", SigMemberTypeOptionToString(elem_type), SigMemberTypeToString(elem_type));
    }


    //----------------------------------------------------

    // a property with given element type
    virtual void NotifyBeginProperty(sig_elem_type elem_type)
    {
        DEBUG_PRINT(L"BEGIN PROPERTY: '%s%s'", SigMemberTypeOptionToString(elem_type), SigMemberTypeToString(elem_type));
    }

    virtual void NotifyEndProperty()
    {
        DEBUG_PRINT(L"END PROPERTY");
    }


    //----------------------------------------------------

    // starting array shape information for array types
    virtual void NotifyBeginArrayShape()
    {
        DEBUG_PRINT(L"BEGIN ARRAY SHAPE");
    }

    virtual void NotifyEndArrayShape()
    {
        DEBUG_PRINT(L"END ARRAY SHAPE");
    }


    // array rank (total number of dimensions)
    virtual void NotifyRank(sig_count count)
    {
        DEBUG_PRINT(L"Rank: '%d'", count);
    }

    // number of dimensions with specified sizes followed by the size of each
    virtual void NotifyNumSizes(sig_count count)
    {
        DEBUG_PRINT(L"Num Sizes: '%d'", count);
    }

    virtual void NotifySize(sig_count count)
    {
        DEBUG_PRINT(L"Size: '%d'", count);
    }

    // BUG BUG lower bounds can be negative, how can this be encoded?
    // number of dimensions with specified lower bounds followed by lower bound of each 
    virtual void NotifyNumLoBounds(sig_count count)
    {
        DEBUG_PRINT(L"Num Low Bounds: '%d'", count);
    }

    virtual void NotifyLoBound(sig_count count)
    {
        DEBUG_PRINT(L"Low Bound: '%d'", count);
    }

    //----------------------------------------------------


    // starting a normal type (occurs in many contexts such as param, field, local, etc)
    virtual void NotifyBeginType()
    {
        DEBUG_PRINT(L"BEGIN TYPE");
    }

    virtual void NotifyEndType()
    {
        DEBUG_PRINT(L"END TYPE");
    }

    virtual void NotifyTypedByref()
    {
        DEBUG_PRINT(L"Typed byref");
        m_isReferenceType = true;
    }

    // the type has the 'byref' modifier on it -- this normally proceeds the type definition in the context
    // the type is used, so for instance a parameter might have the byref modifier on it
    // so this happens before the BeginType in that context
    virtual void NotifyByref()
    {
        DEBUG_PRINT(L"Byref");
        m_isReferenceType = true;
    }

    // the type is "VOID" (this has limited uses, function returns and void pointer)
    virtual void NotifyVoid()
    {
        DEBUG_PRINT(L"System.Void");
        buffer += L"System.Void";
    }

    // the type has the indicated custom modifiers (which can be optional or required)
    virtual void NotifyCustomMod(sig_elem_type cmod, sig_index_type indexType, sig_index index)
    {
        DEBUG_PRINT(
            L"Custom modifers: '%s', index type: '%s', index: '0x%x'",
            SigElementTypeToString(cmod),
            SigIndexTypeToString(indexType),
            index);
    }

    void AddPostfixes()
    {
        if (m_isArrayType)
        {
            m_isArrayType = false;
            buffer += L"[]";
        }

        if (m_isPointerType)
        {
            m_isPointerType = false;
            buffer += L"*";
        }

        if (m_isReferenceType)
        {
            m_isReferenceType = false;
            buffer += L"&";
        }
    }

    // the type is a simple type, the elem_type defines it fully
    virtual void NotifyTypeSimple(sig_elem_type  elem_type)
    {
        DEBUG_PRINT(L"Type simple: '%s'", SigElementTypeToString(elem_type));
        buffer += SigElementTypeToString(elem_type);
        AddPostfixes();
    }

    // the type is specified by the given index of the given index type (normally a type index in the type metadata)
    // this callback is normally qualified by other ones such as NotifyTypeClass or NotifyTypeValueType
    virtual void NotifyTypeDefOrRef(sig_index_type  indexType, int index)
    {
        DEBUG_PRINT(L"Type def or ref: '%s', index: '0x%x'", SigIndexTypeToString(indexType), index);
        GetDefOrRef(indexType, index);
        AddPostfixes();
    }

    void GetDefOrRef(sig_index_type indexType, sig_index index)
    {
        mdTypeDef typeToken = index;
		wchar_t typeName[512];
		ULONG typeNameLength = 512;
        if(indexType == SIG_INDEX_TYPE_TYPEDEF)
        {
            typeToken |= 0x02000000;
		    HRESULT hr = m_metaDataImport2->GetTypeDefProps(typeToken, typeName, typeNameLength, &typeNameLength, NULL, NULL);
		    if(FAILED(hr))
		    {
                return;
		    }
        }
        else if(indexType == SIG_INDEX_TYPE_TYPEREF)
        {
            typeToken |= 0x01000000;
		    HRESULT hr = m_metaDataImport2->GetTypeRefProps(typeToken, NULL, typeName, typeNameLength, &typeNameLength);
		    if(FAILED(hr))
		    {
                return;
		    }
        }
        else
        {
            return;
        }

        buffer += typeName;
    }

    // the type is an instance of a generic
    // elem_type indicates value_type or class
    // indexType and index indicate the metadata for the type in question
    // number indicates the number of type specifications for the generic types that will follow
    virtual void NotifyTypeGenericInst(sig_elem_type elem_type, sig_index_type indexType, sig_index index, sig_mem_number number)
    {
        DEBUG_PRINT(
            L"Type generic instance: '%s', index type: '%s', index: '0x%x', member number: '%d'",
            SigElementTypeToString(elem_type),
            SigIndexTypeToString(indexType),
            index,
            number);
        GetDefOrRef(indexType, index);
        buffer += L"<";
    }

    virtual void NotifyEndTypeGenericInst() 
    {
        DEBUG_PRINT(L"NotifyEndTypeGenericInst");
        buffer += L">";
    }

    virtual void NotifySepTypeGenericInst() 
    {
        buffer += L", ";
    }

    // the type is the type of the nth generic type parameter for the class
    virtual void NotifyTypeGenericTypeVariable(sig_mem_number number)
    {
        DEBUG_PRINT(L"Type generic type variable: number: '%d'", number);
        GetGenericParamName(m_typeToken, number);
        AddPostfixes();
    }

    void GetGenericParamName(mdToken token, sig_mem_number number)
    {
        HCORENUM hEnum = 0;
        mdGenericParam params[128] = {0}; 
        ULONG numParams = 128;

		HRESULT hr = m_metaDataImport2->EnumGenericParams(&hEnum, token, params, numParams, &numParams);
		if(FAILED(hr))
            return;

        wchar_t genericParamName[512] = {0};
		ULONG genericNameLength = 512;
		hr = m_metaDataImport2->GetGenericParamProps(params[number], NULL, NULL, NULL, NULL, genericParamName, genericNameLength, &genericNameLength);
		if(FAILED(hr))
			return;

		buffer += genericParamName;
    }

    // the type is the type of the nth generic type parameter for the member
    virtual void NotifyTypeGenericMemberVariable(sig_mem_number number)
    {
        DEBUG_PRINT(L"Type generic member variable: number: '%d'", number);
        GetGenericParamName(m_functionToken, number);
        AddPostfixes();
    }

    // the type will be a value type
    virtual void NotifyTypeValueType()
    {
        DEBUG_PRINT(L"Type value type");
    }

    // the type will be a class
    virtual void NotifyTypeClass()
    {
        DEBUG_PRINT(L"Type class");
    }

    // the type is a pointer to a type (nested type notifications follow)
    virtual void NotifyTypePointer()
    {
        DEBUG_PRINT(L"Type pointer");
        m_isPointerType = true;
    }

    // the type is a function pointer, followed by the type of the function
    virtual void NotifyTypeFunctionPointer()
    {
        DEBUG_PRINT(L"Type function pointer");
        m_isPointerType = true;
    }

    // the type is an array, this is followed by the array shape, see above, as well as modifiers and element type
    virtual void NotifyTypeArray()
    {
        DEBUG_PRINT(L"Type array");
        m_isArrayType = true;
    }

    // the type is a simple zero-based array, this has no shape but does have custom modifiers and element type
    virtual void NotifyTypeSzArray()
    {
        DEBUG_PRINT(L"Type sz array");
        m_isArrayType = true;
    }
};
