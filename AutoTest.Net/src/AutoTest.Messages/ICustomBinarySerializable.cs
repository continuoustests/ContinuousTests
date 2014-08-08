using System;
using System.IO;
namespace AutoTest.Messages
{
	public interface ICustomBinarySerializable  
	{  
	    void WriteDataTo(BinaryWriter writer);  
	    void SetDataFrom(BinaryReader reader); 
	}  
}

