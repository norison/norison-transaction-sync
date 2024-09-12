using Notion.Client;

namespace Norison.TransactionSync.Persistence.Extensions;

public static class ArrayToRelationExtensions
{
    public static RelationPropertyValue ToRelationPropertyValue(this string[] ids)
    {
        return new RelationPropertyValue { Relation = ids.Select(id => new ObjectId { Id = id }).ToList() };
    }
    
    public static string[] ToArrayValue(this PropertyValue propertyValue)
    {
        if (propertyValue is not RelationPropertyValue relationPropertyValue)
        {
            throw new InvalidCastException("Property value is not a relation.");
        }
        
        return relationPropertyValue.Relation.Select(x => x.Id).ToArray();
    }
}