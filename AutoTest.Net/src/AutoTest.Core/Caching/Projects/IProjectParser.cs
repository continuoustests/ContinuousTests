namespace AutoTest.Core.Caching.Projects
{
    public interface IProjectParser
    {
        ProjectDocument Parse(string projectFile, ProjectDocument existingDocument);
    }
}