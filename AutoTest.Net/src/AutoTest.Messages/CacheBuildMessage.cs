using System.IO;
using AutoTest.Messages;

namespace AutoTest.Messages
{
    public class CacheBuildMessage : ICustomBinarySerializable
    {
        public string Project { get; private set; }
        public BuildMessage BuildItem { get; private set; }

        public CacheBuildMessage(string project, BuildMessage result)
        {
            Project = project;
            BuildItem = result;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            // Overflow is fine, just wrap
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Project == null ? 0 : Project.GetHashCode());
                hash = hash * 23 + (BuildItem == null ? 0 : BuildItem.GetHashCode());
                return hash;
            }
        }

        public void SetDataFrom(BinaryReader reader)
        {
            Project = reader.ReadString();
            BuildItem = new BuildMessage();
            BuildItem.SetDataFrom(reader);
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(Project);
            BuildItem.WriteDataTo(writer);
        }
    }
}
