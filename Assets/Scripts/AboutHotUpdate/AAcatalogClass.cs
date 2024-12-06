using System.Collections.Generic;

public class AAcatalogData
{
    public string m_LocatorId { get; set; }
    public string m_BuildResultHash { get; set; }
    public InstanceProviderData m_InstanceProviderData { get; set; }
    public SceneProviderData m_SceneProviderData { get; set; }
    public List<ResourceProviderData> m_ResourceProviderData { get; set; }
    public List<string> m_ProviderIds { get; set; }
    public List<string> m_InternalIds { get; set; }
    public string m_KeyDataString { get; set; }
    public string m_BucketDataString { get; set; }
    public string m_EntryDataString { get; set; }
    public string m_ExtraDataString { get; set; }
}

public class InstanceProviderData
{
    public string m_Id { get; set; }
    public ObjectType m_ObjectType { get; set; }
    public string m_Data { get; set; }
}

public class SceneProviderData
{
    public string m_Id { get; set; }
    public ObjectType m_ObjectType { get; set; }
    public string m_Data { get; set; }
}

public class ResourceProviderData
{
    public string m_Id { get; set; }
    public ObjectType m_ObjectType { get; set; }
    public string m_Data { get; set; }
}

public class ObjectType
{
    public string m_AssemblyName { get; set; }
    public string m_ClassName { get; set; }
}
