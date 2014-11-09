// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
#include "stdafx.h"
#include "sigformat.h"

LPCWSTR SigFormat::SigIndexTypeToString(sig_index_type sit)
{
    switch(sit)
    {
    default:
        DebugBreak();
        return L"unknown index type";
    MAKE_CASE(SIG_INDEX_TYPE_TYPEDEF)
    MAKE_CASE(SIG_INDEX_TYPE_TYPEREF)
    MAKE_CASE(SIG_INDEX_TYPE_TYPESPEC)
    }
}

LPCWSTR SigFormat::SigMemberTypeOptionToString(sig_elem_type set)
{
    switch(set & 0xf0)
    {
    default:
        DebugBreak();
        return L"unknown element type";
    case 0:
        return L"";
    MAKE_CASE_OR(SIG_GENERIC)
    MAKE_CASE_OR(SIG_HASTHIS)
    MAKE_CASE_OR(SIG_EXPLICITTHIS)
    }
}

LPCWSTR SigFormat::SigMemberTypeToString(sig_elem_type set)
{
    switch(set & 0xf)
    {
    default:
        DebugBreak();
        return L"unknown element type";
    MAKE_CASE(SIG_METHOD_DEFAULT)
    MAKE_CASE(SIG_METHOD_C)
    MAKE_CASE(SIG_METHOD_STDCALL)
    MAKE_CASE(SIG_METHOD_THISCALL)
    MAKE_CASE(SIG_METHOD_FASTCALL)
    MAKE_CASE(SIG_METHOD_VARARG)
    MAKE_CASE(SIG_FIELD)
    MAKE_CASE(SIG_LOCAL_SIG)
    MAKE_CASE(SIG_PROPERTY)
    }
}

LPCWSTR SigFormat::SigElementTypeToString(sig_elem_type set)
{
    switch(set)
    {
    default:
        DebugBreak();
        return L"unknown element type";
    MAKE_CASE(ELEMENT_TYPE_END)
    case ELEMENT_TYPE_VOID: return L"System.Void";
	case ELEMENT_TYPE_BOOLEAN: return L"System.Boolean";
    case ELEMENT_TYPE_CHAR: return L"System.Char";
    case ELEMENT_TYPE_I1: return L"System.SByte";
    case ELEMENT_TYPE_U1: return L"System.Byte";
	case ELEMENT_TYPE_I2: return L"System.Int16";
    case ELEMENT_TYPE_U2: return L"System.UInt16";
    case ELEMENT_TYPE_I4: return L"System.Int32";
    case ELEMENT_TYPE_U4: return L"System.UInt32";
    case ELEMENT_TYPE_I8: return L"System.Int64";
    case ELEMENT_TYPE_U8: return L"System.UInt64";
    case ELEMENT_TYPE_R4: return L"System.Float";
    case ELEMENT_TYPE_R8: return L"System.Double";
    case ELEMENT_TYPE_STRING: return L"System.String";
    MAKE_CASE(ELEMENT_TYPE_PTR)
    MAKE_CASE(ELEMENT_TYPE_BYREF)
    MAKE_CASE(ELEMENT_TYPE_VALUETYPE)
    MAKE_CASE(ELEMENT_TYPE_CLASS)
    MAKE_CASE(ELEMENT_TYPE_VAR)
    MAKE_CASE(ELEMENT_TYPE_ARRAY)
    MAKE_CASE(ELEMENT_TYPE_GENERICINST)
    MAKE_CASE(ELEMENT_TYPE_TYPEDBYREF)
    case ELEMENT_TYPE_I: return L"System.IntPtr";
	case ELEMENT_TYPE_U: return L"System.UIntPtr";
    MAKE_CASE(ELEMENT_TYPE_FNPTR)
	case ELEMENT_TYPE_OBJECT: return L"System.Object";
    MAKE_CASE(ELEMENT_TYPE_SZARRAY)
    MAKE_CASE(ELEMENT_TYPE_MVAR)
    MAKE_CASE(ELEMENT_TYPE_CMOD_REQD)
    MAKE_CASE(ELEMENT_TYPE_CMOD_OPT)
    MAKE_CASE(ELEMENT_TYPE_INTERNAL)
    MAKE_CASE(ELEMENT_TYPE_MODIFIER)
    MAKE_CASE(ELEMENT_TYPE_SENTINEL)
    MAKE_CASE(ELEMENT_TYPE_PINNED)
    }
}
